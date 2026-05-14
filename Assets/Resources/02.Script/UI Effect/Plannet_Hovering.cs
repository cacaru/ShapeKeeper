using CUSTOM_DATA;
using System.Collections;
using UnityEngine;
using static CUSTOM_DATA.Common_Data;

public class Plannet_Hovering : MonoBehaviour
{
    private Vector2 pos;
    private Vector2 ori_pos;
    private float max;
    private float min;
    private bool upper = true;
    public float speed = 0.1f;

    private bool selecting = false;

    public void Start_floating() {
        selecting = true;

        ori_pos = transform.localPosition;
        pos = transform.localPosition;
        min = transform.localPosition.y - 50;
        max = transform.localPosition.y;
        
        StartCoroutine(Floating());
    }

    IEnumerator Floating() {
        // true 부분에 일시정지 변수 넣기
        while (true) {
            //max = min + upper_value;
            if (selecting) {

                if (upper) {
                    pos.y += speed;
                    if (pos.y >= max) upper = false;
                }
                else {
                    pos.y -= speed;
                    if (pos.y <= min) upper = true;
                }
                
                //transform.position = pos;
                transform.localPosition = pos;
            }

            yield return wfef;
        }
    }

    public void Init() {
        selecting = false;
        transform.localPosition = ori_pos;
    }
}
