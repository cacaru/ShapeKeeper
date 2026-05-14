using UnityEngine;
using UnityEngine.SceneManagement;
using static CUSTOM_DATA.Game_State_Data;
using static CUSTOM_DATA.Game_Data;

public class In_Game_Scene_Changer : Scene_Singleton<In_Game_Scene_Changer>
{

    public void Go_Home() {
        // all init
        State_Init();
        Data_Init();
        is_gaming = false;
        is_menu = true;
        Scene_Changer.Instance.Go_Back_Home();
    }

}
