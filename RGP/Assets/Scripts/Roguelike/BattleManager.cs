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

    public bool isClear = false;    // �������� Ŭ���� ����
    public Coroutine coUpdateHealth = null; // ü�� �����̴� ������Ʈ ���� �ڷ�ƾ
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
        // �뷡�� ������ ��, �÷��̾ ����ִٸ� Ŭ���� �������� �Ѿ��
        if (isClear && !AudioManager.Instance.IsPlaying())  
        {
            isClear = false;
            ClearStage();
        }
    }

    // �÷��̾��� ü��, ������ ü�� �� ����
    public void Init()
    {
        // �ӽ� �ʱ�ȭ
        playerHealthAmount = 3000;  //�ӽ÷� 3000���� ����
        currentPlayerHealth = playerHealthAmount;
        playerHealth.maxValue = playerHealthAmount; // �÷��̾� �����̴� ����
        playerHealth.value = playerHealthAmount;

        monsterHealthAmount = 50000;
        currentMonsterHealth = monsterHealthAmount;
        monsterHealth.maxValue = monsterHealthAmount;   // ���� �����̴� ����
        monsterHealth.value = monsterHealthAmount;
    }

    // �÷��̾��� Ŭ���� ���� ����
    public void ClearStage()
    {
        
        if (currentMonsterHealth > 0)   // ������ ü���� �����ִٸ�
        {
            if (coUpdateHealth == null)
            {
                coUpdateHealth = StartCoroutine(ClearUpdateHealth());
            }
        }
        else                            // ������ ü���� 0�̸�
        {
            ItemSelection.Instance.ShowItemSelectionPanel();
            
        }
    }

    // �뷡 ���� �� �÷��̾�� ������ ü�� ������Ʈ
    IEnumerator ClearUpdateHealth()
    {
        float remainedPlayerHealth = (float) currentPlayerHealth - (float) playerHealthAmount * ((float)currentMonsterHealth / (float) monsterHealthAmount);    // ���� �÷��̾��� ü��
        float duration = 5f;    // �����ϴ� �ð� ����
        float elapsedTime = 0f; // ����� �ð�

        // ������ �ð���ŭ �ݺ�
        while (elapsedTime < duration)
        {
            monsterHealth.value = Mathf.Lerp(currentMonsterHealth, 0, elapsedTime / duration);  // ���� ü�� �����̴� duration ���� ������ ���ҽ�Ű��
            playerHealth.value = Mathf.Lerp(currentPlayerHealth, remainedPlayerHealth, elapsedTime / duration); // �÷��̾��� ü�� �����̴� duration ���� ������ ���ҽ�Ű��
            elapsedTime += Time.deltaTime;  // ���� �ð���ŭ ���� (������ ����)
            yield return null;
        }
        // ���Ϳ� �÷��̾� ���� ü�� ������Ʈ
        currentMonsterHealth = 0;
        currentPlayerHealth = (int) remainedPlayerHealth;

        if (currentPlayerHealth <= 0)   // ���� �÷��̾� ü���� 0�� �Ǹ� ���� ����
        {
            isClear = false;
            GameManager.Instance.GameOver();    // ���� ���� ȭ������ �̵�
        }
        else                            // ���� �÷��̾� ü���� 0 �ʰ��� ����
        {
            ItemSelection.Instance.ShowItemSelectionPanel();
            //GameManager.Instance.NextStage();   // ���� �������� ���� ȭ������ �̵�
        }
    }
    public void StopCoUpdateHealth()
    {
        // �ڷ�ƾ ����
        if (coUpdateHealth != null)
        {
            StopCoroutine(coUpdateHealth);
            Debug.Log("�ڷ�ƾ ����");
        }
        coUpdateHealth = null;
    }

}
