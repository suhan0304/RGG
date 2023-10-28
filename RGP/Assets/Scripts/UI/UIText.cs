using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIText : UIObject
{
    TextMeshProUGUI textTMP;
    Text text;

    private void Start()
    {
        textTMP = GetComponent<TextMeshProUGUI>();
        if(textTMP == null)
        {
            text = GetComponent<Text>();
        }
    }

    public void SetText(string _text)
    {
        if (textTMP == null)
        {
            text.text = _text;
        }
        else
        {
            textTMP.text = _text;
        }
    }

    public void SetColor(Color color)
    {
        if (textTMP == null)
        {
            text = GetComponent<Text>();
        }
        else
        {
            textTMP.color = color;
        }
    }

    public void ChangeText()
    {
        //UIController.Instance.find.Invoke(Name);
    }
}
