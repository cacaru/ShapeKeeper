using UnityEngine;
using UnityEngine.UI;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Game_Value_Data;
using CUSTOM_DATA;
using TMPro;

public class Field_Unit_Click_Setter : Scene_Singleton<Field_Unit_Click_Setter> {

    public GameObject target;
    public void Set_Info(int unit_id, GameObject target, int tower_id) {
        this.target = target;
        var tower = installed_tower[tower_id];
        Unit _unit = unit[unit_id];

        transform.Find("Combine").GetComponent<Button>().onClick.RemoveAllListeners();
        transform.Find("Recall").GetComponent<Button>().onClick.RemoveAllListeners();
        transform.Find("Changer").GetComponent<Button>().onClick.RemoveAllListeners();

        transform.Find("Image").GetComponent<Image>().sprite = Utility.Get_sprite(unit_id);
        transform.Find("Image").GetComponent<Image>().color = Utility.Get_Type_Color(unit_id);
        GetComponent<Image>().color = Utility.Get_Grade_Color(unit_id);

        // 공격비 계산
        int figure = _unit.grade switch {
            "d" => D_UPGRADE_FIGURE * upgrade_value_d,
            "c" => C_UPGRADE_FIGURE * upgrade_value_c,
            "b" => B_UPGRADE_FIGURE * upgrade_value_b,
            "a" => A_UPGRADE_FIGURE * upgrade_value_a,
            "s" => S_UPGRADE_FIGURE * upgrade_value_s,
            "ex" => EX_UPGRADE_FIGURE * upgrade_value_ex,
            _ => 0
        };

        float temp_value = tower.attack + figure;
        int skill_value = user.GetSkillValue(SkillType.AttackIncrease);
        float now_attack = skill_value > 0 ? temp_value + temp_value * skill_value / 100 : temp_value;

        float material_attack = tower.material > 0 ? (now_attack * tower.material / 100) : 0;
        float ethereal_attack = tower.ethereal > 0 ? (now_attack * tower.ethereal / 100) : 0;
        transform.Find("Material_Attack").GetComponent<TMP_Text>().text = material_attack.ToString("0.#");
        transform.Find("Ethereal_Attack").GetComponent<TMP_Text>().text = ethereal_attack.ToString("0.#");
        transform.Find("Speed").GetComponent<TMP_Text>().text = _unit.speed.ToString();

        // 조합법의 모든 버튼 listener를 빼고 / listener를 붙이기
        transform.Find("Combine").GetComponent<Button>().onClick.AddListener( () => {
            Combine_Observer.Instance.Set_Combine_Field_from_Field_Click(unit_id);
            Pannel_Controller.Instance.Activate_for_Combine();
            Pannel_Controller.Instance.Deactivate_for_Field_Unit_Click();
        });

        // 회수하기의 모든 listener를 빼고 붙이기
        transform.Find("Recall").GetComponent<Button>().onClick.AddListener(() => {
            // to-do 회수 확인 창 => 옵션으로 키고 끄게 만들기 
            // 비용 확인 
            Recall.Instance.Recall_Unit(unit_id, target, tower_id);
            Pannel_Controller.Instance.Deactivate_for_Field_Unit_Click();
        });

        // 별 달 일 경우 Changer 활성화
        if (Utility.Is_Special(unit_id)) {
            transform.Find("Changer").gameObject.SetActive(true);
            transform.Find("Changer").GetComponent<Button>().onClick.AddListener(() => {
                // 공격 타입 전환하기
                target.GetComponent<Special_Unit_Type_Changer>().tower_id = tower_id;
                target.GetComponent<Special_Unit_Type_Changer>().Change_Attack();
            });
        }
        else {
            transform.Find("Changer").gameObject.SetActive(false);
        }
    }
}
