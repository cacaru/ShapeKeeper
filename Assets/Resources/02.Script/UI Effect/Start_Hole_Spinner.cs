using UnityEngine;

public class Start_Hole_Spinner : MonoBehaviour
{
    float degrees = 0;
    Animator start_btn_ani;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        start_btn_ani = transform.parent.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (start_btn_ani.GetBool("Activate")) {
            // rotate
            degrees += .1f;
            transform.rotation = Quaternion.Euler(0, 70, degrees);
        }
    }
}
