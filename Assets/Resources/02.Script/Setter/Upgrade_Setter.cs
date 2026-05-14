using TMPro;
using UnityEngine;
using static CUSTOM_DATA.Game_Data;

public class Upgrade_Setter : Scene_Singleton<Upgrade_Setter>
{
    [SerializeField] private TMP_Text d_value;
    [SerializeField] private TMP_Text c_value;
    [SerializeField] private TMP_Text b_value;
    [SerializeField] private TMP_Text a_value;
    [SerializeField] private TMP_Text s_value;
    [SerializeField] private TMP_Text ex_value;

    [SerializeField] private TMP_Text d_need_value;
    [SerializeField] private TMP_Text c_need_value;
    [SerializeField] private TMP_Text b_need_value;
    [SerializeField] private TMP_Text a_need_value;
    [SerializeField] private TMP_Text s_need_value;
    [SerializeField] private TMP_Text ex_need_value;


    public void Setting() {
        d_value.text = upgrade_value_d.ToString();
        c_value.text = upgrade_value_c.ToString();
        b_value.text = upgrade_value_b.ToString();
        a_value.text = upgrade_value_a.ToString();
        s_value.text = upgrade_value_s.ToString();
        ex_value.text = upgrade_value_ex.ToString();
    }

    public void Upgrade_Grade(string grade) {
        int target_upgrade_value;
        TMP_Text target_value;
        TMP_Text target_need_value;

        switch (grade) {
            case "d":
                target_need_value = d_need_value;
                target_value = d_value;
                target_upgrade_value = upgrade_value_d;
                break;
            case "c":
                target_need_value = c_need_value;
                target_value = c_value;
                target_upgrade_value = upgrade_value_c;
                break;
            case "b":
                target_need_value = b_need_value;
                target_value = b_value;
                target_upgrade_value = upgrade_value_b;
                break;
            case "a":
                target_need_value = a_need_value;
                target_value = a_value;
                target_upgrade_value = upgrade_value_a;
                break;
            case "s":
                target_need_value = s_need_value;
                target_value = s_value;
                target_upgrade_value = upgrade_value_s;
                break;
            case "ex":
                target_need_value = ex_need_value;
                target_value = ex_value;
                target_upgrade_value = upgrade_value_ex;
                break;
            default:
                return;
        }

        // 이미 맥스인지 확인
        if (target_upgrade_value >= 10) return;
        int need_value = int.Parse(target_need_value.text);
        if (dot < need_value) return;
        dot -= need_value;
        target_need_value.text = (need_value + 50).ToString();
        target_upgrade_value += 1;

        if (target_upgrade_value >= 10) {
            target_upgrade_value = 10;
            target_value.text = "MAX";
        }
        else {
            target_value.text = target_upgrade_value.ToString();
        }

        switch (grade) {
            case "d":
                upgrade_value_d = target_upgrade_value;
                break;
            case "c":
                upgrade_value_c = target_upgrade_value;
                break;
            case "b":
                upgrade_value_b = target_upgrade_value;
                break;
            case "a":
                upgrade_value_a = target_upgrade_value;
                break;
            case "s":
                upgrade_value_s = target_upgrade_value;
                break;
            case "ex":
                upgrade_value_ex = target_upgrade_value;
                break;
        }

        // 업적 추가
        Achievement_Observer.Instance.Use_Dot_Achievement(need_value);

        // 업그레이드가 완료되면 현 재화를 다시 설정
        In_Game_Setter.Instance.Set_Goods();
        int counter = 0;
        // 현재 필드의 유닛들의 공격력을 재설정해야함
        foreach(var unit in installed_tower) {
            if(unit.Value.me != null) {
                counter++;
                unit.Value.me.GetComponent<Attack>().Reset_Attack();
            }
        }

        //Debug.Log(counter + " . " + installed_tower.Count);
    }

}
