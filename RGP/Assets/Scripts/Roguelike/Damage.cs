using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Damage : MonoBehaviour
{
    public UIText[] textPlayerList = new UIText[5]; // 플레이어가 받는 데미지 텍스트 목록
    public UIText[] textMonsterList = new UIText[5];// 몬스터가 받는 데미지 텍스트 목록

    int damage; // 입힌 데미지
    static int p_i; // 플레이어가 받는 데미지 텍스트의 인덱스 접근자
    static int m_i; // 몬스터가 받는 데미지 텍스트의 인덱스 접근자

    // 임시 변수 플레이어와 몬스터의 공격력
    int playerPower;
    int monsterPower;

    static Damage instance;

    public static Damage Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        
    }

    public void Start()
    {
        // 처음은 0으로 초기화
        p_i = 0;    
        m_i = 0;

        //임시 초기화
        playerPower = 20;
        monsterPower = 10;
    }

    // 판정별 데미지 계산
    public int CalculateDamage(JudgeType judge, int power)
    {
        int amount;

        switch (judge)
        {
            case JudgeType.max100: amount = 100; break; // 20 ~ 100은 플레이어가 몬스터에게 입히는 데미지
            case JudgeType.max90: amount = 90; break;
            case JudgeType.max80: amount = 80; break;
            case JudgeType.max70: amount = 70; break;
            case JudgeType.max60: amount = 60; break;
            case JudgeType.max50: amount = 50; break;
            case JudgeType.max40: amount = 40; break;
            case JudgeType.max30: amount = 30; break;
            case JudgeType.max20: amount = 20; break;
            case JudgeType.max10: amount = 80; break;   // maxbreak와 10은 몬스터가 플레이어에게 입히는 데미지
            default: amount = 100; break;
        }
        // 현재 데미지 공식: (판정별 수치 + 공격력)
        amount += power;

        return amount;
    }

    // 플레이어 또는 몬스터의 공격 함수
    public void Attack(JudgeType judge)
    {
        if (judge == JudgeType.maxbreak || judge == JudgeType.max10)    // 몬스터가 공격할 경우
        {
            damage = Damage.Instance.CalculateDamage(judge, monsterPower);   // 몬스터가 입힐 데미지
            BattleManager.Instance.currentPlayerHealth -= damage;   // 현재 플레이어 HP 감소

            if (BattleManager.Instance.currentPlayerHealth <= 0) {
                GameManager.Instance.GameOver();
            }

            BattleManager.Instance.playerHealth.value = BattleManager.Instance.currentPlayerHealth;   // 플레이어 HP 슬라이더 업데이트

            textPlayerList[p_i].SetText(damage.ToString());     // 데미지 텍스트에 값 업데이트
            
            EffectManager.Instance.playerDamagedEffect(p_i++);  // 데미지 텍스트 띄우기
            EffectManager.Instance.monsterAttackEffect();       // 몬스터의 공격 모션 작동

            // 데미지 텍스트 총 5개, 0~4 순차적으로 데미지 텍스트 띄움. 4번이 작동하면 다시 0번부터 작동
            if (p_i == 4)
                p_i = 0;

            if (BattleManager.Instance.playerHealth.value <= 0)    // 플레이어의 체력이 0이 되었을 경우
            {
                Debug.Log("Player is defeated!");
            }
        }
        else    // 플레이어가 공격할 경우
        {
            damage = Damage.Instance.CalculateDamage(judge, playerPower);   // 플레이어가 입힐 데미지
            BattleManager.Instance.currentMonsterHealth -= damage;  // 현재 몬스터 HP 감소
            BattleManager.Instance.monsterHealth.value = BattleManager.Instance.currentMonsterHealth; // 몬스터 HP 슬라이더 업데이트

            textMonsterList[m_i].SetText(damage.ToString());    // 데미지 텍스트에 값 업데이트
             
            EffectManager.Instance.monsterDamagedEffect(m_i++); // 데미지 텍스트 띄우기
            EffectManager.Instance.playerAttackEffect();        // 플레이어의 공격 모션 작동
            
            // 데미지 텍스트 총 5개, 0~4 순차적으로 데미지 텍스트 띄움. 4번이 작동하면 다시 0번부터 작동
            if (m_i == 4)
                m_i = 0;

            if (BattleManager.Instance.monsterHealth.value <= 0)   // 몬스터의 체력이 0이 되었을 경우
            {
                Debug.Log("Monster is defeated");
            }
        }
    }
}
