using CUSTOM_DATA;
using System.Collections;
using UnityEngine;

public class Shadow_Hovering : MonoBehaviour
{
    private Vector2 pos;
    private Vector3 scale;
    private float max;
    private float min;
    private bool upper = true;
    public float speed = 0.0001f;
    public float scale_factor = 0.0008f;
    public float upper_value = 0.03f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        pos = transform.position;
        scale = transform.localScale;
        min = pos.y;
        max = pos.y - upper_value;
        
        StartCoroutine(floating());
    }

    IEnumerator floating() {
        // true 부분에 일시정지 변수 넣기
        while (true) {
            //max = min + upper_value;
            if (Game_State_Data.is_gaming) {
                pos.y = upper ? pos.y - speed : pos.y + speed;
                scale.x = upper ? scale.x - scale_factor : scale.x + scale_factor;
                scale.y = upper ? scale.y - scale_factor : scale.y + scale_factor;
                scale.z = upper ? scale.z - scale_factor : scale.z + scale_factor;
                
                if (upper) {
                    if (pos.y <= max) { upper = false; }
                }
                else {
                    if (pos.y >= min) upper = true;
                }
                transform.localScale = scale;
                transform.position = pos;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
