using UnityEngine;
using CUSTOM_DATA;

public class Pannel_Activater : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private string pannel_name;

    private bool active = false;
    public void Set_Animator(Animator ani) {
        animator = ani;
    }

    public virtual void Pannel_Activate() {
        active = true;
        animator.SetBool(Game_Value_Data.ANI_ACTIVATE, true);
    }

    public virtual void Pannel_Deactivate() {
        active = false;
        animator.SetBool(Game_Value_Data.ANI_ACTIVATE, false);
    }

    public virtual void Pannel_Toggle() {
        active = !active;
        if (active) {
            Pannel_Activate();
        }
        else {
            Pannel_Deactivate();
        }
    }
}
