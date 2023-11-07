using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class BattleManager : MonoBehaviour
{
    public Slider playerHealth;     // �÷��̾� HP �����̴� (name: PlayerHealthBar)
    public Slider monsterHealth;    // ���� HP �����̴� (name: MonsterHealthBar)

    public int playerHealthAmount;  // �÷��̾� �ִ� HP
    public int monsterHealthAmount; // ���� �ִ� HP

    public int currentPlayerHealth;    // ���� �÷��̾� HP
    public int currentMonsterHealth;   // ���� ���� HP



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

    // �÷��̾��� ü��, ������ ü�� �� ����
    public void Init()
    {
        // �ӽ� �ʱ�ȭ
        playerHealthAmount = 5000;
        currentPlayerHealth = playerHealthAmount;
        playerHealth.maxValue = playerHealthAmount; // �÷��̾� �����̴� ����
        playerHealth.value = playerHealthAmount;

        monsterHealthAmount = 5000;
        currentMonsterHealth = monsterHealthAmount;
        monsterHealth.maxValue = monsterHealthAmount;   // ���� �����̴� ����
        monsterHealth.value = monsterHealthAmount;
    }

   

    
}
