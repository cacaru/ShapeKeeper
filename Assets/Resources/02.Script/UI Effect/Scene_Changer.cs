using CUSTOM_DATA;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static CUSTOM_DATA.Common_Data;
using static CUSTOM_DATA.Game_State_Data;

public class Scene_Changer : Singleton<Scene_Changer>, IPointerClickHandler
{
   
    // 현재 클릭한 ui의 이름을 받아 해당 이름에 맞는 scene으로 이동
    public void OnPointerClick(PointerEventData eventData) {
        string name = eventData.pointerCurrentRaycast.gameObject.name.ToLower();

        if (!name.Contains("btn")) { return; }
        string now = SceneManager.GetActiveScene().name;
        switch (now) {
            case "Home":
                Home_Setter.Instance.Close_UI();
                break;
            case "Unit":
                Unit_Page_Setter.Instance.Close_UI();
                break;
            case "Shop":
                Shop_Setter.Instance.Close_UI();
                break;
            case "Achievement":
                Achievement_Setter.Instance.Close_UI();
                break;
        }

        switch (name) {
            case "home_btn":
                //Header_Setter.Instance.gameObject.SetActive(true);
                StartCoroutine(Load_Scene("Home"));
                break;
            case "unit_btn":
                //Header_Setter.Instance.gameObject.SetActive(false);
                StartCoroutine(Load_Scene("Unit"));
                break;
            case "shop_btn":
                StartCoroutine(Load_Scene("Shop"));
                break;
            case "achievement_btn":
                StartCoroutine(Load_Scene("Achievement"));
                break;
            default:
                StartCoroutine(Load_Scene("Home"));
                return;
        }
    }
    float time = 0;
    IEnumerator Load_Scene(string name) {
        time = 0;
        AsyncOperation oper = SceneManager.LoadSceneAsync(name);
        oper.allowSceneActivation = false;

        while(!oper.isDone) {
            time += Time.deltaTime;
            
            if(time >= 1f && oper.progress >= 0.9f) {
                oper.allowSceneActivation = true;
            }
            yield return null;
        }
        now_scene = name switch {
            "Home" => SceneType.Home,
            "Unit" => SceneType.Unit,
            "Shop" => SceneType.Shop,
            "Achievement" => SceneType.Achievement,
            "Plannet" => SceneType.Plannet,
        };
        oper.allowSceneActivation = true;
    }

    public void Go_Back_Home() {
        StartCoroutine(Go_Home());
    }

    IEnumerator Go_Home() {
        Page_Loading_End_Mover.Instance.Closing();
        yield return wfs_1;
        StartCoroutine(Load_Scene("Home"));
    }

    public void Game_Start() {
        StartCoroutine(Start_Game());
    }

    IEnumerator Start_Game() {
        Page_Loading_End_Mover.Instance.Closing();
        yield return wfs_1_5;
        // loading progress
        Header_Setter.Instance.Deactive();
        Footer_Setter.Instance.Deactive();

        StartCoroutine(Load_Scene("Plannet"));
    }


}
