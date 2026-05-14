using CUSTOM_DATA;
using UnityEngine;
using static CUSTOM_DATA.Game_State_Data;
using static CUSTOM_DATA.Common_Data;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Stage_Selecter : Scene_Singleton<Stage_Selecter>
{
    private GameObject now_selected;
    private readonly string ENTER_AWAIT_TXT = "선택";
    private readonly string ENTER_SELECTED_TXT = "진입대기";
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private float ori_x_pos = -1;
    private Vector2 btn_pos;
    private int btn_width;

    private void Start() {
        // 현재 width 가져오기
        btn_width = Screen.width / 10;
    }

    public void Select_Plannet(GameObject target_plannet) {
        if (target_plannet == null) return;
        // 한 번 더 누른경우 선택 해제 하기
        if(target_plannet == now_selected) {
            target_plannet.transform.Find("Text Area").Find("Enter").DOMoveX(ori_x_pos, .25f);
            Game_Start_Checker.Instance.Start_Btn_Deactive();
            return;
        }
 
        Now_plannet = target_plannet.name.ToLower() switch {
            "green" => Plannet.Green,
            "blue" => Plannet.Blue,
            "gray" => Plannet.Gray,
            "ancient" => Plannet.Ancient,
            _ => Plannet.non
        };
        
        // 선택되었으니 ui 효과 변경
        target_plannet.transform.Find("Text Area").Find("Enter").GetComponent<Image>().color = enter_select;
        target_plannet.transform.Find("Text Area").Find("Enter").Find("text").GetComponent<TMP_Text>().text = ENTER_SELECTED_TXT;        
        target_plannet.transform.Find("Image Area").Find("Image").GetComponent<Plannet_Hovering>().Start_floating();
        // 좌로 이동시킴
        ori_x_pos = target_plannet.transform.Find("Text Area").Find("Enter").transform.position.x;
        btn_pos = target_plannet.transform.Find("Text Area").Find("Enter").transform.position;
        btn_pos.x += btn_width;
        target_plannet.transform.Find("Text Area").Find("Enter").DOMoveX(ori_x_pos - 100f, .25f);

        // 기존 선택이던 ui 변경
        if (now_selected != null) {
            now_selected.transform.Find("Text Area").Find("Enter").GetComponent<Image>().color = enter_await;
            now_selected.transform.Find("Text Area").Find("Enter").Find("text").GetComponent<TMP_Text>().text = ENTER_AWAIT_TXT;
            now_selected.transform.Find("Image Area").Find("Image").GetComponent<Plannet_Hovering>().Init();
            now_selected.transform.Find("Text Area").Find("Enter").DOMoveX(ori_x_pos, .25f);
        }
        // 현 선택값 저장
        now_selected = target_plannet;

        // 선택된 것이 있으므로 시작 가능하게 변경
        Game_Start_Checker.Instance.Start_Btn_Active(btn_pos);
    }

    public void All_Deselect_plannet() {
        Now_plannet = Plannet.non;
        if(now_selected != null) {
            now_selected.transform.Find("Text Area").Find("Enter").GetComponent<Image>().color = enter_await;
            now_selected.transform.Find("Text Area").Find("Enter").Find("text").GetComponent<TMP_Text>().text = ENTER_AWAIT_TXT;
            now_selected.transform.Find("Image Area").Find("Image").GetComponent<Plannet_Hovering>().Init();
            now_selected.transform.Find("Text Area").Find("Enter").DOMoveX(ori_x_pos, 0);
            now_selected = null;
        }

    }
}
