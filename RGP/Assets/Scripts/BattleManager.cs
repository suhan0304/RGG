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

    int currentPlayerHealth;    // ���� �÷��̾� HP
    int currentMonsterHealth;   // ���� ���� HP

    int damage; // ���� ������

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

    // ������ ������ ���
    int Calculate(JudgeType judge)
    {
        int amount = 0;

        switch (judge)
        {
            case JudgeType.max100: amount = 100; break; // 20 ~ 100�� �÷��̾ ���Ϳ��� ������ ������
            case JudgeType.max90: amount = 90; break;
            case JudgeType.max80: amount = 80; break;
            case JudgeType.max70: amount = 70; break;
            case JudgeType.max60: amount = 60; break;
            case JudgeType.max50: amount = 50; break;
            case JudgeType.max40: amount = 40; break;
            case JudgeType.max30: amount = 30; break;
            case JudgeType.max20: amount = 20; break;
            case JudgeType.max10: amount = 80; break;   // maxbreak�� 10�� ���Ͱ� �÷��̾�� ������ ������
            default: amount = 100; break;
        }

        return amount;
    }

    // �÷��̾� �Ǵ� ������ ���� �Լ�
    public void Attack(JudgeType judge)
    {
        damage = Calculate(judge);  // ������ ���
        if (judge == JudgeType.maxbreak || judge == JudgeType.max10)    // ���Ͱ� ������ ���
        {
            currentPlayerHealth -= damage;
            playerHealth.value = currentPlayerHealth;   // �÷��̾� HP �����̴� ������Ʈ
            
            if (playerHealth.value <= 0)    // �÷��̾��� ü���� 0�� �Ǿ��� ���
            {
                Debug.Log("Player is defeated!");
            }
        }
        else    // �÷��̾ ������ ���
        {
            currentMonsterHealth -= damage;
            monsterHealth.value = currentMonsterHealth; // ���� HP �����̴� ������Ʈ

            if (monsterHealth.value <= 0)   // ������ ü���� 0�� �Ǿ��� ���
            {
                Debug.Log("Monster is defeated");
            }
        }
    }
}
