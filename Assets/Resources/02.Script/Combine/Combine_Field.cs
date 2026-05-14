using UnityEngine;
using static CUSTOM_DATA.Game_State_Data;

public class Combine_Field : MonoBehaviour
{
    public int id = 0;
    public int tower_id = -1;


    private void OnMouseDown() {

        is_click_unit = true;
        /*
        // x : 1410 ~ 390 
        // y : 2934.2 ~ 2334.2
        // info pannel active 상태 확인
        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;
        
        bool under_pannel = false;
        if (x >= 390 && x <= 1410 && y >= 1600f && y <= 1950f) {
            under_pannel = true;
        }
        */
        if (Pannel_Controller.Instance.Get_Field_Unit_Pannel_Activate()) return;
        if (Pannel_Controller.Instance.Get_Field_Book_Pannel_Activate()) return;
        if (Pannel_Controller.Instance.Get_Field_Summon_Pannel_Activate()) return;
        if (Pannel_Controller.Instance.Get_Field_Combine_Pannel_Activate()) return;
        if (Pannel_Controller.Instance.Get_Field_Mission_Pannel_Activate()) return;
        if (Pannel_Controller.Instance.Get_Field_Upgrade_Pannel_Activate()) return;
        if (Pannel_Controller.Instance.Get_Field_Spell_Pannel_Activate()) return;
        if (In_Game_Option_Checker.Instance.Active) return;
        if (Game_End.Instance.Is_End) return;

        // 아래의 일들은 그 화면에서 버튼으로 조작
        Field_Unit_Click_Setter.Instance.Set_Info(id, gameObject, tower_id);
        Pannel_Controller.Instance.Activate_for_Field_Unit_Click();

        // 공격 범위 보이기
        Area_Pool.Instance.Activate(id, transform.position);
    }
}
