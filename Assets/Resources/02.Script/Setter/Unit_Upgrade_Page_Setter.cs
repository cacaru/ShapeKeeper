using UnityEngine;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Game_Value_Data;
using CUSTOM_DATA;
using TMPro;
using UnityEngine.UI;

public class Unit_Upgrade_Page_Setter : Scene_Singleton<Unit_Upgrade_Page_Setter>
{
    
    [SerializeField] private Transform unit_info;

    private Unit _unit;
    private int unit_id = -1;
    private int upgrade_value;

    public void Setting_Before_Upgrade(int _unit_id) {
        if(_unit_id > 0) {
            unit_id = _unit_id;
        }
        else {
            if(unit_id < 0) {
                return;
            }
        }

        _unit = unit[unit_id];

        unit_info.Find("Unit").Find("unit").GetComponent<Image>().sprite = Utility.Get_sprite(unit_id);
        unit_info.Find("Unit").Find("unit").GetComponent<Image>().color = Utility.Get_Type_Color(unit_id);
        var color = Utility.Get_Grade_Color(unit_id);
        color.a = 80 / 255f;
        unit_info.Find("In_Window").GetComponent<Image>().color = color;

        if (unit_id > 5000) {
            unit_info.Find("Unit").Find("satellite_1").GetComponent<Image>().sprite = Utility.Get_Satellite_sprite(unit_id);
            unit_info.Find("Unit").Find("satellite_1").gameObject.SetActive(true);
        }
        else {
            unit_info.Find("Unit").Find("satellite_1").gameObject.SetActive(false);
        }
        if (unit_id > 6000 && unit_id < 7000) {
            unit_info.Find("Unit").Find("satellite_2").GetComponent<Image>().sprite = Utility.Get_Satellite_sprite(unit_id);
            unit_info.Find("Unit").Find("satellite_2").gameObject.SetActive(true);
        }
        else {
            unit_info.Find("Unit").Find("satellite_2").gameObject.SetActive(false);
        }

        unit_info.Find("In_Window").Find("name").GetComponent<TMP_Text>().text = _unit.nick_name;

        unit_info.Find("In_Window").Find("piece").GetComponent<TMP_Text>().text = _unit.piece.ToString();
        upgrade_value = _unit.upgrade + 1;
        Now_Upgrade_Value();
        unit_info.Find("In_Window").Find("downgrade_btn").gameObject.SetActive(false);

        Upgrade.Instance.unit_id = unit_id;
        Upgrade.Instance.upgrade = upgrade_value;

        if(_unit_id > 0) {
            // 기존 창 밀고
            Unit_Page_Info_Activater.Instance.Deactive(false);
            // unit_info 창 열기
            Unit_Page_Upgrade_Activator.Instance.Active();
        }
    }

    public void Now_Upgrade_Value() {

        int figure = _unit.grade switch {
            "d" => D_UPGRADE_FIGURE,
            "c" => C_UPGRADE_FIGURE,
            "b" => B_UPGRADE_FIGURE,
            "a" => A_UPGRADE_FIGURE,
            "s" => S_UPGRADE_FIGURE,
            "ex" => EX_UPGRADE_FIGURE,
            _ => 0
        };

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

        float material_attack = _unit.material > 0 ? ((_unit.attack + _unit.upgrade * figure) * _unit.material / 100) : 0;
        float ethereal_attack = _unit.ethereal > 0 ? ((_unit.attack + _unit.upgrade * figure) * _unit.ethereal / 100) : 0;
        unit_info.Find("In_Window").Find("material_before").GetComponent<TMP_Text>().text = material_attack.ToString();
        unit_info.Find("In_Window").Find("ethereal_before").GetComponent<TMP_Text>().text = ethereal_attack.ToString();

        material_attack = _unit.material > 0 ? ((_unit.attack + upgrade_value * figure) * _unit.material / 100) : 0;
        ethereal_attack = _unit.ethereal > 0 ? ((_unit.attack + upgrade_value * figure) * _unit.ethereal / 100) : 0;
        unit_info.Find("In_Window").Find("material_after").GetComponent<TMP_Text>().text = material_attack.ToString();
        unit_info.Find("In_Window").Find("ethereal_after").GetComponent<TMP_Text>().text = ethereal_attack.ToString();

        unit_info.Find("In_Window").Find("need_piece").GetComponent<TMP_Text>().text = (upgrade_piece_figure * upgrade_value ).ToString();

        unit_info.Find("In_Window").Find("cost").GetComponent<TMP_Text>().text = (upgrade_value * upgrade_cost_figure).ToString();
        unit_info.Find("In_Window").Find("upgrade").GetComponent<TMP_Text>().text = upgrade_value.ToString() + "강";
    }


    public void Upgrade_Value_Set() {
        upgrade_value++;
        if (upgrade_value >= 15) {
            unit_info.Find("In_Window").Find("upgrade_btn").gameObject.SetActive(false);
            upgrade_value = 15;
        }
        else {
            unit_info.Find("In_Window").Find("upgrade_btn").gameObject.SetActive(true);
        }
        unit_info.Find("In_Window").Find("downgrade_btn").gameObject.SetActive(true);
        Upgrade.Instance.upgrade = upgrade_value;
        Now_Upgrade_Value();
    }

    public void Downgrade_Value_Set() {
        upgrade_value--;
        if(upgrade_value <= _unit.upgrade + 1) {
            unit_info.Find("In_Window").Find("downgrade_btn").gameObject.SetActive(false);
            upgrade_value = _unit.upgrade + 1;
        }
        else {
            unit_info.Find("In_Window").Find("downgrade_btn").gameObject.SetActive(true);
        }
        unit_info.Find("In_Window").Find("upgrade_btn").gameObject.SetActive(true);
        Upgrade.Instance.upgrade = upgrade_value;
        Now_Upgrade_Value();
    }
}
