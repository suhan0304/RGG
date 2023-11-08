using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 판정 종류
public enum JudgeType
{
    max100,
    max90,
    max80,
    max70,
    max60,
    max50,
    max40,
    max30,
    max20,
    max10,
    max1,
    maxbreak
}

public class Judgement : MonoBehaviour
{
    // 판정 범위(ms) 설정
    readonly int max100 = 42;
    readonly int max90 = 60;
    readonly int max80 = 78;
    readonly int max70 = 99;
    readonly int max60 = 120;
    readonly int max50 = 129;
    readonly int max40 = 138;
    readonly int max30 = 147;
    readonly int max20 = 159;
    readonly int max10 = 177;
    readonly int max1 = 400;
    readonly int maxbreak = 600;


    // 노트 시간 정보를 담을 큐 및 큐들을 담을 리스트 
    List<Queue<Note>> notes = new List<Queue<Note>>();
    Queue<Note> note1 = new Queue<Note>();
    Queue<Note> note2 = new Queue<Note>();
    Queue<Note> note3 = new Queue<Note>();
    Queue<Note> note4 = new Queue<Note>();


    int[] longNoteCheck = new int[4] { 0, 0, 0, 0 };

    int curruntTime = 0;
    /// User에 의해 조정된 판정 타이밍
    public int judgeTimeFromUserSetting = 0;

    Coroutine coCheckMiss;

    public void Init()
    {
        foreach (var note in notes)
        {
            note.Clear();
        }
        notes.Clear();

        foreach (var note in GameManager.Instance.sheets[GameManager.Instance.title].notes)
        {
            if (note.line == 1)
                note1.Enqueue(note);
            else if (note.line == 2)
                note2.Enqueue(note);
            else if (note.line == 3)
                note3.Enqueue(note);
            else
                note4.Enqueue(note);
        }
        notes.Add(note1);
        notes.Add(note2);
        notes.Add(note3);
        notes.Add(note4);

        if (coCheckMiss != null)
        {
            StopCoroutine(coCheckMiss);
        }
        coCheckMiss = StartCoroutine(IECheckMiss());
    }

    public void Judge(int line)
    {
        if (notes[line].Count <= 0 || !AudioManager.Instance.IsPlaying())
            return;

        Note note = notes[line].Peek();
        // judgeTime = 노래 진행 시간 - 노트 판정 시간 : 해당 값을 이용해 판정
        int judgeTime = curruntTime - note.time + judgeTimeFromUserSetting;
        JudgeType note_Judgement = JudgeType.max1;


        if (judgeTime < maxbreak && judgeTime > -maxbreak)  // judgeTime이 maxBreak 범위 안에 들어오면 -> 판정 시작
        {
            if (judgeTime < max1 && judgeTime > -max1)      // max1 범위 안에 break가 아니라 점수로 인정 : combo, effect 동작 실행
            {
                if (judgeTime <= max100 && judgeTime >= -max100)
                {
                    Score.Instance.data.max100++;
                    note_Judgement = JudgeType.max100;
                }
                else if (judgeTime <= max90 && judgeTime >= -max90)
                {
                    Score.Instance.data.max90++;
                    note_Judgement = JudgeType.max90;
                }
                else if (judgeTime <= max80 && judgeTime >= -max80)
                {
                    Score.Instance.data.max80++;
                    note_Judgement = JudgeType.max80;
                }
                else if (judgeTime <= max70 && judgeTime >= -max70)
                {
                    Score.Instance.data.max70++;
                    note_Judgement = JudgeType.max70;
                }
                else if (judgeTime <= max60 && judgeTime >= -max60)
                {
                    Score.Instance.data.max60++;
                    note_Judgement = JudgeType.max60;
                }
                else if (judgeTime <= max50 && judgeTime >= -max50)
                {
                    Score.Instance.data.max50++;
                    note_Judgement = JudgeType.max50;
                }
                else if (judgeTime <= max40 && judgeTime >= -max40)
                {
                    Score.Instance.data.max40++;
                    note_Judgement = JudgeType.max40;
                }
                else if (judgeTime <= max30 && judgeTime >= -max30)
                {
                    Score.Instance.data.max30++;
                    note_Judgement = JudgeType.max30;
                }
                else if (judgeTime <= max20 && judgeTime >= -max20)
                {
                    Score.Instance.data.max20++;
                    note_Judgement = JudgeType.max20;
                }
                else if (judgeTime <= max10 && judgeTime >= -max10)
                {
                    Score.Instance.data.max10++;
                    note_Judgement = JudgeType.max10;
                }
                else
                {
                    Score.Instance.data.max1++;
                    note_Judgement = JudgeType.max1;
                }
                Score.Instance.data.combo++;

            }
            else // break 범위 안에는 들어옴, max1 범위 안에 못들어옴 => break 판정 (miss)
            {
                Score.Instance.data.fastMiss++;
                note_Judgement = JudgeType.maxbreak;
                Score.Instance.data.combo = 0;      //break시 콤보 초기화
            }
            Score.Instance.data.judge = note_Judgement; // Score에 판정 결과를 넘김
            Score.Instance.SetScore();                  // Score의 SetScore를 진행

            Damage.Instance.Attack(note_Judgement);  // BattleManager에서 판정에 맞는 공격 결과 넘김


            

            if (note.type == (int)NoteType.Short)       //Short 노트 : 바로 Release를 진행
            {
                
                EffectManager.Instance.coolbomb_Animation(line, (int)note_Judgement, (int)NoteType.Short); // Combo Animation 실행
                Note ReleaseNote = notes[line].Dequeue();
                NoteGenerator.Instance.judgedNoteRelease(ReleaseNote.line - 1);
            }
            else if (note.type == (int)NoteType.Long)   // Long 노트 : 꾹 누르고 있어야 하므로 CheckLongNote 동작
            {
                EffectManager.Instance.coolbomb_Animation(line, (int)note_Judgement, (int)NoteType.Long); // Combo Animation 실행
                if (note_Judgement != JudgeType.maxbreak)
                {
                    longNoteCheck[line] = 1;
                }
            }
        }
    }

    public void CheckLongNote(int line)
    {
        if (notes[line].Count <= 0)
            return;

        Note note = notes[line].Peek();
        if (note.type != (int)NoteType.Long)
            return;

        if (longNoteCheck[line] == 1)
        {
            JudgeType note_Judgement = JudgeType.max1;
            int judgeTime = curruntTime - note.tail + judgeTimeFromUserSetting;
            if (judgeTime < max1 && judgeTime > -max1)      // max1 범위 안에 break가 아니라 점수로 인정 : combo, effect 동작 실행
            {
                if (judgeTime <= max100 && judgeTime >= -max100)
                {
                    Score.Instance.data.max100++;
                    note_Judgement = JudgeType.max100;
                }
                else if (judgeTime <= max90 && judgeTime >= -max90)
                {
                    Score.Instance.data.max90++;
                    note_Judgement = JudgeType.max90;
                }
                else if (judgeTime <= max80 && judgeTime >= -max80)
                {
                    Score.Instance.data.max80++;
                    note_Judgement = JudgeType.max80;
                }
                else if (judgeTime <= max70 && judgeTime >= -max70)
                {
                    Score.Instance.data.max70++;
                    note_Judgement = JudgeType.max70;
                }
                else if (judgeTime <= max60 && judgeTime >= -max60)
                {
                    Score.Instance.data.max60++;
                    note_Judgement = JudgeType.max60;
                }
                else if (judgeTime <= max50 && judgeTime >= -max50)
                {
                    Score.Instance.data.max50++;
                    note_Judgement = JudgeType.max50;
                }
                else if (judgeTime <= max40 && judgeTime >= -max40)
                {
                    Score.Instance.data.max40++;
                    note_Judgement = JudgeType.max40;
                }
                else if (judgeTime <= max30 && judgeTime >= -max30)
                {
                    Score.Instance.data.max30++;
                    note_Judgement = JudgeType.max30;
                }
                else if (judgeTime <= max20 && judgeTime >= -max20)
                {
                    Score.Instance.data.max20++;
                    note_Judgement = JudgeType.max20;
                }
                else if (judgeTime <= max10 && judgeTime >= -max10)
                {
                    Score.Instance.data.max10++;
                    note_Judgement = JudgeType.max10;
                }
                else if (judgeTime <= max1 && judgeTime >= -max1)
                {
                    Score.Instance.data.max1++;
                    note_Judgement = JudgeType.max1;
                }
            }
            else
            {
                Score.Instance.data.longMiss++;
            }
            Score.Instance.data.judge = note_Judgement;
            Score.Instance.SetScore();

            longNoteCheck[line] = 0;

            // Combo Animation 실행
            EffectManager.Instance.coolbomb_Animation(line, (int)note_Judgement,0);

            //Long 노트 : Release를 진행
            Note ReleaseNote = notes[line].Dequeue();
            NoteGenerator.Instance.FallNoteDequeue(ReleaseNote.line - 1);
        }
    }

    // 노트가 판정 기준의 범위 밖으로 나가버리면 -> 노트 판정 실패 = break (노트가 아래로 그냥 진행될 경우)
    IEnumerator IECheckMiss()
    {
        while (true)
        {
            curruntTime = (int)AudioManager.Instance.GetMilliSec();

            for (int i = 0; i < notes.Count; i++)
            {
                if (notes[i].Count <= 0)
                    break;

                Note note = notes[i].Peek();
                int judgeTime = note.time - curruntTime + judgeTimeFromUserSetting;

                if (note.type == (int)NoteType.Long)
                {
                    if (longNoteCheck[note.line - 1] == 0)  // Head가 판정처리가 안된 경우
                    {
                        // 노트가 -maxbreak 범위 밖으로 벗어남 = break 판정 진행
                        if (judgeTime < -maxbreak)
                        {
                            Score.Instance.data.maxbreak++;
                            Score.Instance.data.judge = JudgeType.maxbreak;
                            Score.Instance.data.combo = 0;
                            Score.Instance.SetScore();

                            Damage.Instance.Attack(JudgeType.maxbreak);  // BattleManager에서 플레이어가 공격 받도록 함

                            // break로 판정 후 큐에서 해당 노트 정보 Dequeue 후 release 진행
                            Note ReleaseNote = notes[i].Dequeue();
                            NoteGenerator.Instance.FallNoteDequeue(ReleaseNote.line - 1);
                        }
                    }
                }
                else
                {
                    // 노트가 -maxbreak 범위 밖으로 벗어남 = break 판정 진행
                    if (judgeTime < -maxbreak)
                    {
                        Score.Instance.data.maxbreak++;
                        Score.Instance.data.judge = JudgeType.maxbreak;
                        Score.Instance.data.combo = 0;
                        Score.Instance.SetScore();

                        Damage.Instance.Attack(JudgeType.maxbreak);  // BattleManager에서 플레이어가 공격 받도록 함

                        // break로 판정 후 큐에서 해당 노트 정보 Dequeue 후 release 진행
                        Note ReleaseNote = notes[i].Dequeue();
                        NoteGenerator.Instance.judgedNoteRelease(ReleaseNote.line - 1);
                    }
                }
            }

            yield return null;
        }
    }
}
