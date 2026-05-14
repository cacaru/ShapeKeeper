using System.Collections;
using UnityEngine;
using static CUSTOM_DATA.Game_Value_Data;
using static CUSTOM_DATA.Common_Data;
using static CUSTOM_DATA.Home_UI_Controller;
using DG.Tweening;

public class Achievement_Setter : Scene_Singleton<Achievement_Setter>
{
    [SerializeField] GameObject Achieve_Canvas;

    private Vector3 close = new(90, 0, 0);

    void Start() {
        Header_Setter.Instance.Set_Head();

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
        Page_Loading_End_Mover.Instance.Closing();
        Closing();
    }

    public void Openning() {
        Header_Setter.Instance.Active();
        Footer_Setter.Instance.Active();
        Achieve_Canvas.transform.DORotate(open, .25f);
    }

    public void Closing() {
        Header_Setter.Instance.Deactive();
        Footer_Setter.Instance.Deactive();
        Achieve_Canvas.transform.DORotate(close, .25f);
    }
}
