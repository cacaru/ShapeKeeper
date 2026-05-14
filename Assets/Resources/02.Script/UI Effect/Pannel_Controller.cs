using UnityEngine;
using CUSTOM_DATA;
using System.Collections.Generic;
using static CUSTOM_DATA.Game_Value_Data;
using static CUSTOM_DATA.Game_State_Data;
using TMPro;
using UnityEngine.EventSystems;

// 스크립트에서 패널에 접근하기 위해 작성함
public class Pannel_Controller : Scene_Singleton<Pannel_Controller>, IPointerClickHandler
{
    private readonly Dictionary<UIPanelID, Animator> pannel_controller = new();

    private bool field_unit_click_pannel_active = false;

    private bool summon_btn_toggle_state = false;
    private bool mission_btn_toggle_state = false;
    private bool upgrade_btn_toggle_state = false;
    private bool combine_book_toggle_state = false;
    private bool spell_toggle_state = false;

    private bool btn_area_toggle_state = true;
    
    void Start()
    {
        // summon btn area
        // install area
        // btn area
        // combine area
        // unit info area
        // upgrade_area,
        // field_unit_click_area,
        // announce_area,
        // round_check_area,
        // combine_book_area,
        // mission_area,
        //  현재 구현된 영역 가져오기
        foreach (Transform child in transform) {
            if (child.name.Contains("Area")) {
                var value = Utility.Get_Pannel_ID(child.name.ToLower());
                //Debug.Log(value);
                pannel_controller.Add(value, child.gameObject.GetComponent<Animator>());
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        var now_target = eventData.pointerCurrentRaycast.gameObject.name;
        
        if (now_target.Equals("Canvas_") && !is_click_unit) {
            bool someting_closed = false;
            // 무엇이 열려있뜬 일단 모든걸 닫아보자
            foreach (var child in pannel_controller) {
                if (child.Value.GetBool(ANI_ACTIVATE)) {
                    child.Value.SetBool(ANI_ACTIVATE, false);
                    if(child.Key != UIPanelID.ButtonArea) someting_closed = true;
                }
            }
            // 모든 패널이 닫힌 상태에선 btn area를 toggle
            if(!someting_closed) {
                btn_area_toggle_state = !btn_area_toggle_state;
                pannel_controller[UIPanelID.ButtonArea].SetBool(ANI_ACTIVATE, btn_area_toggle_state);
            }
            else {
                // 그리고 버튼 에어리어를 toggle
                pannel_controller[UIPanelID.ButtonArea].SetBool(ANI_ACTIVATE, true);
                btn_area_toggle_state = true;
                // 버튼 상태를 닫기와 동일하게 만들어야함
                // 현재 btn_toggle이 true라면 false로 변경해줘야함
                Btn_Controller.Instance.Deactive_All();
                summon_btn_toggle_state = false;
                mission_btn_toggle_state = false;
                upgrade_btn_toggle_state = false;
                combine_book_toggle_state = false;
                field_unit_click_pannel_active = false;
                spell_toggle_state = false;
            }
        }
        if (!is_click_unit) {
            Area_Pool.Instance.Return();
        }
        is_click_unit = false;
    }

    public void Activate_for_Install() {
        // 현재 activate ==ture 인 상태를 false로 바꿔야함 (모든 패널)
        foreach(var child in pannel_controller) {
            if (child.Value.GetBool(ANI_ACTIVATE)) {
                child.Value.SetBool(ANI_ACTIVATE, false);
            }
        }
        field_unit_click_pannel_active = false;
        Area_Pool.Instance.Return();
    }
    // --------------------------------- toggle ---------------------------------------

    public void Toggle_Summon_Btn() {
        summon_btn_toggle_state = !summon_btn_toggle_state;

        pannel_controller[UIPanelID.SummonButtonArea].SetBool(ANI_ACTIVATE, summon_btn_toggle_state);
        pannel_controller[UIPanelID.InstallArea].SetBool(ANI_ACTIVATE, summon_btn_toggle_state);
        pannel_controller[UIPanelID.ButtonArea].SetBool(ANI_ACTIVATE, summon_btn_toggle_state);

        if (summon_btn_toggle_state == false) {
            pannel_controller[UIPanelID.ButtonArea].SetBool(ANI_ACTIVATE, true);
            pannel_controller[UIPanelID.UnitInfoArea].SetBool(ANI_ACTIVATE, false);
        }
    }

    public void Toggle_Mission_Btn() {
        mission_btn_toggle_state = !mission_btn_toggle_state;

        pannel_controller[UIPanelID.MissionArea].SetBool(ANI_ACTIVATE, mission_btn_toggle_state);
    }

    public void Toggle_Upgrade_Btn() {
        upgrade_btn_toggle_state = !upgrade_btn_toggle_state;

        pannel_controller[UIPanelID.UpgradeArea].SetBool(ANI_ACTIVATE, upgrade_btn_toggle_state);
    }

    public void Toggle_Combine_Btn() {
        combine_book_toggle_state = !combine_book_toggle_state;

        pannel_controller[UIPanelID.CombineBookArea].SetBool(ANI_ACTIVATE, combine_book_toggle_state);
        // 조합식 창이 열려있을지도 모르니 그것도 닫기
        if (pannel_controller[UIPanelID.CombineArea].GetBool(ANI_ACTIVATE)) {
            pannel_controller[UIPanelID.CombineArea].SetBool(ANI_ACTIVATE, false);
        }

        // 필드 유닛 선택 창도 닫기
        if (pannel_controller[UIPanelID.FieldUnitClickArea].GetBool(ANI_ACTIVATE)) {
            Area_Pool.Instance.Return();
            pannel_controller[UIPanelID.FieldUnitClickArea].SetBool(ANI_ACTIVATE, false);
        }
    }

    public void Toggle_Spell_btn() {
        spell_toggle_state = !spell_toggle_state;
        // 조합식 창이 열려있을지도 모르니 그것도 닫기
        if (pannel_controller[UIPanelID.CombineArea].GetBool(ANI_ACTIVATE)) {
            pannel_controller[UIPanelID.CombineArea].SetBool(ANI_ACTIVATE, false);
        }

        // 필드 유닛 선택 창도 닫기
        if (pannel_controller[UIPanelID.FieldUnitClickArea].GetBool(ANI_ACTIVATE)) {
            Area_Pool.Instance.Return();
            pannel_controller[UIPanelID.FieldUnitClickArea].SetBool(ANI_ACTIVATE, false);
        }
        // spell pannel toggle
        pannel_controller[UIPanelID.SpellArea].SetBool(ANI_ACTIVATE, spell_toggle_state);
    }

    // -------------------------------------------------------------------------------
    public void Deactivate_for_Install() {
        // summon btn area와 install area를 다시 펼쳐줌( 조합법과 상세는 펼치지 않음 )
        pannel_controller[UIPanelID.SummonButtonArea].SetBool(ANI_ACTIVATE, true);
        pannel_controller[UIPanelID.InstallArea].SetBool(ANI_ACTIVATE, true);
        pannel_controller[UIPanelID.ButtonArea].SetBool(ANI_ACTIVATE, true);
    }
    public void Deactivate_for_Spell_Install() {
        // spell area를 다시 보여줌
        pannel_controller[UIPanelID.SpellArea].SetBool(ANI_ACTIVATE, true);
        pannel_controller[UIPanelID.ButtonArea].SetBool(ANI_ACTIVATE, true);
    }

    public void Activate_for_Combine() {
        // 조합식 패널 불러오기
        pannel_controller[UIPanelID.CombineArea].SetBool(ANI_ACTIVATE, true);
    }

    public void Deactivate_for_Combine() {
        pannel_controller[UIPanelID.CombineArea].SetBool(ANI_ACTIVATE, false);
    }

    public void Activate_for_UnitInfo() {
        pannel_controller[UIPanelID.UnitInfoArea].SetBool(ANI_ACTIVATE, true);
    }

    public void Deactivate_for_UnitInfo() {
        pannel_controller[UIPanelID.UnitInfoArea].SetBool(ANI_ACTIVATE, false);
    }

    public void Activate_for_Field_Unit_Click() {
        field_unit_click_pannel_active = true;
        pannel_controller[UIPanelID.FieldUnitClickArea].SetBool(ANI_ACTIVATE, true);
    }

    public void Deactivate_for_Field_Unit_Click() {
        field_unit_click_pannel_active = false;
        Area_Pool.Instance.Return();
        pannel_controller[UIPanelID.FieldUnitClickArea].SetBool(ANI_ACTIVATE, false);
    }


    public bool Get_Field_Unit_Pannel_Activate() {
        return field_unit_click_pannel_active;
    }
    public bool Get_Field_Book_Pannel_Activate() {
        return pannel_controller[UIPanelID.CombineBookArea].GetBool(ANI_ACTIVATE);
    }
    public bool Get_Field_Summon_Pannel_Activate() {
        return pannel_controller[UIPanelID.SummonButtonArea].GetBool(ANI_ACTIVATE);
    }
    public bool Get_Field_Combine_Pannel_Activate() {
        return pannel_controller[UIPanelID.CombineArea].GetBool(ANI_ACTIVATE);
    }
    public bool Get_Field_Mission_Pannel_Activate() {
        return pannel_controller[UIPanelID.MissionArea].GetBool(ANI_ACTIVATE);
    }
    public bool Get_Field_Upgrade_Pannel_Activate() {
        return pannel_controller[UIPanelID.UpgradeArea].GetBool(ANI_ACTIVATE);
    }
    public bool Get_Field_Spell_Pannel_Activate() {
        return pannel_controller[UIPanelID.SpellArea].GetBool(ANI_ACTIVATE);
    }


    public void Activate_for_Announce(string txt) {
        pannel_controller[UIPanelID.AnnounceArea].gameObject.transform.Find("announce").GetComponent<TMP_Text>().text = txt;
        pannel_controller[UIPanelID.AnnounceArea].SetBool(ANI_ACTIVATE, true);
    }

    public void Deactivate_for_Annoucne() {
        pannel_controller[UIPanelID.AnnounceArea].SetBool(ANI_ACTIVATE, false);
    }

    public bool Activate_check() {
        foreach (var child in pannel_controller) {
            if (child.Value.GetBool(ANI_ACTIVATE)) {
                return true;
            }
        }
        return false;
    }
}
