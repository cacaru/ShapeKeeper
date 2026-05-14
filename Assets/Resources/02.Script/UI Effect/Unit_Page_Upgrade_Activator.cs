using UnityEngine;
using static CUSTOM_DATA.Game_Value_Data;

public class Unit_Page_Upgrade_Activator : Scene_Singleton<Unit_Page_Upgrade_Activator>
{
    [SerializeField] private Animator image_ani;
    [SerializeField] private Animator window_ani;

    public void Active() {
        image_ani.SetBool(ANI_ACTIVATE, true);
        window_ani.SetBool(ANI_ACTIVATE, true);
    }
    public void Deactive() {
        image_ani.SetBool(ANI_ACTIVATE, false);
        window_ani.SetBool(ANI_ACTIVATE, false);
    }

    public void Window_active() {
        window_ani.SetBool(ANI_ACTIVATE, true);
    }

    public void Window_deactive() {
        window_ani.SetBool(ANI_ACTIVATE, false);
    }

    public void Image_upgrade_active() {
        image_ani.SetBool("Upgrade_Activate", true);
    }

    public void Image_upgrade_deactive() {
        image_ani.SetBool("Upgrade_Activate", false);
    }
}
