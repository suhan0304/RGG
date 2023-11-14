using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// 노트 생성 코드
public class NoteGenerator : MonoBehaviour
{

    static NoteGenerator instance;
    public static NoteGenerator Instance
    {
        get
        {
            return instance;
        }
    }

    //노트 생성 위치 설정을 위한 객체
    [SerializeField] Transform[] GenerateLocation = null;

    //판정 라인 위치 설정을 위한 객체
    [SerializeField] Transform JudgeLine = null;


    public GameObject parent;
    public GameObject notePrefab;
    public GameObject LinePrefab;

    //기본 interval, interval 단위로 노트를 생성
    readonly float defaultInterval = 0.5f; // 1배속 기준점 (1마디 전체가 화면에 그려지는 정도를 정의)
    public float Interval { get; private set; }

    //Object Pool - Short Note용 생성
    IObjectPool<NoteShort> poolShort;
    public IObjectPool<NoteShort> PoolShort
    {
    get
        {
            if (poolShort == null)
            {
                poolShort = new ObjectPool<NoteShort>(CreatePooledShort, defaultCapacity: 256);
            }
            return poolShort;
        }
    }
    //Object Pool (short) : Short Note 생성
    NoteShort CreatePooledShort()
    {
        GameObject note = Instantiate(notePrefab);          //Short Note - Instantiate를 이용해 notePrefab을 복사해서 생성
        note.transform.SetParent(parent.transform, false);  //생성한 노트 Parent 설정 : Parent 객체는 Notes(Empty Object)로 설정되어 있음

        note.AddComponent<NoteShort>();                     //생성한 Note Object 에 NoteShort Component 추가
        return note.GetComponent<NoteShort>();              //생성 완료된 Note Object를 반환 
    }

    //Object Pool - Long Note용 생성
    IObjectPool<NoteLong> poolLong;
    public IObjectPool<NoteLong> PoolLong
    {
        get
        {
            if (poolLong == null)
            {
                poolLong = new ObjectPool<NoteLong>(CreatePooledLong, defaultCapacity: 64);
            }
            return poolLong;
        }
    }
    //Object Pool (Long) : Long Note 생성
    NoteLong CreatePooledLong()
    {
        GameObject note = new GameObject("NoteLong");
        note.transform.SetParent(parent.transform, false);
        
        GameObject head = Instantiate(notePrefab);
        head.name = "head";
        head.transform.SetParent(note.transform, false);

        GameObject tail = Instantiate(notePrefab);
        tail.name = "tail";
        tail.transform.SetParent(note.transform, false);

        GameObject line = Instantiate(LinePrefab);
        line.name = "line";
        line.transform.SetParent(note.transform, false);

        note.AddComponent<NoteLong>();
        return note.GetComponent<NoteLong>();
    }

    
    int currentBar = 3; // 최초 플레이 시 3마디 먼저 생성
    int next = 0;
    int prev = 0;

    //생성된 Note 들이 담길 List
    public List<NoteObject> toReleaseList = new List<NoteObject>();

    Coroutine coGenTimer;
    Coroutine coReleaseTimer;
    Coroutine coInterpolate;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }


    //노트 파괴
    public List<Queue<NoteObject>> notes_queues = new List<Queue<NoteObject>>();

    [SerializeField] public Queue<NoteObject> notes_queue1 = new Queue<NoteObject>();
    [SerializeField] public Queue<NoteObject> notes_queue2 = new Queue<NoteObject>();
    [SerializeField] public Queue<NoteObject> notes_queue3 = new Queue<NoteObject>();
    [SerializeField] public Queue<NoteObject> notes_queue4 = new Queue<NoteObject>();

    //라인별 노트들이 담길 Queue를 한 스테이지 시작마다 초기화
    public void noteQueueInit()
    {
        foreach (var note_queue in notes_queues)
        {
            note_queue.Clear();
        }
        notes_queues.Clear();

        notes_queues.Add(notes_queue1);
        notes_queues.Add(notes_queue2);
        notes_queues.Add(notes_queue3);
        notes_queues.Add(notes_queue4);
    }

    //판정이 완료된 노트는 바로 Object Pool에 Release를 진행
    public void FallNoteDequeue(int line)
    {
        NoteObject note = notes_queues[line].Dequeue();
    }

    //판정이 완료된 노트는 바로 Object Pool에 Release를 진행
    public void judgedNoteRelease(int line)
    {
        NoteObject note = notes_queues[line].Dequeue();

        if (note is NoteShort)
            PoolShort.Release(note as NoteShort);
        else
            PoolLong.Release(note as NoteLong);

        //Release 후 비활성화
        note.gameObject.SetActive(false);
    }

    // 풀링 기반 생성 (게임 플레이 시 사용)
    public void StartGen()
    {
        noteQueueInit();                                            // 노트 생성 전 판정 노트 파괴용 큐 초기화
        Interval = defaultInterval * GameManager.Instance.Speed;    // interver = 기본 interval * 속도 
        coGenTimer = StartCoroutine(IEGenTimer(GameManager.Instance.sheets[GameManager.Instance.title].BarPerMilliSec * 0.001f)); // 음악의 1마디 시간(BarPerMilliSec)마다 생성할 노트 오브젝트 탐색 
        coReleaseTimer = StartCoroutine(IEReleaseTimer(GameManager.Instance.sheets[GameManager.Instance.title].BarPerMilliSec * 0.001f * 0.5f)); // 1마디 시간(BarPerMilliSec)의 절반 주기로 해제할 노트 오브젝트 탐색
        coInterpolate = StartCoroutine(IEInterpolate(0.1f, 4f));    //노트들의 위치를 설정해주는 Interpolate Coroutine 실행
    }

    void Gen()
    {

        List<Note> notes = GameManager.Instance.sheets[GameManager.Instance.title].notes;   //title에 맞는 sheet 정보에서 노트들의 정보를 가져옴
        List<Note> reconNotes = new List<Note>();                                           


        // interval 시간 단위로 마디 생성을 위해 생성해야할 노트 확인
        for (; next < notes.Count; next++)
        {
            // currentBar의 초기값 = 3 : CurrentBar는 GenTimer에서 지속적으로 BarPerMilliSec가 지날 때마다 1씩 증가 시켜줌
            // currentBar * BarPerMilliSec 의 값은 3마디가 진행되는 시간 : 노트의 판정 시간이 앞에서 구한 시간보다 커질때까지 next를 증가
            // 이 과정을 통해 앞으로 한 마디(Bar)마다 만들어야 할 노트들의 번호를 찾을 수 있음 ( Note[next]번까지 생성 )
            if (notes[next].time > currentBar * GameManager.Instance.sheets[GameManager.Instance.title].BarPerMilliSec)
            {
                break;
            }
        }

        // 생성 예정 노트(note[next]번까지)를 reconNotes안에 추가
        for (int j = prev; j < next; j++)
        {
            reconNotes.Add(notes[j]);
        }
        prev = next; // prev에 next를 당겨서 다음 번 생성 때 중복된 노트 생성 방지

        float currentTime = AudioManager.Instance.GetMilliSec();
        float noteSpeed = Interval * 1000;
        
        foreach (Note note in reconNotes) // 생성 예정 노트를 생성
        {
            NoteObject noteObject = null;

            switch (note.type)
            {
                case (int)NoteType.Short: // 생성해야 할 노트가 Short인 경우
                    noteObject = PoolShort.Get();                       //PoolShort에서 Note하나 Get 
                    notes_queues[note.line - 1].Enqueue(noteObject);    //해당 line에 맞는 큐에 노트 객체 저장
                    //노트 생성 위치로 이동 (포지션은 노트 시간 - 현재 음악 시간 + 판정 Line의 y좌표 ) << 이렇게 설정하면 노트가 정해진 시간에 judgeLine에 도착하게 됨
                    noteObject.SetPosition(new Vector3[] { new Vector3(GenerateLocation[note.line - 1].position.x, (note.time - currentTime) * Interval + JudgeLine.position.y, 0f) });
                    break;
                case (int)NoteType.Long: // 생성해야 할 노트가 Long인 경우
                    noteObject = PoolLong.Get();                        //PoolLong에서 Note하나 Get
                    notes_queues[note.line - 1].Enqueue(noteObject);    //해당 line에 맞는 큐에 노트 객체 저장
                    //노트 생성 위치로 이동 (포지션은 노트 시간 - 현재 음악 시간 + 판정 Line의 y좌표 )
                    noteObject.SetPosition(new Vector3[]
                    {
                        new Vector3(GenerateLocation[note.line - 1].position.x, (note.time - currentTime) * Interval + JudgeLine.position.y, 0f),
                        new Vector3(GenerateLocation[note.line - 1].position.x, (note.tail - currentTime) * Interval + JudgeLine.position.y, 0f)
                    });
                    break;
                default:
                    break;
            }
            // 생성한 noteObject의 속성 값들을 설정
            noteObject.speed = noteSpeed;
            noteObject.note = note;
            noteObject.life = true;
            noteObject.gameObject.SetActive(true);
            noteObject.Move();              //Note 이동 시작
            toReleaseList.Add(noteObject);  //생성한 노트를 Release 리스트에 추가 < 추후에 Release에 사용
        }
    }

    //아직 미구현
    /* 
    public void DisposeNoteShort(NoteType type, Vector3 pos)
    {
        NoteObject noteObject = PoolShort.Get();
        noteObject.SetPosition(new Vector3[] { pos });
        noteObject.gameObject.SetActive(true);
        toReleaseList.Add(noteObject);
    }

    NoteObject noteObjectTemp;
    public void DisposeNoteLong(int makingCount, Vector3[] pos)
    {
        if (makingCount == 0)
        {
            noteObjectTemp = PoolLong.Get();
            noteObjectTemp.SetPosition(new Vector3[] { pos[0], pos[1] });
            noteObjectTemp.gameObject.SetActive(true);
        }
        else if (makingCount == 1)
        {
            noteObjectTemp.SetPosition(new Vector3[] { pos[0], pos[1] });
            toReleaseList.Add(noteObjectTemp);
        }
    }
    */

    //life가 false인 Note들만 Release
    void Release()
    {
        List<NoteObject> reconNotes = new List<NoteObject>();

        //Release를 진행할때 모든 노트들을 점검하면서 life가 false인 경우에만 Release를 해줌
        foreach (NoteObject note in toReleaseList)
        {
            if (!note.life) //Life가 false인 경우 -> Note가 특정 좌표 밑으로 내려간 경우 (NoteObject.cs 참고)
            {
                if (note is NoteShort)
                    PoolShort.Release(note as NoteShort);
                else
                    PoolLong.Release(note as NoteLong);

                note.gameObject.SetActive(false);
            }
            else
            {
                //life가 False가 아닌 Note의 경우 Release를 추후에 해줘야 하므로 reconNotes에 추가
                reconNotes.Add(note); 
            }
        }
        //Release 노트를 비운 후 아직 Release가 안 된 노트들을 다시 Release 리스트에 추가 
        //Release를 해줘도 노트들은 아직 Release List에 남아있음. 따라서, List를 비운 후에 삭제가 안된 Note들만 다시 Release 리스트에 추가해서 다음 Release가 진행될 수 있도록 해야함
        toReleaseList.Clear();
        toReleaseList.AddRange(reconNotes);
    }

    // 노트 위치 보간용 Interpolate
    public void Interpolate()
    {
        if (coInterpolate != null)
            StopCoroutine(coInterpolate);

        coInterpolate = StartCoroutine(IEInterpolate());
    }

    // 단위 시간(마디시간)마다 currentBar을 증가 시키기 위한 Timer
    IEnumerator IEGenTimer(float interval)
    {
        while (true)
        {
            //게임 state가 NonePlaying으로 변하면 노트생성 중지
            if (GameManager.Instance.state == GameManager.GameState.NoneGamePlaying)
            {
                break;
            }
            Gen();
            yield return new WaitForSeconds(interval);
            currentBar++;
        }
    }

    // 단위 시간(마디/2)마다 Release를 진행시켜주기 위한 Timer
    IEnumerator IEReleaseTimer(float interval)
    {
        while (true)
        {
            //게임 state가 NonePlaying으로 변하면 노트 Release 중지
            if (GameManager.Instance.state == GameManager.GameState.NoneGamePlaying)
            {
                break;
            }
            yield return new WaitForSeconds(interval);
            Release();
        }
    }

    //Note 위치 보간용 함수
    IEnumerator IEInterpolate(float rate = 1f, float duration = 1f)
    {
        float time = 0;
        Interval = defaultInterval * GameManager.Instance.Speed;
        float noteSpeed = Interval * 1000;
        // time이 duration을 넘어갈 동안만 지속 
        while (time < duration)
        {
            //게임 state가 NonePlaying으로 변하면 노트 위치 보간 중지
            if (GameManager.Instance.state == GameManager.GameState.NoneGamePlaying)
            {
                break;
            }
            float milli = AudioManager.Instance.GetMilliSec();

            //생성된 노트들을 대상으로 진행
            foreach (NoteObject note in toReleaseList)
            {
                //Note 위치, 속도를 다시 한 번 설정해줌으로써 위치를 보정해줌
                note.speed = noteSpeed;
                note.Interpolate(milli, Interval, JudgeLine.position.y); 
            }
            time += rate; 
            yield return new WaitForSeconds(rate);
        }
    }
}
