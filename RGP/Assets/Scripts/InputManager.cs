using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    Judgement judgement = null;
    Sync sync = null;

    // Start is called before the first frame update
    void Start()
    {

        /*
        foreach 
            (var effect in keyEffects)
        {
           effect.gameObject.SetActive(false);
        }
        */
        //sync = FindObjectOfType<Sync>();

        judgement = FindObjectOfType<Judgement>();
        sync = FindObjectOfType<Sync>();
    }

    Update is called once per frame
    void Update()
    {
    }

    public void OnNoteLine0(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            judgement.Judge(0);
            EffectManager.Instance.activate_lineEffect(0);
        }
        else if (context.canceled)
        {
            judgement.CheckLongNote(0);
            EffectManager.Instance.deactivate_lineEffect(0);
        }
    }
    public void OnNoteLine1(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            judgement.Judge(1);
            EffectManager.Instance.activate_lineEffect(1);
        }
        else if (context.canceled)
        {
            judgement.CheckLongNote(1);
            EffectManager.Instance.deactivate_lineEffect(1);
        }
    }
    public void OnNoteLine2(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            judgement.Judge(2);
            EffectManager.Instance.activate_lineEffect(2);
        }
        else if (context.canceled)
        {
            judgement.CheckLongNote(2);
            EffectManager.Instance.deactivate_lineEffect(2);
        }
    }
    public void OnNoteLine3(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            judgement.Judge(3);
            EffectManager.Instance.activate_lineEffect(3);
        }
        else if (context.canceled)
        {
            judgement.CheckLongNote(3);
            EffectManager.Instance.deactivate_lineEffect(3);
        }
    }

    public void OnEnter(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (GameManager.Instance.state == GameManager.GameState.GamePlaying)
            {
                if (!GameManager.Instance.isPlaying)
                    GameManager.Instance.Play();
            }
        }
    }

    public void OnJudgeDown(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (GameManager.Instance.isPlaying)
                sync.Down();
        }
    }

    public void OnJudgeUp(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (GameManager.Instance.isPlaying)
                sync.Up();
        }
    }
}
