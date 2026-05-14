using System.Collections;
using UnityEngine;
using static CUSTOM_DATA.Game_Value_Data;
using static CUSTOM_DATA.Common_Data;
using static CUSTOM_DATA.Home_UI_Controller;
using DG.Tweening;

public class Shop_Setter : Scene_Singleton<Shop_Setter> {
    [SerializeField] private Transform shop;

    private Vector3 open = new(0, 0, 0);
    private Vector3 close = new(90, 0, 0);

    void Start() {
        Header_Setter.Instance.Set_Head();
        Page_Loading_End_Mover.Instance.Init();
        StartCoroutine(Watcher());
    }

    IEnumerator Watcher() {
        while (!page_loading) {
            yield return null;
        }
        StartCoroutine(Open_UI());
    }

    IEnumerator Open_UI() {
        Page_Loading_End_Mover.Instance.Openning();
        yield return wfs_1;
        Openning();
    }

    public void Close_UI() {
        Closing();
        Page_Loading_End_Mover.Instance.Closing();
    }

    public void Openning() {
        Header_Setter.Instance.Active();
        Footer_Setter.Instance.Active();
        Active();

        // 조각 판매 설정
    }

    public void Closing() {
        Header_Setter.Instance.Deactive();
        Footer_Setter.Instance.Deactive();
        Deactive();
    }

    public void Active() {
        shop.DORotate(open, .25f);
    }
    public void Deactive() {
        shop.DORotate(close, .25f);
    }
}
