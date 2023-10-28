using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

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

    [SerializeField] Transform[] GenerateLocation = null;
    [SerializeField] Transform JudgeLine = null;
    public GameObject parent;
    public GameObject notePrefab;
    public Material lineRendererMaterial;

    readonly float defaultInterval = 0.5f; // 1배속 기준점 (1마디 전체가 화면에 그려지는 정도를 정의)
    public float Interval { get; private set; }

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
    NoteShort CreatePooledShort()
    {
        GameObject note = Instantiate(notePrefab);
        note.transform.SetParent(parent.transform, false);

        note.AddComponent<NoteShort>();
        return note.GetComponent<NoteShort>();
    }

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
    NoteLong CreatePooledLong()
    {
        GameObject note = new GameObject("NoteLong");
        note.transform.SetParent(parent.transform, false);


        GameObject head = Instantiate(notePrefab);
        head.name = "head";
        head.transform.SetParent(note.transform, false);

        GameObject tail = Instantiate(notePrefab);
        tail.transform.SetParent(note.transform, false);
        

        GameObject line = new GameObject("line");
        line.transform.SetParent(note.transform, false);

        line.AddComponent<LineRenderer>();
        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.material = lineRendererMaterial;
        lineRenderer.sortingOrder = 3;
        lineRenderer.widthMultiplier = 0.8f;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = false;

        note.AddComponent<NoteLong>();
        return note.GetComponent<NoteLong>();
    }

    int currentBar = 3; // 최초 플레이 시 3마디 먼저 생성
    int next = 0;
    int prev = 0;
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

    public void judgedNoteRelease(int line)
    {
        NoteObject note = notes_queues[line].Dequeue();

        if (note is NoteShort)
            PoolShort.Release(note as NoteShort);
        else
            PoolLong.Release(note as NoteLong);

        note.gameObject.SetActive(false);
    }

    // 풀링 기반 생성 (게임 플레이 시 사용)
    public void StartGen()
    {
        noteQueueInit();
        Interval = defaultInterval * GameManager.Instance.Speed;
        coGenTimer = StartCoroutine(IEGenTimer(GameManager.Instance.sheets[GameManager.Instance.title].BarPerMilliSec * 0.001f)); // 음악의 1마디 시간마다 생성할 노트 오브젝트 탐색
        coReleaseTimer = StartCoroutine(IEReleaseTimer(GameManager.Instance.sheets[GameManager.Instance.title].BarPerMilliSec * 0.001f * 0.5f)); // 1마디 시간의 절반 주기로 해제할 노트 오브젝트 탐색
        coInterpolate = StartCoroutine(IEInterpolate(0.1f, 4f));
    }

    void Gen()
    {

        List<Note> notes = GameManager.Instance.sheets[GameManager.Instance.title].notes;
        List<Note> reconNotes = new List<Note>();

        for (; next < notes.Count; next++)
        {
            if (notes[next].time > currentBar * GameManager.Instance.sheets[GameManager.Instance.title].BarPerMilliSec)
            {
                break;
            }
        }
        for (int j = prev; j < next; j++)
        {
            reconNotes.Add(notes[j]);
        }
        prev = next;

        float currentTime = AudioManager.Instance.GetMilliSec();
        float noteSpeed = Interval * 1000;
        foreach (Note note in reconNotes)
        {
            NoteObject noteObject = null;

            switch (note.type)
            {
                case (int)NoteType.Short:
                    noteObject = PoolShort.Get();
                    notes_queues[note.line - 1].Enqueue(noteObject);
                    noteObject.SetPosition(new Vector3[] { new Vector3(GenerateLocation[note.line - 1].position.x, (note.time - currentTime) * Interval + JudgeLine.position.y, 0f) });
                    break;
                case (int)NoteType.Long:
                    noteObject = PoolLong.Get();
                    notes_queues[note.line - 1].Enqueue(noteObject);
                    noteObject.SetPosition(new Vector3[] // 포지션은 노트 시간 - 현재 음악 시간
                    {
                        new Vector3(GenerateLocation[note.line - 1].position.x, (note.time - currentTime) * Interval + JudgeLine.position.y, 0f),
                        new Vector3(GenerateLocation[note.line - 1].position.x, (note.tail - currentTime) * Interval + JudgeLine.position.y, 0f)
                    });
                    break;
                default:
                    break;
            }
            noteObject.speed = noteSpeed;
            noteObject.note = note;
            noteObject.life = true;
            noteObject.gameObject.SetActive(true);
            noteObject.Move();
            toReleaseList.Add(noteObject);
        }
    }

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

    void ReleaseCompleted()
    {
        foreach (NoteObject note in toReleaseList)
        {
            note.gameObject.SetActive(false);

            if (note is NoteShort)
                PoolShort.Release(note as NoteShort);
            else
                PoolLong.Release(note as NoteLong);
        }
    }

    void Release()
    {
        List<NoteObject> reconNotes = new List<NoteObject>();
        foreach (NoteObject note in toReleaseList)
        {
            if (!note.life)
            {
                if (note is NoteShort)
                    PoolShort.Release(note as NoteShort);
                else
                    PoolLong.Release(note as NoteLong);

                note.gameObject.SetActive(false);
            }
            else
            {
                reconNotes.Add(note);
            }
        }
        toReleaseList.Clear();
        toReleaseList.AddRange(reconNotes);
    }

    public void Interpolate()
    {
        if (coInterpolate != null)
            StopCoroutine(coInterpolate);

        coInterpolate = StartCoroutine(IEInterpolate());
    }

    IEnumerator IEGenTimer(float interval)
    {
        while (true)
        {
            Gen();
            yield return new WaitForSeconds(interval);
            currentBar++;
        }
    }

    IEnumerator IEReleaseTimer(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            Release();
        }
    }

    IEnumerator IEInterpolate(float rate = 1f, float duration = 1f)
    {
        float time = 0;
        Interval = defaultInterval * GameManager.Instance.Speed;
        float noteSpeed = Interval * 1000;
        while (time < duration)
        {
            float milli = AudioManager.Instance.GetMilliSec();

            foreach (NoteObject note in toReleaseList)
            {
                note.speed = noteSpeed;
                note.Interpolate(milli, Interval, JudgeLine.position.y);
            }
            time += rate;
            yield return new WaitForSeconds(rate);
        }
    }
}
