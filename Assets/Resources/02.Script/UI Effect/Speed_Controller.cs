using UnityEngine;
using UnityEngine.UI;
using static CUSTOM_DATA.Common_Data;
using static CUSTOM_DATA.Game_State_Data;

public class Speed_Controller : MonoBehaviour
{
    private int speed_type = 0;
    public void Toggle() {
        speed_type += 1;
        if(speed_type > 2) {
            speed_type = 0;
        }

        switch(speed_type) {
            // normal speed
            case 0:
                GetComponent<Image>().sprite = speed_normal;
                game_speed = 1;
                is_gaming = true;
                break;
            // x2
            case 1:
                GetComponent<Image>().sprite = speed_bust;
                game_speed = 2;
                break;
            // stop
            case 2:
                GetComponent<Image>().sprite = speed_stop;
                game_speed = 0;
                is_gaming = false;
                break;
        }
    }
}
