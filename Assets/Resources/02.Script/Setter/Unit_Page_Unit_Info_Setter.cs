using CUSTOM_DATA;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Game_Value_Data;
using static CUSTOM_DATA.Home_UI_Controller;


public class Unit_Page_Unit_Info_Setter : Scene_Singleton<Unit_Page_Unit_Info_Setter>, IPointerClickHandler 
{
    [SerializeField] private Transform unit_info;

    public void OnPointerClick(PointerEventData eventData) {
        int id = int.Parse(eventData.pointerCurrentRaycast.gameObject.name);
        
        if(id < 2000) return;
        now_info_unit_id = id;

        Init();

        // 기존 창 밀고
        Unit_Page_Setter.Instance.Deactive();
        Footer_Setter.Instance.Deactive();
        // unit_info 창 열기
        Unit_Page_Info_Activater.Instance.Active();
    }

    public void Init() {
        int id = now_info_unit_id;
        // setting
        var _unit = unit[id];
        unit_info.Find("unit").GetComponent<Image>().sprite = Utility.Get_sprite(id);
        unit_info.Find("unit").GetComponent<Image>().color = Utility.Get_Type_Color(id);
        var color = Utility.Get_Grade_Color(id);
        color.a = 80 / 255f;
        unit_info.gameObject.GetComponent<Image>().color = color;

        if (id > 5000) {
            unit_info.Find("satellite_1").GetComponent<Image>().sprite = Utility.Get_Satellite_sprite(id);
            unit_info.Find("satellite_1").gameObject.SetActive(true);
        }
        else {
            unit_info.Find("satellite_1").gameObject.SetActive(false);
        }
        if (id > 6000 && id < 7000) {
            unit_info.Find("satellite_2").GetComponent<Image>().sprite = Utility.Get_Satellite_sprite(id);
            unit_info.Find("satellite_2").gameObject.SetActive(true);
        }
        else {
            unit_info.Find("satellite_2").gameObject.SetActive(false);
        }

        unit_info.Find("field").GetComponent<Image>().sprite = Utility.Get_Field_Type(id);
        unit_info.Find("name").GetComponent<TMP_Text>().text = _unit.nick_name;
        unit_info.Find("type").GetComponent<TMP_Text>().text = Utility.Get_Type_String(_unit.type); 

        unit_info.Find("speed").GetComponent<TMP_Text>().text = _unit.speed.ToString();
        int figure = _unit.grade switch {
            "d" => D_UPGRADE_FIGURE,
            "c" => C_UPGRADE_FIGURE,
            "b" => B_UPGRADE_FIGURE,
            "a" => A_UPGRADE_FIGURE,
            "s" => S_UPGRADE_FIGURE,
            "ex" => EX_UPGRADE_FIGURE,
            _ => 0
        };
        float material_attack = _unit.material > 0 ? ((_unit.attack + _unit.upgrade * figure) * _unit.material / 100) : 0;
        float ethereal_attack = _unit.ethereal > 0 ? ((_unit.attack + _unit.upgrade * figure) * _unit.ethereal / 100) : 0;
        unit_info.Find("material").GetComponent<TMP_Text>().text = material_attack.ToString("N1");
        unit_info.Find("ethereal").GetComponent<TMP_Text>().text = ethereal_attack.ToString("N1");

        unit_info.Find("piece").GetComponent<TMP_Text>().text = _unit.piece.ToString();

        int upgrade_cost_figure = _unit.grade switch {
            "d" => D_UPGRADE_GOLD_COEFFICENT,
            "c" => C_UPGRADE_GOLD_COEFFICENT,
            "b" => B_UPGRADE_GOLD_COEFFICENT,
            "a" => A_UPGRADE_GOLD_COEFFICENT,
            "s" => S_UPGRADE_GOLD_COEFFICENT,
            "ex" => EX_UPGRADE_GOLD_COEFFICENT,
            _ => 0
        };
        //unit_info.Find("cost").GetComponent<TMP_Text>().text = ((_unit.upgrade + 1) * upgrade_cost_figure).ToString();
        unit_info.Find("upgrade").GetComponent<TMP_Text>().text = _unit.upgrade.ToString() + "강";

        unit_info.Find("Upgrade").GetComponent<Button>().onClick.RemoveAllListeners();
        unit_info.Find("Upgrade").GetComponent<Button>().onClick.AddListener(() => {
            Unit_Upgrade_Page_Setter.Instance.Setting_Before_Upgrade(id);
        });
    }
}
