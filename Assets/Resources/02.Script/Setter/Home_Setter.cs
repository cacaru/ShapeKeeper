using System.Collections;
using UnityEngine;
using static CUSTOM_DATA.Game_Value_Data;
using static CUSTOM_DATA.Common_Data;
using static CUSTOM_DATA.Home_UI_Controller;

public class Home_Setter : Scene_Singleton<Home_Setter>
{
    [SerializeField] private Animator stamina;
    [SerializeField] private Animator start_btn;
    [SerializeField] private Animator skill_btn;

    void Start() {
        Header_Setter.Instance.Set_Head();
        StartCoroutine(Watcher());
    }

    IEnumerator Watcher() {
        while (!page_loading) {
            yield return null;
        }
        StartCoroutine(Open_UI());
    }

    IEnumerator Open_UI() {
        Page_Loading_End_Mover.Instance.Openning();
        yield return wfs_1;
        Openning();
    }

    public void Close_UI() {
        Page_Loading_End_Mover.Instance.Closing();
        Closing();
    }

    public void Openning() {
        Header_Setter.Instance.Active();
        Footer_Setter.Instance.Active();
        stamina.SetBool(ANI_ACTIVATE, true);
        Btn_Show_Toggle();
    }

    public void Closing() {
        Header_Setter.Instance.Deactive();
        Footer_Setter.Instance.Deactive();
        stamina.SetBool(ANI_ACTIVATE, false);
        Btn_Show_Toggle();
    }

    public void Btn_Show_Toggle() {
        var state = start_btn.GetBool(ANI_ACTIVATE);
        state = !state;

        start_btn.SetBool(ANI_ACTIVATE, state);
        skill_btn.SetBool(ANI_ACTIVATE, state);
    }

}
