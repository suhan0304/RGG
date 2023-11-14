using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Damage : MonoBehaviour
{
    public UIText[] textPlayerList = new UIText[5]; // �÷��̾ �޴� ������ �ؽ�Ʈ ���
    public UIText[] textMonsterList = new UIText[5];// ���Ͱ� �޴� ������ �ؽ�Ʈ ���

    int damage; // ���� ������
    static int p_i; // �÷��̾ �޴� ������ �ؽ�Ʈ�� �ε��� ������
    static int m_i; // ���Ͱ� �޴� ������ �ؽ�Ʈ�� �ε��� ������

    // �ӽ� ���� �÷��̾�� ������ ���ݷ�
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
        // ó���� 0���� �ʱ�ȭ
        p_i = 0;    
        m_i = 0;

        //�ӽ� �ʱ�ȭ
        playerPower = 20;
        monsterPower = 10;
    }

    // ������ ������ ���
    public int CalculateDamage(JudgeType judge, int power)
    {
        int amount;

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
        // ���� ������ ����: (������ ��ġ + ���ݷ�)
        amount += power;

        return amount;
    }

    // �÷��̾� �Ǵ� ������ ���� �Լ�
    public void Attack(JudgeType judge)
    {
        if (judge == JudgeType.maxbreak || judge == JudgeType.max10)    // ���Ͱ� ������ ���
        {
            damage = Damage.Instance.CalculateDamage(judge, monsterPower);   // ���Ͱ� ���� ������
            BattleManager.Instance.currentPlayerHealth -= damage;   // ���� �÷��̾� HP ����

            if (BattleManager.Instance.currentPlayerHealth <= 0) {
                GameManager.Instance.GameOver();
            }

            BattleManager.Instance.playerHealth.value = BattleManager.Instance.currentPlayerHealth;   // �÷��̾� HP �����̴� ������Ʈ

            textPlayerList[p_i].SetText(damage.ToString());     // ������ �ؽ�Ʈ�� �� ������Ʈ
            
            EffectManager.Instance.playerDamagedEffect(p_i++);  // ������ �ؽ�Ʈ ����
            EffectManager.Instance.monsterAttackEffect();       // ������ ���� ��� �۵�

            // ������ �ؽ�Ʈ �� 5��, 0~4 ���������� ������ �ؽ�Ʈ ���. 4���� �۵��ϸ� �ٽ� 0������ �۵�
            if (p_i == 4)
                p_i = 0;

            if (BattleManager.Instance.playerHealth.value <= 0)    // �÷��̾��� ü���� 0�� �Ǿ��� ���
            {
                Debug.Log("Player is defeated!");
            }
        }
        else    // �÷��̾ ������ ���
        {
            damage = Damage.Instance.CalculateDamage(judge, playerPower);   // �÷��̾ ���� ������
            BattleManager.Instance.currentMonsterHealth -= damage;  // ���� ���� HP ����
            BattleManager.Instance.monsterHealth.value = BattleManager.Instance.currentMonsterHealth; // ���� HP �����̴� ������Ʈ

            textMonsterList[m_i].SetText(damage.ToString());    // ������ �ؽ�Ʈ�� �� ������Ʈ
             
            EffectManager.Instance.monsterDamagedEffect(m_i++); // ������ �ؽ�Ʈ ����
            EffectManager.Instance.playerAttackEffect();        // �÷��̾��� ���� ��� �۵�
            
            // ������ �ؽ�Ʈ �� 5��, 0~4 ���������� ������ �ؽ�Ʈ ���. 4���� �۵��ϸ� �ٽ� 0������ �۵�
            if (m_i == 4)
                m_i = 0;

            if (BattleManager.Instance.monsterHealth.value <= 0)   // ������ ü���� 0�� �Ǿ��� ���
            {
                Debug.Log("Monster is defeated");
            }
        }
    }
}
