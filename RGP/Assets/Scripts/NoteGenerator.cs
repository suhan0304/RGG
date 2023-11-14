using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// ��Ʈ ���� �ڵ�
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

    //��Ʈ ���� ��ġ ������ ���� ��ü
    [SerializeField] Transform[] GenerateLocation = null;

    //���� ���� ��ġ ������ ���� ��ü
    [SerializeField] Transform JudgeLine = null;


    public GameObject parent;
    public GameObject notePrefab;
    public GameObject LinePrefab;

    //�⺻ interval, interval ������ ��Ʈ�� ����
    readonly float defaultInterval = 0.5f; // 1��� ������ (1���� ��ü�� ȭ�鿡 �׷����� ������ ����)
    public float Interval { get; private set; }

    //Object Pool - Short Note�� ����
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
    //Object Pool (short) : Short Note ����
    NoteShort CreatePooledShort()
    {
        GameObject note = Instantiate(notePrefab);          //Short Note - Instantiate�� �̿��� notePrefab�� �����ؼ� ����
        note.transform.SetParent(parent.transform, false);  //������ ��Ʈ Parent ���� : Parent ��ü�� Notes(Empty Object)�� �����Ǿ� ����

        note.AddComponent<NoteShort>();                     //������ Note Object �� NoteShort Component �߰�
        return note.GetComponent<NoteShort>();              //���� �Ϸ�� Note Object�� ��ȯ 
    }

    //Object Pool - Long Note�� ����
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
    //Object Pool (Long) : Long Note ����
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

    
    int currentBar = 3; // ���� �÷��� �� 3���� ���� ����
    int next = 0;
    int prev = 0;

    //������ Note ���� ��� List
    public List<NoteObject> toReleaseList = new List<NoteObject>();

    Coroutine coGenTimer;
    Coroutine coReleaseTimer;
    Coroutine coInterpolate;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }


    //��Ʈ �ı�
    public List<Queue<NoteObject>> notes_queues = new List<Queue<NoteObject>>();

    [SerializeField] public Queue<NoteObject> notes_queue1 = new Queue<NoteObject>();
    [SerializeField] public Queue<NoteObject> notes_queue2 = new Queue<NoteObject>();
    [SerializeField] public Queue<NoteObject> notes_queue3 = new Queue<NoteObject>();
    [SerializeField] public Queue<NoteObject> notes_queue4 = new Queue<NoteObject>();

    //���κ� ��Ʈ���� ��� Queue�� �� �������� ���۸��� �ʱ�ȭ
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

    //������ �Ϸ�� ��Ʈ�� �ٷ� Object Pool�� Release�� ����
    public void FallNoteDequeue(int line)
    {
        NoteObject note = notes_queues[line].Dequeue();
    }

    //������ �Ϸ�� ��Ʈ�� �ٷ� Object Pool�� Release�� ����
    public void judgedNoteRelease(int line)
    {
        NoteObject note = notes_queues[line].Dequeue();

        if (note is NoteShort)
            PoolShort.Release(note as NoteShort);
        else
            PoolLong.Release(note as NoteLong);

        //Release �� ��Ȱ��ȭ
        note.gameObject.SetActive(false);
    }

    // Ǯ�� ��� ���� (���� �÷��� �� ���)
    public void StartGen()
    {
        noteQueueInit();                                            // ��Ʈ ���� �� ���� ��Ʈ �ı��� ť �ʱ�ȭ
        Interval = defaultInterval * GameManager.Instance.Speed;    // interver = �⺻ interval * �ӵ� 
        coGenTimer = StartCoroutine(IEGenTimer(GameManager.Instance.sheets[GameManager.Instance.title].BarPerMilliSec * 0.001f)); // ������ 1���� �ð�(BarPerMilliSec)���� ������ ��Ʈ ������Ʈ Ž�� 
        coReleaseTimer = StartCoroutine(IEReleaseTimer(GameManager.Instance.sheets[GameManager.Instance.title].BarPerMilliSec * 0.001f * 0.5f)); // 1���� �ð�(BarPerMilliSec)�� ���� �ֱ�� ������ ��Ʈ ������Ʈ Ž��
        coInterpolate = StartCoroutine(IEInterpolate(0.1f, 4f));    //��Ʈ���� ��ġ�� �������ִ� Interpolate Coroutine ����
    }

    void Gen()
    {

        List<Note> notes = GameManager.Instance.sheets[GameManager.Instance.title].notes;   //title�� �´� sheet �������� ��Ʈ���� ������ ������
        List<Note> reconNotes = new List<Note>();                                           


        // interval �ð� ������ ���� ������ ���� �����ؾ��� ��Ʈ Ȯ��
        for (; next < notes.Count; next++)
        {
            // currentBar�� �ʱⰪ = 3 : CurrentBar�� GenTimer���� ���������� BarPerMilliSec�� ���� ������ 1�� ���� ������
            // currentBar * BarPerMilliSec �� ���� 3���� ����Ǵ� �ð� : ��Ʈ�� ���� �ð��� �տ��� ���� �ð����� Ŀ�������� next�� ����
            // �� ������ ���� ������ �� ����(Bar)���� ������ �� ��Ʈ���� ��ȣ�� ã�� �� ���� ( Note[next]������ ���� )
            if (notes[next].time > currentBar * GameManager.Instance.sheets[GameManager.Instance.title].BarPerMilliSec)
            {
                break;
            }
        }

        // ���� ���� ��Ʈ(note[next]������)�� reconNotes�ȿ� �߰�
        for (int j = prev; j < next; j++)
        {
            reconNotes.Add(notes[j]);
        }
        prev = next; // prev�� next�� ��ܼ� ���� �� ���� �� �ߺ��� ��Ʈ ���� ����

        float currentTime = AudioManager.Instance.GetMilliSec();
        float noteSpeed = Interval * 1000;
        
        foreach (Note note in reconNotes) // ���� ���� ��Ʈ�� ����
        {
            NoteObject noteObject = null;

            switch (note.type)
            {
                case (int)NoteType.Short: // �����ؾ� �� ��Ʈ�� Short�� ���
                    noteObject = PoolShort.Get();                       //PoolShort���� Note�ϳ� Get 
                    notes_queues[note.line - 1].Enqueue(noteObject);    //�ش� line�� �´� ť�� ��Ʈ ��ü ����
                    //��Ʈ ���� ��ġ�� �̵� (�������� ��Ʈ �ð� - ���� ���� �ð� + ���� Line�� y��ǥ ) << �̷��� �����ϸ� ��Ʈ�� ������ �ð��� judgeLine�� �����ϰ� ��
                    noteObject.SetPosition(new Vector3[] { new Vector3(GenerateLocation[note.line - 1].position.x, (note.time - currentTime) * Interval + JudgeLine.position.y, 0f) });
                    break;
                case (int)NoteType.Long: // �����ؾ� �� ��Ʈ�� Long�� ���
                    noteObject = PoolLong.Get();                        //PoolLong���� Note�ϳ� Get
                    notes_queues[note.line - 1].Enqueue(noteObject);    //�ش� line�� �´� ť�� ��Ʈ ��ü ����
                    //��Ʈ ���� ��ġ�� �̵� (�������� ��Ʈ �ð� - ���� ���� �ð� + ���� Line�� y��ǥ )
                    noteObject.SetPosition(new Vector3[]
                    {
                        new Vector3(GenerateLocation[note.line - 1].position.x, (note.time - currentTime) * Interval + JudgeLine.position.y, 0f),
                        new Vector3(GenerateLocation[note.line - 1].position.x, (note.tail - currentTime) * Interval + JudgeLine.position.y, 0f)
                    });
                    break;
                default:
                    break;
            }
            // ������ noteObject�� �Ӽ� ������ ����
            noteObject.speed = noteSpeed;
            noteObject.note = note;
            noteObject.life = true;
            noteObject.gameObject.SetActive(true);
            noteObject.Move();              //Note �̵� ����
            toReleaseList.Add(noteObject);  //������ ��Ʈ�� Release ����Ʈ�� �߰� < ���Ŀ� Release�� ���
        }
    }

    //���� �̱���
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

    //life�� false�� Note�鸸 Release
    void Release()
    {
        List<NoteObject> reconNotes = new List<NoteObject>();

        //Release�� �����Ҷ� ��� ��Ʈ���� �����ϸ鼭 life�� false�� ��쿡�� Release�� ����
        foreach (NoteObject note in toReleaseList)
        {
            if (!note.life) //Life�� false�� ��� -> Note�� Ư�� ��ǥ ������ ������ ��� (NoteObject.cs ����)
            {
                if (note is NoteShort)
                    PoolShort.Release(note as NoteShort);
                else
                    PoolLong.Release(note as NoteLong);

                note.gameObject.SetActive(false);
            }
            else
            {
                //life�� False�� �ƴ� Note�� ��� Release�� ���Ŀ� ����� �ϹǷ� reconNotes�� �߰�
                reconNotes.Add(note); 
            }
        }
        //Release ��Ʈ�� ��� �� ���� Release�� �� �� ��Ʈ���� �ٽ� Release ����Ʈ�� �߰� 
        //Release�� ���൵ ��Ʈ���� ���� Release List�� ��������. ����, List�� ��� �Ŀ� ������ �ȵ� Note�鸸 �ٽ� Release ����Ʈ�� �߰��ؼ� ���� Release�� ����� �� �ֵ��� �ؾ���
        toReleaseList.Clear();
        toReleaseList.AddRange(reconNotes);
    }

    // ��Ʈ ��ġ ������ Interpolate
    public void Interpolate()
    {
        if (coInterpolate != null)
            StopCoroutine(coInterpolate);

        coInterpolate = StartCoroutine(IEInterpolate());
    }

    // ���� �ð�(����ð�)���� currentBar�� ���� ��Ű�� ���� Timer
    IEnumerator IEGenTimer(float interval)
    {
        while (true)
        {
            //���� state�� NonePlaying���� ���ϸ� ��Ʈ���� ����
            if (GameManager.Instance.state == GameManager.GameState.NoneGamePlaying)
            {
                break;
            }
            Gen();
            yield return new WaitForSeconds(interval);
            currentBar++;
        }
    }

    // ���� �ð�(����/2)���� Release�� ��������ֱ� ���� Timer
    IEnumerator IEReleaseTimer(float interval)
    {
        while (true)
        {
            //���� state�� NonePlaying���� ���ϸ� ��Ʈ Release ����
            if (GameManager.Instance.state == GameManager.GameState.NoneGamePlaying)
            {
                break;
            }
            yield return new WaitForSeconds(interval);
            Release();
        }
    }

    //Note ��ġ ������ �Լ�
    IEnumerator IEInterpolate(float rate = 1f, float duration = 1f)
    {
        float time = 0;
        Interval = defaultInterval * GameManager.Instance.Speed;
        float noteSpeed = Interval * 1000;
        // time�� duration�� �Ѿ ���ȸ� ���� 
        while (time < duration)
        {
            //���� state�� NonePlaying���� ���ϸ� ��Ʈ ��ġ ���� ����
            if (GameManager.Instance.state == GameManager.GameState.NoneGamePlaying)
            {
                break;
            }
            float milli = AudioManager.Instance.GetMilliSec();

            //������ ��Ʈ���� ������� ����
            foreach (NoteObject note in toReleaseList)
            {
                //Note ��ġ, �ӵ��� �ٽ� �� �� �����������ν� ��ġ�� ��������
                note.speed = noteSpeed;
                note.Interpolate(milli, Interval, JudgeLine.position.y); 
            }
            time += rate; 
            yield return new WaitForSeconds(rate);
        }
    }
}
