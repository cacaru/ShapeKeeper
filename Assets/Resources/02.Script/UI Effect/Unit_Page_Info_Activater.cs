using UnityEngine;
using static CUSTOM_DATA.Game_Value_Data;

public class Unit_Page_Info_Activater : Scene_Singleton<Unit_Page_Info_Activater>
{
    private Animator ani;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ani = GetComponent<Animator>();
    }

    public void Active() {
        ani.SetBool(ANI_ACTIVATE, true);
    }

    public void Deactive(bool footer_active) {
        ani.SetBool(ANI_ACTIVATE, false);
        if(footer_active ) { Footer_Setter.Instance.Active(); }
    }

}
