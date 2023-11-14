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


    public void coolbomb_Animation(int line, int max_index, int notetype_long)  //애니메이션 실행 라인:int line  노트타입(롱노트, 숏노트): int notetype_long
    {
        if(max_index != 11) // 11 == maxbreak이므로 coolbomb 이펙트 없음
        {
            if (notetype_long == 0) //숏노트인 경우 , 롱노트가 아닌 경우
            {
                coolBomb[line].SetBool("coolbomb_max_loop", false);     //숏노트이므로 루프파라미터 해제
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
            else  //롱노트인 경우
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

    // 플레이어 공격 모션
    public void playerAttackEffect()
    {
        // 플레이어가 공격 중이 아닐 때
        if (player.GetBool("attack") == false)
        {
            player.SetBool("damaged", false);
            monster.SetBool("attack", false);

            player.SetBool("attack", true);
            monster.SetBool("damaged", true);
        }
    }
        
    // 몬스터 공격 모션
    public void monsterAttackEffect()
    {
        // 몬스터가 공격 중이 아닐 떄
        if (monster.GetBool("attack") == false)
        {
            monster.SetBool("damaged", false);
            player.SetBool("attack", false);

            monster.SetBool("attack", true);
            player.SetBool("damaged", true);
        }
        
    }

    // 플레이어가 데미지 받는 텍스트 띄우기
    public void playerDamagedEffect(int i)
    {
        playerDamaged[i].SetTrigger("damaged");        
    }

    // 몬스터가 데미지 받는 텍스트 띄우기
    public void monsterDamagedEffect(int i)
    {
        monsterDamaged[i].SetTrigger("damaged");
    }


}
