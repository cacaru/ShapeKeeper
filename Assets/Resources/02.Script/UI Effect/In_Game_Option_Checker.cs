using UnityEngine;
using static CUSTOM_DATA.Game_State_Data;

public class In_Game_Option_Checker : Scene_Singleton<In_Game_Option_Checker>
{
    [SerializeField] private GameObject option_pannel;
    private bool activating = false;
    private int temp_game_speed;

    public bool Active { get { return activating; } }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !activating) {
            // 켜진 패널이 있다면
            // 켜진 패널을 끄기
            if (Pannel_Controller.Instance.Activate_check()) {
                Pannel_Controller.Instance.Activate_for_Install();
            }
            // 모든 패널이 꺼진 상태라면
            // 옵션창 켜기
            else {
                // 옵션창 켜기
                option_pannel.SetActive(true);
                activating = true;
            }
        }
    }

    public void Option_Open() {
        // 옵션창 켜기
        option_pannel.SetActive(true);
        activating = true;
        is_gaming = false;
        temp_game_speed = game_speed;
        game_speed = 0;
    }

    public void Option_Close() {
        option_pannel.SetActive(false);
        activating = false;
        is_gaming = true;
        game_speed = temp_game_speed;
    }
}
