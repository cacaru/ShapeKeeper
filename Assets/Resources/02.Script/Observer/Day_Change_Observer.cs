using System;
using System.Collections;
using UnityEngine;

public class Day_Change_Observer : Singleton<Day_Change_Observer>
{
    // attendance check 이후 켜져 시작시간부터 자정까지 남은 시간을 구하고
    // 남은 시간 까지 대기하는 corutine을 생성한 이후
    // 코루틴이 실행되면 다음날이 된것으로 간주하고 attendance check 한번 더 실행

    private WaitForSecondsRealtime wfsr;
    private DateTime start_time;
    public void Activate() {
        start_time = Convert.ToDateTime(PlayerPrefs.GetString("Start_Time"));
        DateTime mid_night = DateTime.Now.AddDays(1).Date;

        // 자정까지 남은 시간 구하기
        TimeSpan remain_time = mid_night - start_time;

        // 남은 시간 만큼 대기하는 코루틴 생성
        wfsr = new((int)remain_time.TotalSeconds);
        StartCoroutine(WaitNextDay());
    }

    IEnumerator WaitNextDay() {
        yield return wfsr;

        gameObject.AddComponent<Attendence_Checker>();
    }
}
