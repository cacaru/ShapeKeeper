using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Game_State_Data;
using static CUSTOM_DATA.Common_Data;
using static CUSTOM_DATA.Home_UI_Controller;
using System;
using System.Collections.Generic;
using DG.Tweening;
using CUSTOM_DATA;

public class Buy_Energy : Scene_Singleton<Buy_Energy>
{

    // 에너지 버튼을 클릭하면 
    // confrim - panel을 열고
    // border 수정
    // buy_announce_title => title
    // buy_announce       => "에너지 " + set_piece + "개 구매"
    // buy_item           => battery 이미지로 변경
    // price              => set_piece * 50
    // 
    // Confirm btn >Add Listener >> energy + / gold - / header resetting

    [SerializeField] private GameObject alert_panel;
    [SerializeField] private GameObject result_panel;
    [SerializeField] private GameObject border;
    [SerializeField] private GameObject confirm_btn;
    
    [SerializeField] private TMP_Text buy_energy;
    [SerializeField] private TMP_Text buy_energy_price;

    public bool free_energy = false;
    private Color img_color = new(246 / 255f, 1f, 174 / 255f);

    private readonly List<string> parts = new();
    private Vector3 close = new(90, 0, 0);

    public void Random_Energy_Buy() {
        free_energy = true;
        // 3시간마다 한번씩만 받을 수있음
        // 마지막에 받았던 시간을 확인

        System.DateTime now = System.DateTime.Now;

        System.DateTime end_time = System.Convert.ToDateTime(PlayerPrefs.GetString("Free_Energy_Chest_RemainTime") == "" ? now : PlayerPrefs.GetString("Free_Energy_Chest_RemainTime"));
        System.TimeSpan dif = now - end_time;

        int sec_dif = (int)System.Math.Round(dif.TotalSeconds);

        // 3시간이 지나지 않았으면 받기 불가
        if (sec_dif > 0 && sec_dif < 10800) {
            // announce alert
            Alert(Utility.FormatLeftTime(10800 - sec_dif));
            return;
        }

        int end_point = (user.level + 1) * 4;
        buy_energy_value = UnityEngine.Random.Range(1, end_point);
        Setting_Buy_Energy_Confirm_Panel();
    }

    private void Alert(string announce) {
        alert_panel.transform.Find("Border").Find("announce").GetComponent<TMP_Text>().text = announce;
        alert_panel.transform.DORotate(open, .25f);
    }

    public void Alert_End() {
        alert_panel.transform.DORotate(close, .25f);
    }

    public void Setting_Buy_Energy_Confirm_Panel() {
        if (buy_energy_value <= 0) return;

        // setting confirm panel
        border.transform.Find("buy_announce").GetComponent<TMP_Text>().text = "에너지 " + buy_energy_value + "개 구매";
        border.transform.Find("buy_item").GetComponent<Image>().sprite = battery_sprite;
        border.transform.Find("buy_item").GetComponent<Image>().color = img_color;
        /*
        var rect = border.transform.Find("buy_item").GetComponent<RectTransform>().sizeDelta;
        rect.x = 120;
        border.transform.Find("buy_item").GetComponent<RectTransform>().sizeDelta = rect;
        */
        border.transform.Find("price").GetComponent<TMP_Text>().text = free_energy ? "0" : (buy_energy_value * 50).ToString();

        confirm_btn.GetComponent<Button>().onClick.RemoveAllListeners();
        confirm_btn.GetComponent<Button>().onClick.AddListener(() => {
            Buy_Energy_Confirm();
        });

        // setting result panel
        result_panel.transform.Find("Simple_Border").Find("Item_Obj").Find("image").GetComponent<Image>().sprite = battery_sprite;
        result_panel.transform.Find("Simple_Border").Find("Item_Obj").Find("image").GetComponent<Image>().color = img_color;
        result_panel.transform.Find("Simple_Border").Find("Item_Obj").Find("name").GetComponent<TMP_Text>().text = "에너지";
        result_panel.transform.Find("Simple_Border").Find("Item_Obj").Find("type").GetComponent<TMP_Text>().text = "일반";
        result_panel.transform.Find("Simple_Border").Find("piece_before").GetComponent<TMP_Text>().text = stamina_storage.Stamina.ToString();
        result_panel.transform.Find("Simple_Border").Find("piece_after").GetComponent<TMP_Text>().text = (stamina_storage.Stamina + buy_energy_value).ToString();
        result_panel.transform.Find("Simple_Border").gameObject.SetActive(true);
        /*
        rect = result_panel.transform.Find("Simple_Border").Find("Item").Find("image").GetComponent<RectTransform>().sizeDelta;
        rect.x -= 80;
        result_panel.transform.Find("Simple_Border").Find("Item").Find("image").GetComponent<RectTransform>().sizeDelta = rect;
        */
        // panel on
        Shop_Seting_Intergrator.Instance.Confirm_Field_On();
    }

    public void Buy_Energy_Confirm() {
        stamina_storage.Add_Stamina(buy_energy_value);
        Utility.SaveStaminaStorage();
        if (!free_energy) {
            user.Gold -= buy_energy_value * 50;
            Achievement_Observer.Instance.Use_Gold_Achievement(buy_energy_value * 50);
        }
        else {
            // 현재 시간 설정
            PlayerPrefs.SetString("Free_Energy_Chest_RemainTime", DateTime.Now.ToString());
            PlayerPrefs.Save();
        }
        Header_Setter.Instance.Set_Head();
        free_energy = false;
        Shop_Seting_Intergrator.Instance.Loading_On(0);
    }


    // btn controll
    public void Energy_controll(int controll_value) {
        buy_energy_value += controll_value;
        //buy_energy_value = plus ? buy_energy_value += controll_value : buy_energy_value -= controll_value;
        if (buy_energy_value < 0) {
            buy_energy_value = 0;
        }
        
        if(buy_energy_value >= 100) {
            buy_energy_value = 99;
        }

        buy_energy.text = buy_energy_value.ToString();

        buy_energy_price.text = (buy_energy_value * 50).ToString();
    }
}
