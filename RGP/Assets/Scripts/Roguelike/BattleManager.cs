using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class BattleManager : MonoBehaviour
{
    public Slider playerHealth;     // 플레이어 HP 슬라이더 (name: PlayerHealthBar)
    public Slider monsterHealth;    // 몬스터 HP 슬라이더 (name: MonsterHealthBar)

    public int playerHealthAmount;  // 플레이어 최대 HP
    public int monsterHealthAmount; // 몬스터 최대 HP

    public int currentPlayerHealth;    // 현재 플레이어 HP
    public int currentMonsterHealth;   // 현재 몬스터 HP

    public bool isClear = false;    // 스테이지 클리어 여부
    public Coroutine coUpdateHealth = null; // 체력 슬라이더 업데이트 위한 코루틴
    static BattleManager instance;

    public static BattleManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    
    private void Update()
    {
        // 노래가 끝났을 때, 플레이어가 살아있다면 클리어 판정으로 넘어가기
        if (isClear && !AudioManager.Instance.IsPlaying())  
        {
            isClear = false;
            ClearStage();
        }
    }

    // 플레이어의 체력, 몬스터의 체력 등 설정
    public void Init()
    {
        // 임시 초기화
        playerHealthAmount = 3000;  //임시로 3000으로 설정
        currentPlayerHealth = playerHealthAmount;
        playerHealth.maxValue = playerHealthAmount; // 플레이어 슬라이더 설정
        playerHealth.value = playerHealthAmount;

        monsterHealthAmount = 50000;
        currentMonsterHealth = monsterHealthAmount;
        monsterHealth.maxValue = monsterHealthAmount;   // 몬스터 슬라이더 설정
        monsterHealth.value = monsterHealthAmount;
    }

    // 플레이어의 클리어 여부 판정
    public void ClearStage()
    {
        
        if (currentMonsterHealth > 0)   // 몬스터의 체력이 남아있다면
        {
            if (coUpdateHealth == null)
            {
                coUpdateHealth = StartCoroutine(ClearUpdateHealth());
            }
        }
        else                            // 몬스터의 체력이 0이면
        {
            ItemSelection.Instance.ShowItemSelectionPanel();
            
        }
    }

    // 노래 끝난 후 플레이어와 몬스터의 체력 업데이트
    IEnumerator ClearUpdateHealth()
    {
        float remainedPlayerHealth = (float) currentPlayerHealth - (float) playerHealthAmount * ((float)currentMonsterHealth / (float) monsterHealthAmount);    // 남은 플레이어의 체력
        float duration = 5f;    // 감소하는 시간 설정
        float elapsedTime = 0f; // 경과한 시간

        // 설정한 시간만큼 반복
        while (elapsedTime < duration)
        {
            monsterHealth.value = Mathf.Lerp(currentMonsterHealth, 0, elapsedTime / duration);  // 몬스터 체력 슬라이더 duration 동안 서서히 감소시키기
            playerHealth.value = Mathf.Lerp(currentPlayerHealth, remainedPlayerHealth, elapsedTime / duration); // 플레이어의 체력 슬라이더 duration 동안 서서히 감소시키기
            elapsedTime += Time.deltaTime;  // 현재 시간만큼 증가 (프레임 영향)
            yield return null;
        }
        // 몬스터와 플레이어 현재 체력 업데이트
        currentMonsterHealth = 0;
        currentPlayerHealth = (int) remainedPlayerHealth;

        if (currentPlayerHealth <= 0)   // 현재 플레이어 체력이 0이 되면 게임 오버
        {
            isClear = false;
            GameManager.Instance.GameOver();    // 게임 오버 화면으로 이동
        }
        else                            // 현재 플레이어 체력이 0 초과면 생존
        {
            ItemSelection.Instance.ShowItemSelectionPanel();
            //GameManager.Instance.NextStage();   // 다음 스테이지 선택 화면으로 이동
        }
    }
    public void StopCoUpdateHealth()
    {
        // 코루틴 중지
        if (coUpdateHealth != null)
        {
            StopCoroutine(coUpdateHealth);
            Debug.Log("코루틴 중지");
        }
        coUpdateHealth = null;
    }

}
