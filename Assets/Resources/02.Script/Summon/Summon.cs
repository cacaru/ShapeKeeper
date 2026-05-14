using CUSTOM_DATA;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Common_Data;

/// <summary>
/// E급 조각들을 소환하는 버튼을 눌렀을 때 
/// </summary>
public class Summon : Scene_Singleton<Summon>
{
    [SerializeField] Image circle;
    [SerializeField] Image triangle;
    [SerializeField] Image square;

    public TMP_Text e_circle_value;
    public TMP_Text e_triangle_value;
    public TMP_Text e_square_value;

    private readonly int need_dot_count = 20;

    Sequence summon_circle;
    Sequence summon_triangle;
    Sequence summon_square;

    void Start() {
        summon_circle = DOTween.Sequence().Pause().SetAutoKill(false)
                               .Append(circle.DOFillAmount(1, .1f))
                               .AppendCallback(() => {
                                   e_circle_value.text = unit_counter[1001].count.ToString();
                                   circle.fillAmount = 0;
                               });
        summon_triangle = DOTween.Sequence().Pause().SetAutoKill(false)
                               .Append(triangle.DOFillAmount(1, .1f))
                               .AppendCallback(() => {
                                   e_triangle_value.text = unit_counter[1002].count.ToString();
                                   triangle.fillAmount = 0;
                               });
        summon_square = DOTween.Sequence().Pause().SetAutoKill(false)
                               .Append(square.DOFillAmount(1, .1f))
                               .AppendCallback(() => {
                                   e_square_value.text = unit_counter[1003].count.ToString();
                                   square.fillAmount = 0;
                               });
    }
    private Coroutine summon_coroutine;
    private bool is_summoning = false;
    public void On_Summon_Btn_Down() {
        if (!is_summoning) {
            is_summoning = true;
            summon_coroutine = StartCoroutine(Auto_Summon_Coroutine());
        }
    }

    public void On_Summon_Btn_Up() {
        is_summoning = false;
        if (summon_coroutine != null) {
            StopCoroutine(summon_coroutine);
            summon_coroutine = null;
        }
    }

    IEnumerator Auto_Summon_Coroutine() {
 
        while (is_summoning) {
            Summon_Unit();
            yield return wfs_0_1;

        }
    }

    // 해야하는 것
    // 버튼을 눌렀을 때 동일한 확률로 원 / 세모 / 네모를 출현시킴
    public void Summon_Unit() {
        // 재화 확인
        if (dot < need_dot_count) {return;}
        Set_dot(dot - need_dot_count);
        E_Unit_Summon();

        // skill 에 의한 extra summon
        // 연속성 없음
        int skill_bonus_rate = user.GetSkillValue(SkillType.ExtraSummon);
        if (skill_bonus_rate < 0) return;
        int rand = Random.Range(1, 101); // 1 ~ 100
        // 25~75 사이의 숫자가 나오면 추가 소환 진행
        if (rand >= 25 && rand < 25 + skill_bonus_rate) E_Unit_Summon();

    }

    private void E_Unit_Summon() {
        int rand = Random.Range(1, 1000); // 1 ~ 999
        int type = 0;
        // circle
        if (rand >= 1 && rand <= 333) {
            type = 1;
            unit_counter[1001].count++;
        }
        // triangle
        else if (rand >= 334 && rand <= 666) {
            type = 2;
            unit_counter[1002].count++;
        }
        // square
        else if (rand >= 667 && rand <= 999) {
            type = 3;
            unit_counter[1003].count++;
        }
        // 업적 추가
        Achievement_Observer.Instance.Summon_Achievement();
        Achievement_Observer.Instance.Use_Dot_Achievement(need_dot_count);
        // 결정되었으면 install area의 content에 unit_prefab을 추가함 => E급은 무채색으로 설치 할 수 없게 해야함 -> 그냥 수치화 하시죠
        E_Unit_Display(type);
        // 조합식이 떠있으면 갱신해줘야함
        Combine_Function_Creater.Show_Again_Now();
    }

    public void E_Unit_Display(int type) {
        // animate on
        switch (type) {
            case 1:
                summon_circle.Restart();
                break;
            case 2:
                summon_triangle.Restart();
                break;
            case 3:
                summon_square.Restart();
                break;
        }
    }

    public void E_Unit_Display() {
        e_circle_value.text = unit_counter[1001].count.ToString();
        e_triangle_value.text = unit_counter[1002].count.ToString();
        e_square_value.text = unit_counter[1003].count.ToString();
    }
}
