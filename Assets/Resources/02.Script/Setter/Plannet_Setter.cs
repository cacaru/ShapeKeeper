using UnityEngine;
using static CUSTOM_DATA.Common_Data;
using static CUSTOM_DATA.Home_UI_Controller;
using System.Collections;

public class Plannet_Setter : MonoBehaviour
{

    void Start() {
        Page_Loading_End_Mover.Instance.Init();
        StartCoroutine(Watcher());
    }

    IEnumerator Watcher() {
        while (!page_loading) {
            yield return null;
        }
        Page_Loading_End_Mover.Instance.Openning();
    }

    public void Close_UI() {
        Page_Loading_End_Mover.Instance.Closing();
    }

}
