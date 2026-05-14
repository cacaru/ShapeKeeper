using System.Collections;
using UnityEngine;
using static CUSTOM_DATA.Game_Value_Data;
using static CUSTOM_DATA.Common_Data;
using static CUSTOM_DATA.Home_UI_Controller;

public class Unit_Page_Setter : Scene_Singleton<Unit_Page_Setter>
{
    [SerializeField] Animator all_unit;

    void Start() {
        Page_Loading_End_Mover.Instance.Init();
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
        Closing();
        Page_Loading_End_Mover.Instance.Closing();
    }

    public void Openning() {
        //Header_Setter.Instance.Active();
        Footer_Setter.Instance.Active();
        all_unit.SetBool(ANI_ACTIVATE, true);
    }

    public void Closing() {
        //Header_Setter.Instance.Deactive();
        Footer_Setter.Instance.Deactive();
        all_unit.SetBool(ANI_ACTIVATE, false);
    }

    public void Active() {
        all_unit.SetBool(ANI_ACTIVATE, true);
    }
    public void Deactive() {
        all_unit.SetBool(ANI_ACTIVATE, false);
    }
}
