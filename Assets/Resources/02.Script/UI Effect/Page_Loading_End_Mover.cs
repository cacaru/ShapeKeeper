using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static CUSTOM_DATA.Home_UI_Controller;
public class Page_Loading_End_Mover : Singleton<Page_Loading_End_Mover>
{
    // 화면에 보여질 때 slider를 이동시킬 스크립트
    // 이 화면으로 왔을 때 는 좌측으로
    // 이 화면에서 다른 화면으로 넘어갈 때는 우측에서 다시 원래 위치로 오기

    private Transform[] sliders;
    private float ori_x;
    private float move_value = 120;

    void Start()
    {
        Init();
    }

    public void Init() {
        // 슬라이더들을 받아옴
        sliders = gameObject.GetComponentsInChildren<Transform>();
        ori_x = sliders[0].gameObject.transform.position.x;

        page_loading = true;
    }

    public void Openning() {
        StartCoroutine(Move_left_slider());
    }

    public void Closing() {
        StartCoroutine(Move_right_slider());
    }

    IEnumerator Move_left_slider() {
        foreach (Transform t in sliders) {
            if(t != transform) {
                StartCoroutine(Open(t.gameObject));
            }
            yield return new WaitForSeconds(.01f);
        }
    }

    IEnumerator Move_right_slider() {
        int size = sliders.Length - 1;
        for(int i = size; i >= 0; i--) {
            if (sliders[i] != transform) {
                StartCoroutine(Close(sliders[i].gameObject));
            }
            yield return new WaitForSeconds(.01f);
        }
    }

    IEnumerator Open(GameObject slider) {
        var pos = slider.transform.position;
        while (pos.x >= -1200f) {
            pos.x -= move_value;
            slider.transform.position = pos;
            yield return null;
        }
    }

    IEnumerator Close(GameObject slider) {
        var pos = slider.transform.position;
        while (pos.x < ori_x) {
            pos.x += move_value;
            slider.transform.position = pos;
            yield return null;
        }
    }
}
