using UnityEngine;
using CUSTOM_DATA;
using System.Collections.Generic;

public class Btn_Controller : Scene_Singleton<Btn_Controller>
{
    [SerializeField] private GameObject mission_btn;
    [SerializeField] private GameObject skip_btn;
    // 3종류의 버튼 ui를 컨트롤 하기 위해 만듬
    // 각 버튼이 눌릴 때 마다 다른 두 버튼은 좌측으로 사라져있다가
    // 닫기 를 누름으로써 원래대로 돌아옴
    // 눌린 버튼은 위로 움직임

    private Dictionary<string, GameObject> btn_controller = new();
    private bool target_state = false;
    public bool is_skipping = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform child in transform) {
            btn_controller.Add(child.name.ToLower(), child.gameObject);
        }
        btn_controller.Add("mission", mission_btn);
        btn_controller.Add("skip", skip_btn);

    }

    public void Toggle(string target) {
        target_state = !target_state;
        // 토글된 본인은 activate 상태를 target_state
        // 나머지 다른 버튼은 passively_activate 를 true로

        // target_state가 false가 되면 
        // anote
        foreach (var btn in btn_controller) {
            if (btn.Key.Equals(target)) {
                btn.Value.GetComponent<Animator>().SetBool(Game_Value_Data.ANI_ACTIVATE, target_state);
                btn.Value.GetComponent<Animator>().SetBool(Game_Value_Data.ANI_Passively_ACTIVATE, false);
                if (btn.Value.TryGetComponent<Btn_Toggle>(out var togller)) {
                    togller.Toggle(target_state);
                }
                //btn.Value.GetComponent<Btn_Toggle>().Toggle(target_state);
            }
            else {
                btn.Value.GetComponent<Animator>().SetBool(Game_Value_Data.ANI_ACTIVATE, !target_state);
                btn.Value.GetComponent<Animator>().SetBool(Game_Value_Data.ANI_Passively_ACTIVATE, target_state);
                if (is_skipping && btn.Key.Equals("skip")) { btn.Value.GetComponent<Animator>().SetBool(Game_Value_Data.ANI_Passively_ACTIVATE, true); }
                if (btn.Value.TryGetComponent<Btn_Toggle>(out var togller)) {
                    togller.Toggle(false);
                }
                //btn.Value.GetComponent<Btn_Toggle>().Toggle(false);
            }
        }

        // target이 summon 이면 combine pannel도 닫아줘야함
        if (target.Equals("summon")) {
            Pannel_Controller.Instance.Deactivate_for_Combine();
        }
    }
    private bool mission_toggle_state = false;
    public void Mission_Toggling() {
        mission_toggle_state = !mission_toggle_state;

        mission_btn.GetComponent<Animator>().SetBool(Game_Value_Data.ANI_ACTIVATE, mission_toggle_state);
        mission_btn.GetComponent<Btn_Toggle>().Toggle(mission_toggle_state);
    }

    public void Deactive_All() {
        target_state = false;
        foreach(var btn in btn_controller) {
            btn.Value.GetComponent<Animator>().SetBool(Game_Value_Data.ANI_ACTIVATE, false);
            if(is_skipping && btn.Key.Equals("skip")) {
               // dont change animate state 
            }
            else {
                btn.Value.GetComponent<Animator>().SetBool(Game_Value_Data.ANI_Passively_ACTIVATE, false);
            }
            
            if (btn.Value.TryGetComponent<Btn_Toggle>(out var togller)) {
                togller.Toggle(false);
            }
        }
    }


    public void Active_Skip_Btn() {
        is_skipping = true;
        btn_controller["skip"].GetComponent<Animator>().SetBool(Game_Value_Data.ANI_Passively_ACTIVATE, true);
    }

    public void Deactive_Skip_Btn() {
        is_skipping = false;
        btn_controller["skip"].GetComponent<Animator>().SetBool(Game_Value_Data.ANI_Passively_ACTIVATE, false);
    }

}
