using CUSTOM_DATA;
using System.Collections;
using UnityEngine;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Game_Value_Data;
using static CUSTOM_DATA.Common_Data;
using TMPro;
/// <summary>
/// 유닛 페이지에서 유닛을 강화할 때 사용
/// </summary>
public class Upgrade : Scene_Singleton<Upgrade>
{
    private readonly string NEED_GOLD = "강화에 필요한 골드가 부족합니다.";
    private readonly string NEED_PIECE = "강화에 필요한 조각이 부족합니다.";

    [SerializeField] private GameObject Announce;
    [SerializeField] private GameObject upgrade_particle;
    [SerializeField] private Animator upgrade_window;
    [SerializeField] private Animator target_image;

    public int unit_id = -1;
    public int upgrade = -1;

    public void Upgrade_Check() {
        if(unit_id < 0 || upgrade <= 0) return;
        
        var _unit = unit[unit_id];

        int upgrade_cost_figure = _unit.grade switch {
            "d" => D_UPGRADE_GOLD_COEFFICENT,
            "c" => C_UPGRADE_GOLD_COEFFICENT,
            "b" => B_UPGRADE_GOLD_COEFFICENT,
            "a" => A_UPGRADE_GOLD_COEFFICENT,
            "s" => S_UPGRADE_GOLD_COEFFICENT,
            "ex" => EX_UPGRADE_GOLD_COEFFICENT,
            _ => 0
        };
        int upgrade_piece_figure = _unit.grade switch {
            "d" => D_UPGRADE_PIECE_COEFFICENT,
            "c" => C_UPGRADE_PIECE_COEFFICENT,
            "b" => B_UPGRADE_PIECE_COEFFICENT,
            "a" => A_UPGRADE_PIECE_COEFFICENT,
            "s" => S_UPGRADE_PIECE_COEFFICENT,
            "ex" => EX_UPGRADE_PIECE_COEFFICENT,
            _ => 0
        };
        // 타겟 upgrade value에 따른 골드량 체크
        int need_gold = upgrade * upgrade_cost_figure;
        if (user.gold < need_gold) {
            // 골드 부족 announce
            Announce.transform.Find("announce").GetComponent<TMP_Text>().text = NEED_GOLD;
            Announce.SetActive(true);
            return;
        }
        // piece 체크
        int need_piece = upgrade * upgrade_piece_figure;
        if(_unit.piece < need_piece) {
            Announce.transform.Find("announce").GetComponent<TMP_Text>().text = NEED_PIECE;
            Announce.SetActive(true);
            return;
        }
        //Debug.Log(unit_id + " > " + need_gold + " + " + need_piece);

        // 가능하면 강화
        user.gold -= need_gold;
        _unit.piece -= need_piece;
        _unit.upgrade = upgrade;

        // 업적 횟수추가
        Achievement_Observer.Instance.Use_Gold_Achievement(need_gold);
        Achievement_Observer.Instance.Upgrade_Achievement();

        // modify db
        Utility.builder.Clear();
        Utility.builder.Append("UPDATE user SET gold=").Append(user.gold);
        ModifyDB.Instance.ModifySet(Utility.builder.ToString(), "user");
        Utility.builder.Clear();

        // piece / upgrade
        Utility.builder.Append("UPDATE unit SET piece=").Append(_unit.piece).Append(", upgrade=").Append(_unit.upgrade).Append(" WHERE id=").Append(_unit.id);
        ModifyDB.Instance.ModifySet(Utility.builder.ToString(), "unit");
        Utility.builder.Clear();
        // 1~2초 지연 - > 강화 애니메이션 실행
        // page reset
        Unit_Page_Book_Setter.Instance.Init();
        Unit_Page_Unit_Info_Setter.Instance.Init();

        StartCoroutine(Ani_Active());
    }

    IEnumerator Ani_Active() {
        // target image => y 185 => -500
        Unit_Page_Upgrade_Activator.Instance.Image_upgrade_active();
        Unit_Page_Upgrade_Activator.Instance.Window_deactive();
        // 요 사이에 setting resetting
        yield return wfs_0_5;
        StartCoroutine(Particle_Start());
    }

    IEnumerator Particle_Start() {
        Unit_Upgrade_Page_Setter.Instance.Setting_Before_Upgrade(-1);
        upgrade_particle.SetActive(true);
        yield return wfs_1;
        StartCoroutine(Particle_end());
    }

    IEnumerator Particle_end() {
        //upgrade_particle.GetComponent<ParticleSystem>().Play();
        yield return wfs_1;
        // particle off
        upgrade_particle.SetActive(false);
        Unit_Page_Upgrade_Activator.Instance.Image_upgrade_deactive();
        yield return wfs_0_5;
        Unit_Page_Upgrade_Activator.Instance.Window_active();
    }


    public void Annoucen_Check() {
        Announce.SetActive(false);
    }
}
