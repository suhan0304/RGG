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



    static BattleManager instance;
    int damage;

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

   

    
}
