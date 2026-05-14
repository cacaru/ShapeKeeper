using UnityEngine;
using static CUSTOM_DATA.Game_Value_Data;
using static CUSTOM_DATA.Game_State_Data;
using CUSTOM_DATA;

public class Footer_Setter : Singleton<Footer_Setter>
{
    private Animator footer_ani;

    private GameObject home_on;
    private GameObject unit_on;
    private GameObject shop_on;
    private GameObject achieve_on;
    //private GameObject setting_on;

    void Start() {
        footer_ani = transform.Find("Background").GetComponent<Animator>();

        home_on = transform.Find("Background").Find("IconArea").Find("Home_Btn").Find("On").gameObject;
        unit_on = transform.Find("Background").Find("IconArea").Find("Unit_Btn").Find("On").gameObject;
        shop_on = transform.Find("Background").Find("IconArea").Find("Shop_Btn").Find("On").gameObject;
        achieve_on = transform.Find("Background").Find("IconArea").Find("Achievement_Btn").Find("On").gameObject;
        //setting_on = transform.Find("Background").Find("IconArea").Find("Setting_Btn").Find("On").gameObject;
    }

    public void Active() {
        footer_ani.SetBool(ANI_ACTIVATE, true);

        // 현재 씬 이름에 따라 달라지게
        switch (now_scene) {
            case SceneType.Home:
                home_on.SetActive(true);
                unit_on.SetActive(false);
                shop_on.SetActive(false);
                achieve_on.SetActive(false);
                break;
            case SceneType.Unit:
                home_on.SetActive(false);
                unit_on.SetActive(true);
                shop_on.SetActive(false);
                achieve_on.SetActive(false);
                break;
            case SceneType.Shop:
                home_on.SetActive(false);
                unit_on.SetActive(false);
                shop_on.SetActive(true);
                achieve_on.SetActive(false);
                break;
            case SceneType.Achievement:
                home_on.SetActive(false);
                unit_on.SetActive(false);
                shop_on.SetActive(false);
                achieve_on.SetActive(true);
                break;
        }
    }

    public void Deactive() {
        footer_ani.SetBool(ANI_ACTIVATE, false);
    }
}
