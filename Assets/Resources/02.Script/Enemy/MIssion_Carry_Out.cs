using NUnit.Framework;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CUSTOM_DATA.Common_Data;


public class MIssion_Carry_Out : MonoBehaviour
{

    [SerializeField] Image D_Image;
    [SerializeField] Image C_Image;
    [SerializeField] Image B_Image;
    [SerializeField] Image A_Image;
    [SerializeField] Image S_Image;

    [SerializeField] TMP_Text D_left_time;
    [SerializeField] TMP_Text C_left_time;
    [SerializeField] TMP_Text B_left_time;
    [SerializeField] TMP_Text A_left_time;
    [SerializeField] TMP_Text S_left_time;

    /// <summary>
    /// 클릭시 미션을 실행하고 
    /// 재사용 대기시간을 굴림
    /// </summary>

    private readonly int d_time = 60;
    private readonly int c_time = 120;
    private readonly int b_time = 180;
    private readonly int a_time = 240;
    private readonly int s_time = 300;

    // 남은 시간을 저장할 변수
    private int[] left_timer = { 0, 0, 0, 0, 0 };
    private Image[] mission_images = new Image[5];
    private TMP_Text[] mission_left_timer = new TMP_Text[5];
    private int[] left_max_timer = { 60, 120, 180, 240, 300 };
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        mission_images[0] = D_Image;
        mission_images[1] = C_Image;
        mission_images[2] = B_Image;
        mission_images[3] = A_Image;
        mission_images[4] = S_Image;

        mission_left_timer[0] = D_left_time;
        mission_left_timer[1] = C_left_time;
        mission_left_timer[2] = B_left_time;
        mission_left_timer[3] = A_left_time;
        mission_left_timer[4] = S_left_time;

    }

    public void Carry_out_Mission(string rank) {
        int time = 999;
        int left_time_value = 999;
        switch (rank) {
            case "d":
                time = d_time;
                left_time_value = 0;
                break;
            case "c":
                time = c_time;
                left_time_value = 1;
                break;
            case "b":
                time = b_time;
                left_time_value = 2;
                break;
            case "a":
                time = a_time;
                left_time_value = 3;
                break;
            case "s":
                time = s_time;
                left_time_value = 4;
                break;
        }

        if (left_timer[left_time_value] > 0) {
            return;
        }

        // 사용 가능하면 spawn
        Spawner.Instance.Spawn_Mission_Boss(left_time_value + 1);
        // time set
        left_timer[left_time_value] = time;
        // timer set
        StartCoroutine(Mission_Timer(left_time_value));

        // 사용 불가 표시 - 이미지로
        mission_images[left_time_value].fillAmount = 0;
    }

    IEnumerator Mission_Timer(int value) {
        while (left_timer[value] > 0) {
            yield return wfs_1;
            left_timer[value]--;
            mission_images[value].fillAmount = left_timer[value] > 0 ? (float)(left_max_timer[value] - left_timer[value]) / left_max_timer[value]: 1;
            mission_left_timer[value].text = left_timer[value].ToString();
            
            // value 에 해당하는 이미지의 filled 를 채우기
        }

        // 다시 사용 가능 표시
        mission_images[value].fillAmount = 1;
        mission_left_timer[value].text = "0";
    }
}
