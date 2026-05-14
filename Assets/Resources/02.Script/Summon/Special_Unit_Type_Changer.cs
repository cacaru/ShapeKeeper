using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Common_Data;
using UnityEngine;

public class Special_Unit_Type_Changer : MonoBehaviour
{
    // 별 달 상태에서 클릭 시 material - ethereal 공격력 변환
    // unit[]의 별 달 위치에 맞는 유닛의 공격력을 전환
    // color도 변경
    public int tower_id = -1;

    public void Change_Attack() {
        if (tower_id < 0) return;

        var tower = installed_tower[tower_id];
        if(tower.material > 0) {
            tower.ethereal = tower.material;
            tower.material = 0;
            // ethereal 색 설정
            transform.Find("Ori").GetComponent<SpriteRenderer>().color = special_ethereal_color;
            var statellite_1 = transform.Find("Satellite");
            if(statellite_1 != null) {
                statellite_1.GetComponent<SpriteRenderer>().color = special_ethereal_color;
            }
            var statellite_2 = transform.Find("Satellite_2");
            if(statellite_2 != null) {
                statellite_2.GetComponent<SpriteRenderer>().color= special_ethereal_color;
            }
        }
        else {
            tower.material = tower.ethereal;
            tower.ethereal = 0;

            // material 색 설정
            transform.Find("Ori").GetComponent<SpriteRenderer>().color = special_material_color;
            var statellite_1 = transform.Find("Satellite");
            if (statellite_1 != null) {
                statellite_1.GetComponent<SpriteRenderer>().color = special_material_color;
            }
            var statellite_2 = transform.Find("Satellite_2");
            if (statellite_2 != null) {
                statellite_2.GetComponent<SpriteRenderer>().color = special_material_color;
            }
        }

        // 전환된 공격력 리세팅
        GetComponent<Attack>().Reset_Attack();

        Pannel_Controller.Instance.Deactivate_for_Field_Unit_Click();

        
    }
}
