using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int resolution_X = 1920;
    public int resolution_Y = 1080;

    static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }
    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public enum GameState
    {
        GamePlaying,
        NoneGamePlaying
    }
    public GameState state = GameState.GamePlaying;

    /// ���� ���� ����. InputManager.OnEnter() ����
    public bool isPlaying = true;
    public string title;
    Coroutine coPlaying;

    public Dictionary<string, Sheet> sheets = new Dictionary<string, Sheet>();

    float speed = 1.0f;
    public float Speed
    {
        get
        {
            return speed;
        }
        set
        {
            speed = Mathf.Clamp(value, 1.0f, 5.0f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(IEInit());
    }

    // Update is called once per frame
    void Update()
    {

    }




    public void Play()
    {
        StartCoroutine(IEInitPlay());
    }

    public void GameOver()
    {
        //�÷��̾� ü���� 0�� �Ǽ� ���� ����
        state = GameState.NoneGamePlaying; //���� ���¸� NonePlaying���� ����
        AudioManager.Instance.Stop(); //�뷡 ����
        NoteGenerator.Instance.ReleaseCompleted();//��� ��Ʈ�� Release ����
    }

    IEnumerator IEInit()
    {
        SheetLoader.Instance.Init();

        UIController.Instance.Init();
        Score.Instance.Init();

        BattleManager.Instance.Init();

        yield return new WaitUntil(() => SheetLoader.Instance.bLoadFinish == true);
        

        Play();
    }

    IEnumerator IEInitPlay()
    {
        // �� ������ ������ �� ���� ����
        isPlaying = true;
        // Sheet �ʱ�ȭ
        title = "Consolation";
        sheets[title].Init();
        Debug.Log("sheet �ʱ�ȭ �Ϸ�");
        // Audio ����
        AudioManager.Instance.Insert(sheets[title].clip);
        Debug.Log("Audio ���� �Ϸ�");
        // ���� �ʱ�ȭ
        FindObjectOfType<Judgement>().Init();
        // ���� �ʱ�ȭ
        Score.Instance.Clear();
        // Note ����
        NoteGenerator.Instance.StartGen();
        Debug.Log("��Ʈ ���� �Ϸ�");
        // 3�� ���
        yield return new WaitForSeconds(3f);
        // Audio ���
        AudioManager.Instance.progressTime = 0f;
        AudioManager.Instance.Play();
    }
}
