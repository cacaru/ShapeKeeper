using CUSTOM_DATA;
using System.Collections;
using UnityEngine;

public class Satellite_Mover : MonoBehaviour
{
    private Vector3 pos;
    private Vector3 ori_pos;
    private float degrees = 0;
    public float speed = 2f;
    public int id = 0;

    float radians = 0;
    readonly float radius = 0.22f;
    float x = 0;
    float y = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ori_pos = transform.position;
        if(id != 0) {
            degrees = 360 / (id + 1);
        }
        // 원을 그리며 돌아야 함 -> 중심은 ori obj
        StartCoroutine(Rotate_satellite());
    }

    
    IEnumerator Rotate_satellite() {
        while (true) {
            if (Game_State_Data.is_gaming) {
                degrees += speed;
                if(degrees < 360) {
                    radians = Mathf.Deg2Rad * degrees;
                    x = (float)Mathf.Sin(radians) * radius;
                    y = (float)Mathf.Cos(radians) * radius;
                    pos.x = x; pos.y = y;
                    transform.position = ori_pos + pos;
                    transform.rotation = Quaternion.Euler(0, 0, degrees);
                }
                else {
                    degrees = 0;
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
