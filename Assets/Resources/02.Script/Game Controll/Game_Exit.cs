using DG.Tweening;
using UnityEngine;
using static CUSTOM_DATA.Game_State_Data;
using static CUSTOM_DATA.Common_Data;

public class Game_Exit : Singleton<Game_Exit>
{

    [SerializeField] private GameObject exit_modal;

    private Vector3 close = new(90,0,0);

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && !is_gaming) {
            // 게임 종료 모달 띄우기
            Exit_Open();
        }
    }

    // modal on
    public void Exit_Open() {
        exit_modal.transform.DORotate(open, .25f);
    }

    // modal close
    public void Exit_Close() {
        exit_modal.transform.DORotate(close, .25f);
    }

    public void Exit() {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

        }
}
