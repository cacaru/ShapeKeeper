using UnityEngine;
using static CUSTOM_DATA.Game_State_Data;
using static CUSTOM_DATA.Game_Data;
using CUSTOM_DATA;

public class Achievement_Observer : Singleton<Achievement_Observer>
{
    string query;
    /***************************** achievement **************************************************/
    public void Boss_Kill_Achievement(int round_type) {
        int boss_round = round_type / 10;
        
        Achievement achieve = boss_round switch {
            1 => achievement[28],
            2 => achievement[29],
            3 => achievement[30],
            4 => achievement[31],
            5 => achievement[32],
            6 => achievement[33],
            7 => achievement[34],
            8 => achievement[35],
            9 => achievement[36],
            10 => achievement[37],
            11 => achievement[38],
            12 => achievement[39],
            _ => null,
        };
        if (achieve != null) { Modify_Achievement(achieve, 0); }

        //첫 클리어 업적 확인
        achieve = boss_round switch {
            1 => achievement[1],
            2 => achievement[2],
            3 => achievement[3],
            4 => achievement[4],
            5 => achievement[5],
            6 => achievement[6],
            7 => achievement[7],
            8 => achievement[8],
            9 => achievement[9],
            10 => achievement[10],
            11 => achievement[11],
            12 => achievement[12],
            _ => null
        };

        if (achieve != null && achieve.counter == 0) {
            Modify_Achievement(achieve, 0);
        }
    }

    public void Mission_Boss_Kill_Achievement(int type) {
        Achievement achieve = type switch {
            1 => achievement[46],
            2 => achievement[47],
            3 => achievement[48],
            4 => achievement[49],
            5 => achievement[50],
            _ => null
        };

        if(achieve == null) {
            return;
        }

        Modify_Achievement(achieve, 0);
    }

    public void Clear_Achievement() {
        Achievement achieve = difficulty switch {
            1 => achievement[13],
            2 => achievement[14],
            3 => achievement[15],
            4 => achievement[16],
            5 => achievement[17],
            6 => achievement[18],
            7 => achievement[19],
            8 => achievement[20],
            9 => achievement[21],
            10 => achievement[22],
            11 => achievement[23],
            12 => achievement[24],
            13 => achievement[25],
            14 => achievement[26],
            15 => achievement[27],
            _ => null
        };

        if(achieve == null || achieve.counter >= 1) { return; }

        Modify_Achievement(achieve, 0);
    }

    // 강화
    public void Upgrade_Achievement() {
        var achieve = achievement[40];
        Modify_Achievement(achieve, 0);

        var achieve_2 = weekly_quest[9];
        if(achieve_2.checker < 1) {
            Modify_Achievement(achieve_2, 2);
        }
    }

    // 유닛 생성 업적?
    public void Summon_Achievement() {
        var achieve = achievement[41];
        Modify_Achievement(achieve, 0);
    }

    // dot 1000 / 3000/ 5000
    public void Possess_Dot_Achievement() {
        Achievement achieve = dot switch {
            >= 1000 and < 3000 => achievement[42],
            >= 3000 and < 5000 => achievement[43],
            >= 5000 => achievement[44],
            _ => null
        };

        if(achieve == null || achieve.counter > 0) { return; }

        Modify_Achievement(achieve, 0);
    }

    // 재화 소모 
    public void Use_Dot_Achievement(int count) {
        var achieve = achievement[45];
        achieve.counter += count - 1;
        Modify_Achievement(achieve, 0);
    }

    /****************************** quest *************************************************/

    // 에너지 소모
    public void Use_Energy_Achievement(int count) {
        var achieve = daily_quest[2];
        if(achieve.checker < 1) {
            achieve.counter += count - 1;
            Modify_Achievement(achieve, 1);
        }


        var achieve_2 = weekly_quest[2];
        if(achieve_2.checker < 1) {
            achieve_2.counter += count - 1;
            Modify_Achievement(achieve_2, 2);
        }
        
    }

    // 골드 소모
    public void Use_Gold_Achievement(int count) {
        var achieve = daily_quest[3];
        achieve.counter += count - 1;
        Modify_Achievement(achieve, 1);

        var achieve_2 = weekly_quest[3];
        achieve_2.counter += count - 1;
        Modify_Achievement(achieve_2, 2);
    }

    // 상자 열기
    public void Open_Chest(string grade) {
        // 일일퀘 - 아무상자나 열기 이므로 등급 상관없이 카운트
        var achieve = daily_quest[4];
        if(achieve.checker < 1) {
            Modify_Achievement(achieve, 1);
        }

        if (grade.Equals("s")) {
            var achieve_2 = weekly_quest[4];
            if(achieve_2.checker < 1) {
                Modify_Achievement(achieve_2, 2);
            }
        }
        
    }

    // 라운드 클리어
    public void Clear_Round() {
        var achieve = daily_quest[5];
        if(achieve.checker < 1) 
            Modify_Achievement(achieve, 1);

        var achieve_2 = weekly_quest[5];
        if(achieve_2.checker < 1)
            Modify_Achievement(achieve_2, 2);
    }

    // S급 조합2회 하기 / ex급 3회 조합하기
    public void Combine_Achievement(string grade) {
        if (grade.Equals("s")) {
            var achieve = daily_quest[6];
            if(achieve.checker < 1) {
                Modify_Achievement(achieve, 1);
            }
        }
        else if(grade.Equals("ex")) {
            var achieve_2 = weekly_quest[6];
            if(achieve_2.checker < 1) {
                Modify_Achievement(achieve_2, 2);
            }
        }
    }

    // 조각 판매하기
    public void Sell_Achievement(int count) {
        var achieve = daily_quest[7];
        if(achieve.checker < 1) {
            achieve.counter += count - 1;
            Modify_Achievement(achieve, 1);
        }
    }

    // 임무 성공하기
    public void Mission_Complete_Achievement(string grade) {
        var achieve = daily_quest[8];
        if(achieve.checker < 1) {
            if (grade.Equals("b")) {
                Modify_Achievement(achieve, 1);
            }
        }

        var achieve_2 = weekly_quest[7];
        if(achieve_2.checker < 1) {
            if (grade.Equals("s")) {
                Modify_Achievement(achieve_2, 2);
            }
        }
    }

    // 스킵하기
    public void Skip_Achievement() {
        var achieve = daily_quest[9];
        if(achieve.checker < 1 || achieve.counter < achieve.request_counter) {
            Modify_Achievement(achieve, 1);
        }
    }

    // 출석 체크
    public void Attendence_Check() {
        // 일일 퀘스트
        Quest daily = daily_quest[1];

        // counter는 하루에 하나만 늘어나므로 0이어야 작동함
        if (daily.counter == 0) {
            daily.counter = 1;
            Utility.builder.Clear();
            query = Utility.builder.Append("UPDATE dailyquest SET counter=")
                                    .Append(daily.counter)
                                    .Append(" WHERE id=")
                                    .Append(daily.id)
                                    .ToString();
            ModifyDB.Instance.ModifySet(query, "daily");
            Utility.builder.Clear();

            // 주간 퀘스트
            Quest weekly = weekly_quest[1];
            weekly.counter += 1;
            query = Utility.builder.Append("UPDATE weeklyquest SET counter=")
                                    .Append(weekly.counter)
                                    .Append(" WHERE id=")
                                    .Append(weekly.id)
                                    .ToString();
            ModifyDB.Instance.ModifySet(query, "weekly");
            Utility.builder.Clear();
        }

        Daily_Complete_Check();
    }

    // 일간 퀘스트 총 완료 시 주간 퀘 등록
    public void Daily_Quest_All_Complete() {
        var weekly = weekly_quest[8];
        if (weekly.checker < 1) {
            Modify_Achievement(weekly, 2);
        }
    }

    /************************************************************************/

    public void Modify_Achievement(Mission achieve, int type) {
        achieve.counter += 1;
        Utility.builder.Clear();

        if(type == 0) {
            query = Utility.builder.Append("UPDATE achievement SET counter=")
                       .Append(achieve.counter)
                       .Append(" WHERE id=")
                       .Append(achieve.id)
                       .ToString();
            ModifyDB.Instance.ModifySet(query, "achievement");

        }
        else if(type == 1) {
            query = Utility.builder.Append("UPDATE dailyquest SET counter=")
                       .Append(achieve.counter)
                       .Append(" WHERE id=")
                       .Append(achieve.id)
                       .ToString();
            ModifyDB.Instance.ModifySet(query, "daily");

            Daily_Complete_Check();
        }
        else if(type == 2) {
            query = Utility.builder.Append("UPDATE weeklyquest SET counter=")
                       .Append(achieve.counter)
                       .Append(" WHERE id=")
                       .Append(achieve.id)
                       .ToString();
            ModifyDB.Instance.ModifySet(query, "weekly");

            Weekly_Complete_Check();
        }
        Utility.builder.Clear();
    }

    public void Daily_Complete_Check() {

        int counter = 0;
        Quest daily;
        int size = daily_quest.Count;
        for (int i = 1; i < size; i++) {
            daily = daily_quest[i];
            if (daily.checker == 1) {
                counter++;
            }
        }
        Utility.builder.Clear();
        daily = daily_quest[size];
        daily.counter = counter;
        query = Utility.builder.Append("UPDATE dailyquest SET counter=")
                               .Append(daily.counter)
                               .Append(" WHERE id=")
                               .Append(size)
                               .ToString();
        Utility.builder.Clear();
        ModifyDB.Instance.ModifySet(query, "daily");
    }

    public void Weekly_Complete_Check() {
        int counter = 0;
        Quest weekly;
        int size = weekly_quest.Count;
        for (int i = 1; i < size; i++) {
            weekly = weekly_quest[i];
            if (weekly.checker == 1) {
                counter++;
            }
        }
        Utility.builder.Clear();
        weekly = daily_quest[size];
        weekly.counter = counter;
        query = Utility.builder.Append("UPDATE weeklyquest SET counter=")
                               .Append(weekly.counter)
                               .Append(" WHERE id=")
                               .Append(size)
                               .ToString();
        Utility.builder.Clear();
        ModifyDB.Instance.ModifySet(query, "weekly");
    }

}
