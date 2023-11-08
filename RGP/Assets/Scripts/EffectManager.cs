using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EffectManager : MonoBehaviour
{

    static EffectManager instance;
    public static EffectManager Instance
    {
        get
        {
            return instance;
        }
    }
    void Awake()
    {
        if (instance == null)
            instance = this;
    }


    //public GameObject[] lineEffect = new GameObject[4];
    public Animator[] lineEffectAnimator = new Animator[4];

    //public GameObject[] hitEffect = new GameObject[4];
    public Animator[] hitEffectAnimator = new Animator[4];

    public Animator[] coolBomb = new Animator[4];
    public Image judgeTextImage;
    public Animator judgeTextAnimator = new Animator();
    public Sprite[] max_Images = new Sprite[12];
    
    public Animator comboImage = new Animator();
    public Animator comboText = new Animator();

    public GameObject comboImage_object;
    public GameObject comboText_object;

    public Animator player = new Animator();
    public Animator monster = new Animator();

    public Animator[] playerDamaged = new Animator[5];
    public Animator[] monsterDamaged = new Animator[5];

    public void activate_lineEffect(int line)
    {
        hitEffectAnimator[line].SetTrigger("on_btnHitEffect");
        lineEffectAnimator[line].SetTrigger("on_lineEffect");
    }

    public void deactivate_lineEffect(int line)
    {
        hitEffectAnimator[line].SetTrigger("off_btnHitEffect");
        lineEffectAnimator[line].SetTrigger("off_lineEffect");
    }


    public void coolbomb_Animation(int line, int max_index, int notetype_long)  //�ִϸ��̼� ���� ����:int line  ��ƮŸ��(�ճ�Ʈ, ����Ʈ): int notetype_long
    {
        if(max_index != 11) // 11 == maxbreak�̹Ƿ� coolbomb ����Ʈ ����
        {
            if (notetype_long == 0) //����Ʈ�� ��� , �ճ�Ʈ�� �ƴ� ���
            {
                coolBomb[line].SetBool("coolbomb_max_loop", false);     //����Ʈ�̹Ƿ� �����Ķ���� ����
                coolBomb[line].SetBool("coolbomb_loop", false);

                if (max_index == 0)
                {
                    coolBomb[line].SetTrigger("coolbomb_max");
                }
                else
                {
                    coolBomb[line].SetTrigger("coolbomb");
                }
            }
            else  //�ճ�Ʈ�� ���
            {
                if (max_index == 0)
                {
                    coolBomb[line].SetTrigger("coolbomb_max");
                    coolBomb[line].SetBool("coolbomb_max_loop", true);
                }
                else
                {
                    coolBomb[line].SetTrigger("coolbomb");
                    coolBomb[line].SetBool("coolbomb_loop", true);
                }
            }
            
        }
    }

    public void judgeEffect(int max_index)
    {
        judgeTextImage.sprite = max_Images[max_index];
        judgeTextImage.SetNativeSize();
        judgeTextAnimator.SetTrigger("JudgeText");
    }

    public void comboEffect()
    {
        comboImage.SetTrigger("combo");
        comboText.SetTrigger("combo");
    }
    public void off_comboEffect()
    {
        comboImage.SetTrigger("offcombo");
    }

    // �÷��̾� ���� ���
    public void playerAttackEffect()
    {
        // �÷��̾ ���� ���� �ƴ� ��
        if (player.GetBool("attack") == false)
        {
            player.SetBool("damaged", false);
            monster.SetBool("attack", false);

            player.SetBool("attack", true);
            monster.SetBool("damaged", true);
        }
    }
        
    // ���� ���� ���
    public void monsterAttackEffect()
    {
        // ���Ͱ� ���� ���� �ƴ� ��
        if (monster.GetBool("attack") == false)
        {
            monster.SetBool("damaged", false);
            player.SetBool("attack", false);

            monster.SetBool("attack", true);
            player.SetBool("damaged", true);
        }
        
    }

    // �÷��̾ ������ �޴� �ؽ�Ʈ ����
    public void playerDamagedEffect(int i)
    {
        playerDamaged[i].SetTrigger("damaged");        
    }

    // ���Ͱ� ������ �޴� �ؽ�Ʈ ����
    public void monsterDamagedEffect(int i)
    {
        monsterDamaged[i].SetTrigger("damaged");
    }


}
