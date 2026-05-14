using UnityEngine;
using CUSTOM_DATA;
using static CUSTOM_DATA.Game_State_Data;
using static CUSTOM_DATA.Common_Data;
using System;
using TMPro;
using DG.Tweening;

public class Game_Start_Checker : Scene_Singleton<Game_Start_Checker>
{
    [SerializeField] Transform alert_panel;

    private bool Can_Start = false;
    private Vector3 close = new(0, 90, 0);
    public void Game_Start() {
        if (!Can_Start) return;

        // change scene 
        // check
        if (Now_plannet == Plannet.non) return;
        if (difficulty < 1 || difficulty > 15) return;

        // stamina check
        // 난이도에 따라 소모 스테미나 증가
        // 기본 4 소모 -> 난이도 2부터 추가 1 
        int default_consume_stamina = 5 + difficulty - 1;
        if (stamina_storage.Stamina < default_consume_stamina) {
            // alert
            string str = Utility.FormatLeftTime((default_consume_stamina - stamina_storage.Stamina) * 60 * 3);
            Alert(str);
            return;
        }

        stamina_storage.Consume_Stamina(default_consume_stamina);
        stamina_storage.last_recover_time = DateTime.Now.ToString("o");
        Utility.SaveStaminaStorage();

        is_menu = false;
        is_gaming = true;
        Scene_Changer.Instance.Game_Start();
        // 업적 추가
        Achievement_Observer.Instance.Use_Energy_Achievement(4);
    }

    public void Start_Btn_Active(Vector2 pos) {
        Can_Start = true;
        transform.position = pos;
        gameObject.SetActive(true);
    }

    public void Start_Btn_Deactive() {
        Can_Start = false;
        gameObject.SetActive(false);
        Stage_Selecter.Instance.All_Deselect_plannet();
    }


    private void Alert(string announce) {
        alert_panel.Find("Border").Find("announce").GetComponent<TMP_Text>().text = announce;
        alert_panel.DORotate(open, .25f);
    }

    public void Alert_End() {
        alert_panel.transform.DORotate(close, .25f);
    }
}
