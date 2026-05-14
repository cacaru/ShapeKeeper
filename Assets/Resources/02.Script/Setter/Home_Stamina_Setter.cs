using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CUSTOM_DATA.Game_State_Data;
using static CUSTOM_DATA.Common_Data;

public class Home_Stamina_Setter : Scene_Singleton<Home_Stamina_Setter>
{
    [SerializeField] Transform stamina;
    [SerializeField] Transform need_stamina;

    // stamina 는 db에 저장할까? 
    // 이번엔 db 에 저장하자

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Setting_Wait());
    }

    IEnumerator Setting_Wait() {
        while (true) {
            yield return wfef;
            if(db_state == CUSTOM_DATA.DB_STATE.Usable) {
                break;
            }
        }

        Setting();
        Need_Stamina_Setting();
    }

    public void Setting() {
        stamina.Find("max_stamina").GetComponent<TMP_Text>().text = stamina_storage.Max_Stamina.ToString();
        stamina.Find("stamina").GetComponent<TMP_Text>().text = stamina_storage.Stamina.ToString();
        float val = (float)stamina_storage.Stamina / stamina_storage.Max_Stamina;
        stamina.Find("stamina_slider").GetComponent<Slider>().value = val;
    }

    Color possible = new(155/255f, 224/255f, 56/255f);
    Color impossible = new(224/255f, 77/255f, 56/255f);

    public void Need_Stamina_Setting() {
        int need_stamina_val = 5 + difficulty - 1;
        
        need_stamina.Find("stamina").GetComponent<TMP_Text>().text = need_stamina_val.ToString();
        if (stamina_storage.Stamina >= need_stamina_val) need_stamina.Find("stamina").GetComponent<TMP_Text>().color = possible;
        else need_stamina.Find("stamina").GetComponent<TMP_Text>().color = impossible;
    }

}
