using TMPro;
using UnityEngine;
using static CUSTOM_DATA.Game_State_Data;

public class Difficulty_Controller : MonoBehaviour
{

    [SerializeField] GameObject up_btn;
    [SerializeField] GameObject down_btn;
    [SerializeField] TMP_Text difficulty_text;

    public void Difficulty_Controll(int value) {
        difficulty += value;
        if(difficulty <= 1) {
            down_btn.SetActive(false);
            difficulty = 1;
        }
        else if(difficulty >= 15) {
            up_btn.SetActive(false);
            difficulty = 15;
        }
        else {
            up_btn.SetActive(true);
            down_btn.SetActive(true);
        }

        difficulty_text.text = difficulty.ToString();
        Home_Stamina_Setter.Instance.Need_Stamina_Setting();
    }
}
