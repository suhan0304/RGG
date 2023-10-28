using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        int judgeTime = curruntTime - note.time + judgeTimeFromUserSetting;
        JudgeType note_Judgement = JudgeType.max1;

        if (judgeTime < maxbreak && judgeTime > -maxbreak)
        {
            if (judgeTime < max1 && judgeTime > -max1)
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
            else
            {
                Score.Instance.data.fastMiss++;
                note_Judgement = JudgeType.maxbreak;
                Score.Instance.data.combo = 0;
            }
            Score.Instance.data.judge = note_Judgement;
            Score.Instance.SetScore();


            EffectManager.Instance.coolbomb_Animation(line, (int)note_Judgement);

            if (note.type == (int)NoteType.Short)
            {
                Note ReleaseNote = notes[line].Dequeue();
                NoteGenerator.Instance.judgedNoteRelease(ReleaseNote.line - 1);
            }
            else if (note.type == (int)NoteType.Long)
            {
                longNoteCheck[line] = 1;
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

        JudgeType note_Judgement = JudgeType.max1;
        int judgeTime = curruntTime - note.tail + judgeTimeFromUserSetting;
        if (judgeTime < max1 && judgeTime > -max1)
        {
            if (judgeTime <= max100 && judgeTime >= -max100)
            {
                Score.Instance.data.max100++;
                note_Judgement = JudgeType.max100;
                Score.Instance.data.combo++;
            }
            else if (judgeTime <= max90 && judgeTime >= -max90)
            {
                Score.Instance.data.max90++;
                note_Judgement = JudgeType.max90;
                Score.Instance.data.combo++;
            }
            else if (judgeTime <= max80 && judgeTime >= -max80)
            {
                Score.Instance.data.max80++;
                note_Judgement = JudgeType.max80;
                Score.Instance.data.combo++;
            }
            else if (judgeTime <= max70 && judgeTime >= -max70)
            {
                Score.Instance.data.max70++;
                note_Judgement = JudgeType.max70;
                Score.Instance.data.combo++;
            }
            else if (judgeTime <= max60 && judgeTime >= -max60)
            {
                Score.Instance.data.max60++;
                note_Judgement = JudgeType.max60;
                Score.Instance.data.combo++;
            }
            else if (judgeTime <= max50 && judgeTime >= -max50)
            {
                Score.Instance.data.max50++;
                note_Judgement = JudgeType.max50;
                Score.Instance.data.combo++;
            }
            else if (judgeTime <= max40 && judgeTime >= -max40)
            {
                Score.Instance.data.max40++;
                note_Judgement = JudgeType.max40;
                Score.Instance.data.combo++;
            }
            else if (judgeTime <= max30 && judgeTime >= -max30)
            {
                Score.Instance.data.max30++;
                note_Judgement = JudgeType.max30;
                Score.Instance.data.combo++;
            }
            else if (judgeTime <= max20 && judgeTime >= -max20)
            {
                Score.Instance.data.max20++;
                note_Judgement = JudgeType.max20;
                Score.Instance.data.combo++;
            }
            else if (judgeTime <= max10 && judgeTime >= -max10)
            {
                Score.Instance.data.max10++;
                note_Judgement = JudgeType.max10;
                Score.Instance.data.combo++;
            }
            else
            {
                Score.Instance.data.longMiss++;
            }
            Score.Instance.data.judge = note_Judgement;
            Score.Instance.SetScore();

            EffectManager.Instance.coolbomb_Animation(line, (int)note_Judgement);

            longNoteCheck[line] = 0;
            notes[line].Dequeue();
        }
    }

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
                    if (longNoteCheck[note.line - 1] == 0) // Head가 판정처리가 안된 경우
                    {
                        if (judgeTime < -maxbreak)
                        {
                            Score.Instance.data.maxbreak++;
                            Score.Instance.data.judge = JudgeType.maxbreak;
                            Score.Instance.data.combo = 0;
                            Score.Instance.SetScore();
                            Note ReleaseNote = notes[i].Dequeue();
                            NoteGenerator.Instance.judgedNoteRelease(ReleaseNote.line-1);
                        }
                    }
                }
                else
                {
                    if (judgeTime < -maxbreak)
                    {
                        Score.Instance.data.maxbreak++;
                        Score.Instance.data.judge = JudgeType.maxbreak;
                        Score.Instance.data.combo = 0;
                        Score.Instance.SetScore();
                        Note ReleaseNote = notes[i].Dequeue();
                        NoteGenerator.Instance.judgedNoteRelease(ReleaseNote.line-1);
                    }
                }
            }

            yield return null;
        }
    }
}
