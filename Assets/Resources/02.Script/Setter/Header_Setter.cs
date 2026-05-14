using System.Collections;
using TMPro;
using UnityEngine.UI;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Game_Value_Data;
using static CUSTOM_DATA.Game_State_Data;
using static CUSTOM_DATA.Common_Data;
using UnityEngine;


public class Header_Setter : Singleton<Header_Setter>
{
    private Animator animator;

    public bool setting = false;
    // header canvas에 붙이기
    public void Set_Head() {
        setting = false;
        transform.Find("Background").Find("Level_Icon").Find("Level").GetComponent<TMP_Text>().text = user.level.ToString();
        float value = user.Experience > 0 ? (float)user.Experience / user.MaxExperience : 0;
        transform.Find("Background").Find("Exp").GetComponent<Slider>().value = value;
        transform.Find("Background").Find("gold").GetComponent<TMP_Text>().text = user.gold.ToString();
        setting = true;
    }

    void Start() {
        animator = transform.Find("Background").GetComponent<Animator>();

        StartCoroutine(Observing());
    }

    IEnumerator Observing() {
        while (true) {
            if(db_state == CUSTOM_DATA.DB_STATE.Usable && user != null) 
                break;
            
            yield return wffu;
        }

        Set_Head();
    }

    public void Active() {
        animator.SetBool(ANI_ACTIVATE, true);
    }

    public void Deactive() {
        animator.SetBool(ANI_ACTIVATE, false);
    }

}
