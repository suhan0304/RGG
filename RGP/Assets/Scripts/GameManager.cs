using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ItemSelection itemSelectionScript; // ItemSelection 스크립트에 대한 참조

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
        itemSelectionScript = FindObjectOfType<ItemSelection>();
    }

    public enum GameState
    {
        Game
    }
    public GameState state = GameState.Game;

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

    void Start()
    {
        StartCoroutine(IEInit());
    }

    void Update()
    {

    }

    public void Play()
    {
        StartCoroutine(IEInitPlay());
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
        isPlaying = true;
        title = "Consolation";

        sheets[title].Init();
        Debug.Log("Sheet 초기화 완료");

        AudioManager.Instance.Insert(sheets[title].clip);
        Debug.Log("Audio 초기화 완료");

        FindObjectOfType<Judgement>().Init();

        Score.Instance.Clear();

        NoteGenerator.Instance.StartGen();
        Debug.Log("Note 생성 시작");

        yield return new WaitForSeconds(3f);
        AudioManager.Instance.progressTime = 0f;
        AudioManager.Instance.Play();

        while (AudioManager.Instance.IsPlaying())
        {
            yield return null;
        }

        Debug.Log("노래가 끝났습니다.");

        // 노래가 끝나면 UI 패널을 활성화
        itemSelectionScript.ShowItemSelectionPanel();

    }
}
