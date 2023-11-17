using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemSelection : MonoBehaviour
{
    public Button item1Button;
    public Button item2Button;
    public Button item3Button;

    static ItemSelection instance;
    public bool isSelect;
    void Start()
    {
        isSelect = false;
    }
    public static ItemSelection Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    public void ShowItemSelectionPanel()
    {
        BattleManager.Instance.StopCoUpdateHealth();
        // ������ ���� �г��� Ȱ��ȭ
        GameManager.Instance.uiItemSelectionPanel.SetActive(true);
        item1Button.onClick.AddListener(SelectItem1);
        item2Button.onClick.AddListener(SelectItem2);
        item3Button.onClick.AddListener(SelectItem3);
        
    }
    void SelectItem1()
    {
        Debug.Log("������ 1�� ���õǾ����ϴ�!");
        isSelect = true;
        GameManager.Instance.NextStage();
    }

    void SelectItem2()
    {
        Debug.Log("������ 2�� ���õǾ����ϴ�!");
        isSelect = true;
        GameManager.Instance.NextStage();
    }

    void SelectItem3()
    {
        Debug.Log("������ 3�� ���õǾ����ϴ�!");
        isSelect = true;
        GameManager.Instance.NextStage();
    }
}