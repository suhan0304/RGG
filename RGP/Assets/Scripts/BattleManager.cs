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

    int currentPlayerHealth;    // 현재 플레이어 HP
    int currentMonsterHealth;   // 현재 몬스터 HP

    int damage; // 입힌 데미지

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

    // 플레이어의 체력, 몬스터의 체력 등 설정
    public void Init()
    {
        // 임시 초기화
        playerHealthAmount = 5000;
        currentPlayerHealth = playerHealthAmount;
        playerHealth.maxValue = playerHealthAmount; // 플레이어 슬라이더 설정
        playerHealth.value = playerHealthAmount;

        monsterHealthAmount = 5000;
        currentMonsterHealth = monsterHealthAmount;
        monsterHealth.maxValue = monsterHealthAmount;   // 몬스터 슬라이더 설정
        monsterHealth.value = monsterHealthAmount;
        
    }

    // 판정별 데미지 계산
    int Calculate(JudgeType judge)
    {
        int amount = 0;

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

        return amount;
    }

    // 플레이어 또는 몬스터의 공격 함수
    public void Attack(JudgeType judge)
    {
        damage = Calculate(judge);  // 데미지 계산
        if (judge == JudgeType.maxbreak || judge == JudgeType.max10)    // 몬스터가 공격할 경우
        {
            currentPlayerHealth -= damage;
            playerHealth.value = currentPlayerHealth;   // 플레이어 HP 슬라이더 업데이트
            
            if (playerHealth.value <= 0)    // 플레이어의 체력이 0이 되었을 경우
            {
                Debug.Log("Player is defeated!");
            }
        }
        else    // 플레이어가 공격할 경우
        {
            currentMonsterHealth -= damage;
            monsterHealth.value = currentMonsterHealth; // 몬스터 HP 슬라이더 업데이트

            if (monsterHealth.value <= 0)   // 몬스터의 체력이 0이 되었을 경우
            {
                Debug.Log("Monster is defeated");
            }
        }
    }
}
