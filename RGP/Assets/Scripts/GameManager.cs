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

    /// 게임 진행 상태. InputManager.OnEnter() 참고
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
        //플레이어 체력이 0이 되서 게임 오버
        state = GameState.NoneGamePlaying; //게임 상태를 NonePlaying으로 변경
        AudioManager.Instance.Stop(); //노래 중지
        NoteGenerator.Instance.ReleaseCompleted();//모든 노트들 Release 실행
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
        // 새 게임을 시작할 수 없게 해줌
        isPlaying = true;
        // Sheet 초기화
        title = "Consolation";
        sheets[title].Init();
        Debug.Log("sheet 초기화 완료");
        // Audio 삽입
        AudioManager.Instance.Insert(sheets[title].clip);
        Debug.Log("Audio 삽입 완료");
        // 판정 초기화
        FindObjectOfType<Judgement>().Init();
        // 점수 초기화
        Score.Instance.Clear();
        // Note 생성
        NoteGenerator.Instance.StartGen();
        Debug.Log("노트 생성 완료");
        // 3초 대기
        yield return new WaitForSeconds(3f);
        // Audio 재생
        AudioManager.Instance.progressTime = 0f;
        AudioManager.Instance.Play();
    }
}
