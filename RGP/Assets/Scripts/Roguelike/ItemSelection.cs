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
        // 아이템 선택 패널을 활성화
        GameManager.Instance.uiItemSelectionPanel.SetActive(true);
        item1Button.onClick.AddListener(SelectItem1);
        item2Button.onClick.AddListener(SelectItem2);
        item3Button.onClick.AddListener(SelectItem3);
        
    }
    void SelectItem1()
    {
        Debug.Log("아이템 1이 선택되었습니다!");
        isSelect = true;
        GameManager.Instance.NextStage();
    }

    void SelectItem2()
    {
        Debug.Log("아이템 2가 선택되었습니다!");
        isSelect = true;
        GameManager.Instance.NextStage();
    }

    void SelectItem3()
    {
        Debug.Log("아이템 3이 선택되었습니다!");
        isSelect = true;
        GameManager.Instance.NextStage();
    }
}