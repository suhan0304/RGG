using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSelection : MonoBehaviour
{
    public GameObject itemSelectionPanel; // UI 패널을 연결할 변수
    public Button item1Button;
    public Button item2Button;
    public Button item3Button;

    void Start()
    {
        // 아이템 선택 패널을 비활성화
        if (itemSelectionPanel != null)
        {
            itemSelectionPanel.SetActive(false);
        }
    }

    public void ShowItemSelectionPanel()
    {
        // 아이템 선택 패널을 활성화
        if (itemSelectionPanel != null)
        {
            itemSelectionPanel.SetActive(true);
            item1Button.onClick.AddListener(SelectItem1);
            item2Button.onClick.AddListener(SelectItem2);
            item3Button.onClick.AddListener(SelectItem3);
        }
    }

    void SelectItem1()
    {
        Debug.Log("아이템 1이 선택되었습니다!");
        if (itemSelectionPanel != null)
        {
            itemSelectionPanel.SetActive(false);
        }
    }

    void SelectItem2()
    {
        Debug.Log("아이템 2가 선택되었습니다!");
        if (itemSelectionPanel != null)
        {
            itemSelectionPanel.SetActive(false);
        }
    }

    void SelectItem3()
    {
        Debug.Log("아이템 3이 선택되었습니다!");
        if (itemSelectionPanel != null)
        {
            itemSelectionPanel.SetActive(false);
        }
    }
}
