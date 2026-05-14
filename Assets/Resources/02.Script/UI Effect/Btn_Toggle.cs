using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CUSTOM_DATA;

public class Btn_Toggle : MonoBehaviour
{

    [SerializeField]
    private Sprite Off;
    [SerializeField]
    private Sprite On;

    [SerializeField]
    private string Off_txt = "";
    [SerializeField]
    private string On_txt = "";

    private Animator animator;
    private bool has_anime = false;

    [SerializeField]
    private GameObject target_img;
    [SerializeField]
    private TMP_Text target_txt;

    private void Start() {
        animator = GetComponent<Animator>();
        if (animator != null) has_anime = true;
    }

    public void Toggle(bool state) { 
        if (state) {
            target_img.GetComponent<Image>().sprite = On;
            target_txt.text = On_txt;
        }
        else {
            target_img.GetComponent<Image>().sprite = Off;
            target_txt.text = Off_txt;
        }

        if (has_anime) {
            animator.SetBool(Game_Value_Data.ANI_ACTIVATE, state);
        }
    }
}
