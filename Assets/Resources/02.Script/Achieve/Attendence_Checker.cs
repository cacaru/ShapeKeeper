using static CUSTOM_DATA.Game_State_Data;
using CUSTOM_DATA;
using System;
using System.Collections;
using UnityEngine;

public class Attendence_Checker : Singleton<Attendence_Checker>
{

    private void Start() {
        StartCoroutine(connect_checking());
    }

    IEnumerator connect_checking() {
        while (true) {
            if(db_state == DB_STATE.Usable) {
                break;
            }
            yield return null;
        }
        Attendence_check();
    }

    public void Attendence_check() {
        DateTime now = DateTime.Now;

        // 게임 시작시간 불러오기
        string last_start_time = PlayerPrefs.GetString("Start_Time");
        //string last_start_time = PlayerPrefs.GetString("End_Time");
        // 시작시간 기록
        PlayerPrefs.SetString("Start_Time", now.ToString());
        PlayerPrefs.Save();
        // 신규 접속
        if (last_start_time == "") {
            // 출석체크
            PlayerPrefs.SetInt("Attend", 1);
            PlayerPrefs.Save();
        }
        else {
            // 지난번 최초 접속일 확인
            DateTime last_start_datetime = Convert.ToDateTime(last_start_time);
            // 주차 확인용 계산
            // 오늘의 올해주차(올해의 몇번째 주차)인지 구함
            DateTime first_day_of_year = new(DateTime.Now.Year, 1, 1);
            int q_num = (int)(first_day_of_year.DayOfWeek + DateTime.Now.DayOfYear);
            int todays_week_of_year = q_num / 7 + (q_num % 7 == 0 ? 0 : 1);

            // 마지막 접속일자의 주차 구하기
            q_num = (int)(first_day_of_year.DayOfWeek + last_start_datetime.DayOfYear);
            int last_connect_week_of_year = q_num / 7 + (q_num % 7 == 0 ? 0 : 1);

            //Debug.Log("이번주는 : " + todays_week_of_year + "주차 // 접속일은 : " + last_connect_week_of_year + "주차 ");
            // 두 주차가 다르면 주간퀘 초기화
            if (todays_week_of_year != last_connect_week_of_year) {
                Achieve_Controller.Instance.Weekly_Reset();
            }

            if (last_start_datetime.Date != now.Date) {

                // 일자가 달라졌다면 일일 퀘스트를 초기화
                Achieve_Controller.Instance.Daily_Reset();
                //datahub.QuestResetChecker = true;
                // 출석체크
                int attend_day_count = PlayerPrefs.GetInt("Attend");
                PlayerPrefs.SetInt("Attend", attend_day_count + 1);
                PlayerPrefs.Save();
            }
        }

        Achievement_Observer.Instance.Attendence_Check();
        // day check observer 실행
        Day_Change_Observer.Instance.Activate();

        // 더이상 필요 없음
        Destroy(gameObject.GetComponent<Attendence_Checker>());

    }
}
