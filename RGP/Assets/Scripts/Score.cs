using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public struct ScoreData
{
    public int max100;
    public int max90;
    public int max80;
    public int max70;
    public int max60;
    public int max50;
    public int max40;
    public int max30;
    public int max20;
    public int max10;
    public int max1;
    public int maxbreak;
    public int fastMiss; // 빨리 입력해서 미스
    public int longMiss; // 롱노트 완성 실패, miss 카운트는 하지 않음
    
    public JudgeType judge;
    public int combo;
    public int score
    {
        get
        {
            return 
                (max100 * 100) + (max90 * 90) + (max80 * 80) 
                + (max70 * 70) + (max60 * 60) + (max50 * 50) 
                + (max90 * 40) + (max30 * 30) + (max20 * 20) 
                + (max90 * 10) + (max90 * 1);
        }
        set
        {
            score = value;
        }
    }
}

public class Score : MonoBehaviour
{
    static Score instance;
    public static Score Instance
    {
        get { return instance; }
    }

    public ScoreData data;
    
    UIText uiCombo;
    UIText uiScore;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void Init()
    {
        uiCombo = UIController.Instance.FindUI("ComboText").uiObject as UIText;
        uiScore = UIController.Instance.FindUI("ScoreText").uiObject as UIText;

        //AniPreset.Instance.Join(uiCombo.Name);
        //AniPreset.Instance.Join(uiScore.Name);
    }

    public void Clear()
    {
        data = new ScoreData();
        uiCombo.SetText("");
        uiScore.SetText("0");
    }


    public void SetScore()
    {
        EffectManager.Instance.judgeEffect((int)data.judge);
        if(data.combo >= 2)
        {
            uiCombo.SetText($"{data.combo}");
            EffectManager.Instance.comboEffect();
        }
        else
        {
            uiCombo.SetText("");
            EffectManager.Instance.off_comboEffect();
        }
        uiScore.SetText($"{data.score}");

        //AniPreset.Instance.PlayPop(uiJudgement.Name, uiJudgement.rect);
        //AniPreset.Instance.PlayPop(uiCombo.Name, uiCombo.rect);

        //UIController.Instance.find.Invoke(uiJudgement.Name);
        //UIController.Instance.find.Invoke(uiCombo.Name);
    }
}