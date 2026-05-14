using CUSTOM_DATA;
using System.Collections;
using UnityEngine;
using static CUSTOM_DATA.Common_Data;

public class Tower_Hovering : MonoBehaviour
{
    private Vector2 pos;
    private float max;
    private float min;
    private bool upper = true;
    public float speed = 0.001f;
    public float upper_value = 0.03f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pos = transform.position;
        min = pos.y;
        max = pos.y + upper_value;

        StartCoroutine(floating());
    }

    IEnumerator floating() {
        // true 부분에 일시정지 변수 넣기
        while (true) {
            //max = min + upper_value;
            if (Game_State_Data.is_gaming) {
                pos.y = upper ? pos.y + speed : pos.y - speed;
                if (upper) {
                    if (pos.y >= max) upper = false;
                }
                else {
                    if (pos.y <= min) upper = true;
                }
                transform.position = pos;
            }

            yield return wfef;
        }
    }
}
