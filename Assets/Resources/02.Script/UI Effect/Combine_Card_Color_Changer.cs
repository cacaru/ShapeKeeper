using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static CUSTOM_DATA.Common_Data;

public class Combine_Card_Color_Changer : MonoBehaviour
{
    private bool changing = false;
    private bool normal = true;
    IEnumerator Changing() {
        while (changing) {
            GetComponent<Image>().color = normal ? material_color : ethereal_color;
            normal = !normal;
            yield return wfs_2;
        }
    }

    public void Set_Changing(bool set) {
        changing = set;
        if(set)
            StartCoroutine(Changing());
        else
            StopAllCoroutines();
    }

}
