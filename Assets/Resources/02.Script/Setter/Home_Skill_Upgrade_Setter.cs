using CUSTOM_DATA;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Common_Data;
using static CUSTOM_DATA.Game_State_Data;
using static CUSTOM_DATA.Game_Value_Data;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class Home_Skill_Upgrade_Setter : Scene_Singleton<Home_Skill_Upgrade_Setter>
{
    private Sequence sq;
    private Sequence result_seq;
    private Sequence end_seq;
    [SerializeField] Transform confirm_pannel;
    [SerializeField] Transform loading_panel;
    [SerializeField] Transform result_pannel;
    [SerializeField] private GameObject loading_bar;

    private Vector3 ori_pos = new(0, 1200f, 0);
    private Vector3 close = new(0, 90, 0);

    private SkillType target_skill;

    private void Start() {
        float top = Screen.height;
        float bottom = 0;
        ori_pos.y = bottom;
        loading_bar.transform.position = ori_pos;

        sq = DOTween.Sequence().Pause().SetAutoKill(false)
                    .Append(loading_bar.transform.DOMoveY(top, 1.5f).SetEase(Ease.InOutExpo))
                    .Append(loading_bar.transform.DOMoveY(bottom, 1.5f).SetEase(Ease.InOutExpo))
                    .AppendCallback(Check_Load_End);

        result_seq = DOTween.Sequence()
                            .Pause()
                            .SetAutoKill(false)
                            .Append(loading_panel.transform.DORotate(close, .25f))
                            .Join(result_pannel.transform.DORotate(open, .25f));

        end_seq = DOTween.Sequence().Pause().SetAutoKill(false)
                         .Append(result_pannel.transform.DORotate(close, .25f));
    }

    // skill_name => 스킬 이름
    // now_value => 현재 수치
    // next_value => 다음 수치
    // Confirm => 확인 버튼
    public void Setting(SkillType skill) {
        target_skill = skill;

        string name = skill switch {
            SkillType.AttackIncrease => "공격력 증가",
            SkillType.GainGold => "몬스터 처치 시 골드 획득량 증가",
            SkillType.ClearGold => "게임 클리어 시 골드 획득량 증가",
            SkillType.MissionGold => "미션 클리어 시 골드 획득량 증가",
            SkillType.ExtraSummon => "소환 시 확률로 E급 추가 획득",
            _ => "__"
        };

        int now_value = user.GetSkillValue(skill);
        int next_value = user.GetSkillValueNext(skill);

        confirm_pannel.Find("Border").Find("skill_name").GetComponent<TMP_Text>().text = name;
        confirm_pannel.Find("Border").Find("now_value").GetComponent<TMP_Text>().text = now_value.ToString() + "%";
        confirm_pannel.Find("Border").Find("next_value").GetComponent<TMP_Text>().text = next_value.ToString() + "%";

        // 확인에 btn
        confirm_pannel.Find("Confirm").GetComponent<Button>().onClick.RemoveAllListeners();
        confirm_pannel.Find("Confirm").GetComponent<Button>().onClick.AddListener(() => {
            Loading();
        });

        result_pannel.Find("Border").Find("skill_name").GetComponent<TMP_Text>().text = name;
        result_pannel.Find("Border").Find("now_value").GetComponent<TMP_Text>().text = now_value.ToString() + "%";
        result_pannel.Find("Border").Find("next_value").GetComponent<TMP_Text>().text = next_value.ToString() + "%";

        // pannel open
        confirm_pannel.transform.DORotate(open, .25f);
    }

    public void Loading() {
        // upgrade 
        user.AddSkillPoint(target_skill);
        confirm_pannel.transform.DORotate(close, .25f);
        loading_panel.transform.DORotate(open, .25f);
        sq.Restart();
    }

    public void Result_On() {
        result_seq.Restart();
    }

    public void Confrim() {
        // end 
        end_seq.Restart();
        // 화면 리로드
        Home_Skill_Setter.Instance.Skill_Init();
    }

    public void Cancel() {
        confirm_pannel.transform.DORotate(close, .25f);
    }

    private void Check_Load_End() {
        if (db_state == DB_STATE.Usable && Header_Setter.Instance.setting) {
            // result announce
            Result_On();
        }
        else {
            sq.Restart();
        }
    }
    
}
