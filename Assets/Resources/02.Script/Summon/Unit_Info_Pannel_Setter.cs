using UnityEngine;
using UnityEngine.UI;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Game_Value_Data;
using static CUSTOM_DATA.Common_Data;
using CUSTOM_DATA;
using TMPro;

public class Unit_Info_Pannel_Setter : Scene_Singleton<Unit_Info_Pannel_Setter>
{
    public void Set_Info(int unit_id) {
        Unit _unit = unit[unit_id];
        transform.Find("Image").GetComponent<Image>().sprite = Utility.Get_sprite(unit_id);
        transform.Find("Image").GetComponent<Image>().color = Utility.Get_Type_Color(unit_id);
        GetComponent<Image>().color = Utility.Get_Grade_Color(unit_id);

        transform.Find("Field").GetComponent<Image>().sprite = _unit.type.Split("_")[1] switch {
            "0" => unit_field_icon_0,
            "1" => unit_field_icon_4,
            "2" => unit_field_icon_2v,
            "3" => unit_field_icon_2h,
            "4" => unit_field_icon_6h,
            "5" => unit_field_icon_6v,
            _ => unit_field_icon_4
        };

        transform.Find("Type").GetComponent<TMP_Text>().text = Utility.Get_Type_String(_unit.type);
        transform.Find("Type").GetComponent<TMP_Text>().color = Utility.Get_Type_Color(unit_id);

        // 공격비 계산
        int figure = _unit.grade switch {
            "d" => D_UPGRADE_FIGURE * (upgrade_value_d > 0 ? upgrade_value_d : 1),
            "c" => C_UPGRADE_FIGURE * (upgrade_value_c > 0 ? upgrade_value_c : 1),
            "b" => B_UPGRADE_FIGURE * (upgrade_value_b > 0 ? upgrade_value_b : 1),
            "a" => A_UPGRADE_FIGURE * (upgrade_value_a > 0 ? upgrade_value_a : 1),
            "s" => S_UPGRADE_FIGURE * (upgrade_value_s > 0 ? upgrade_value_s : 1),
            "ex" => EX_UPGRADE_FIGURE * (upgrade_value_ex > 0 ? upgrade_value_ex : 1),
            _ => 0
        };

        float material_attack = _unit.material > 0 ? ((_unit.attack + figure) * _unit.material / 100) : 0;
        float ethereal_attack = _unit.ethereal > 0 ? ((_unit.attack + figure) * _unit.ethereal / 100) : 0;
        transform.Find("Material_Attack").GetComponent<TMP_Text>().text = material_attack.ToString();
        transform.Find("Ethereal_Attack").GetComponent<TMP_Text>().text = ethereal_attack.ToString();
        transform.Find("Speed").GetComponent<TMP_Text>().text = _unit.speed.ToString();
    }
}
