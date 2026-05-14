using System.Collections.Generic;
using System.Data;
using System;
using UnityEngine;
using System.IO;
using CUSTOM_DATA;
using System.Collections;

using Mono.Data.Sqlite;

//using UnityEngine.Android;
using TMPro;

public class ConnectDB : Singleton<ConnectDB>
{
    private struct Mission_Carrier { 
        public int id;
        public int checker;
        public int counter;

        public Mission_Carrier(int id, int checker, int counter) {
            this.id = id;
            this.checker = checker;
            this.counter = counter;
        }
    }

    private IDbConnection connector;
    private IDbCommand command;
    private IDataReader reader;

    delegate void CONNECT_DB_DELEGATE();
    delegate void DB_CHECK_DELEGATE();
    CONNECT_DB_DELEGATE connect_db_delegate;
    DB_CHECK_DELEGATE db_check_delegate;

    bool updating_achievement = false;
    bool updating_daily_quest = false;
    bool updating_weekly_quest = false;

    readonly List<Mission_Carrier> achieve_list = new();
    readonly List<Mission_Carrier> daily_list = new();
    readonly List<Mission_Carrier> weekly_list = new();

    bool first_check = false;

    void Start()
    {
        Game_State_Data.db_state = DB_STATE.Connecting;

        // 화면 안꺼지게 설정 
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        // 화면 fps 60 고정
        Application.targetFrameRate = 60;
        //PlayerPrefs.DeleteAll();

        // stamina json 확인
        string stamina_json = PlayerPrefs.GetString("Stamina_Json", "");

        if (stamina_json == "{}" || stamina_json == "") {
            // 새로 생성 후 저장
            StaminaClass tmp_stamin_class = new();
            tmp_stamin_class.Add_Stamina(20);
            tmp_stamin_class.Max_Increase();
            tmp_stamin_class.Max_Increase();
            tmp_stamin_class.Max_Increase();
            tmp_stamin_class.Max_Increase();
            tmp_stamin_class.Max_Increase();
            stamina_json = JsonUtility.ToJson(tmp_stamin_class, true);
            Debug.Log(stamina_json);
            Utility.SaveStaminaStorage();
        }

        Game_State_Data.stamina_storage = JsonUtility.FromJson<StaminaClass>(stamina_json);

        connector = new SqliteConnection(Game_Value_Data.DB_CONNECT_STRING);

        db_check_delegate += Check_Achievement;
        db_check_delegate += Check_Daily_Quest;
        db_check_delegate += Check_Weekly_Quest;

        connect_db_delegate += DB_CHECK;
        connect_db_delegate += Connect_UserDB;
        connect_db_delegate += Connect_UnitDB;
        connect_db_delegate += Connect_EnemyDB;
        connect_db_delegate += Connect_Achievement;
        connect_db_delegate += Connect_Weekly_Quest;
        connect_db_delegate += Connect_Daily_Quest;
        connect_db_delegate += End_Observing;

        if(!first_check) connect_db_delegate();
    }


    public void DB_CHECK() {
        
        string default_path = Application.persistentDataPath + "/ShapeKeeperDB.db";

        #region file_create_or_copy
        if (Application.platform == RuntimePlatform.Android) {
            // 파일 검사
            try {
                if (!File.Exists(default_path)) {
                    WWW temp_load_db = new("jar:file://" + Application.dataPath + "!/assets/ShapeKeeperDB.db");
                    while (!temp_load_db.isDone) { };
                    File.WriteAllBytes(default_path, temp_load_db.bytes);
                }
            }
            catch (Exception err) {
                Debug.LogError(err.Message);
            }
        }
        else {
            // 파일 검사
            if (!File.Exists(default_path)) {
                File.Copy(Application.streamingAssetsPath + "/ShapeKeeperDB.db", default_path);
            }
        }
        #endregion

        try {
            connector.Open();
        }
        catch (Exception err) {
            Debug.LogError(err.Message);
            // open이 안되면 이하는 어짜피 불가능
            return;
        }
        command = connector.CreateCommand();

        // db가 비어있는지 확인하는 방법?
        command.CommandText = "SELECT count(*) FROM sqlite_master WHERE type = 'table'";
        reader = command.ExecuteReader();
        int table_count = 0;
        while (reader.Read()) {
            table_count = reader.GetInt32(0);
        }
        reader.Close();
        command.Dispose();
        connector.Close();

        db_check_delegate();
        first_check = true;
    }

    private void Check_Achievement() {
        connector.Open();
        // achievement
        command = connector.CreateCommand();
        command.CommandText = "SELECT count(*) FROM sqlite_master WHERE Name = 'achievement'";
        reader = command.ExecuteReader();
        int table_count = -1;
        while (reader.Read()) {
            table_count = reader.GetInt32(0);
        }
        reader.Close();
        if (table_count < 1) Create_Achievement();
        else {
            command.CommandText = "SELECT count(*) FROM achievement";
            reader = command.ExecuteReader();
            table_count = -1;
            while (reader.Read()) {
                table_count = reader.GetInt32(0);
            }
            if(table_count == 0) {
                Create_Achievement(); 
            }
            else if (table_count != 50) {
                // 정보를 임시로 저장하고
                // 저장은 id, checker, counter만 해두면 됨
                // 업적 데이터 받아오기
                command.CommandText = "SELECT * FROM achievement";
                IDataReader dataReader = command.ExecuteReader();
                achieve_list.Clear();
                while (dataReader.Read()) {
                    int id = dataReader.GetInt32(0);
                    //string name = dataReader.GetString(1);
                    //string check_reward_string = dataReader.GetString(2);
                    int checker = dataReader.GetInt32(3);
                    int counter = dataReader.GetInt32(4);

                    achieve_list.Add(new(id, checker, counter));
                }
                // drop 하고
                command.CommandText = "DROP TABLE achievement";
                command.ExecuteNonQuery();
                // create 한다음
                Create_Achievement();
                // 임시 저장된 정보를 옮김
                updating_achievement = true;
                dataReader.Close();
            }
        }
        reader.Close();
        command.Dispose();
        connector.Close();
    }

    private void Check_Daily_Quest() {
        connector.Open();
        // daily quest
        command = connector.CreateCommand();
        command.CommandText = "SELECT count(*) FROM sqlite_master WHERE Name = 'dailyquest'";
        reader = command.ExecuteReader();
        int table_count = -1;
        while (reader.Read()) {
            table_count = reader.GetInt32(0);
        }
        reader.Close();
        if (table_count < 1) Create_Daily_Quest();
        else {
            command.CommandText = "SELECT count(*) FROM dailyquest";
            reader = command.ExecuteReader();
            table_count = -1;
            while (reader.Read()) {
                table_count = reader.GetInt32(0);
            }
            reader.Close();
            if (table_count == 0) {
                Create_Daily_Quest();
            }
            else if (table_count != 10) {
                command.CommandText = "SELECT * FROM dailyquest";
                IDataReader dataReader = command.ExecuteReader();
                daily_list.Clear();

                while (dataReader.Read()) {
                    int id = dataReader.GetInt32(0);
                    //string name = dataReader.GetString(1);
                    //string check_reward_string = dataReader.GetString(2);
                    int checker = dataReader.GetInt32(3);
                    int counter = dataReader.GetInt32(4);

                    daily_list.Add(new(id, checker, counter));
                }

                dataReader.Close();
                // drop 하고
                command.CommandText = "DROP TABLE dailyquest";
                command.ExecuteNonQuery();
                // create 한다음
                Create_Daily_Quest();
            }
            // 임시 저장된 정보를 옮김
            updating_daily_quest = true;
        }
        reader.Close();
        command.Dispose();
        connector.Close();
    }

    private void Check_Weekly_Quest() {
        connector.Open();
        // weekly quest
        command = connector.CreateCommand();
        command.CommandText = "SELECT count(*) FROM sqlite_master WHERE Name = 'weeklyquest'";
        reader = command.ExecuteReader();
        int table_count = -1;
        while (reader.Read()) {
            table_count = reader.GetInt32(0);
        }
        reader.Close();
        if (table_count < 1) Create_Weekly_Quest();
        else {
            command.CommandText = "SELECT count(*) FROM weeklyquest";
            reader = command.ExecuteReader();
            table_count = -1;
            while (reader.Read()) {
                table_count = reader.GetInt32(0);
            }
            reader.Close();

            if (table_count == 0) {
                Create_Weekly_Quest();
            }
            else if (table_count != 10) {
                command.CommandText = "SELECT * FROM weeklyquest";
                IDataReader dataReader = command.ExecuteReader();
                weekly_list.Clear();

                while (dataReader.Read()) {
                    int id = dataReader.GetInt32(0);
                    //string name = dataReader.GetString(1);
                    //string check_reward_string = dataReader.GetString(2);
                    int checker = dataReader.GetInt32(3);
                    int counter = dataReader.GetInt32(4);

                    weekly_list.Add(new(id, checker, counter));
                }
                dataReader.Close();
                // drop 하고
                command.CommandText = "DROP TABLE weeklyquest";
                command.ExecuteNonQuery();
                // create 한다음
                Create_Weekly_Quest();
                // 임시 저장된 정보를 옮김
                updating_weekly_quest = true;
            }
        }
        reader.Close();
        command.Dispose();
        connector.Close();
    }

    private void Create_Achievement() {
        //Debug.Log("create_achievement active");
        command.CommandText = "CREATE TABLE achievement('id' INT NOT NULL UNIQUE, 'name' CHAR NOT NULL, 'reward' CHAR NOT NULL, 'checker' INT NOT NULL DEFAULT 0, 'counter' INT NOT NULL DEFAULT 0, PRIMARY KEY(id))";
        command.ExecuteNonQuery();

        #region achievement
        command.CommandText = "INSERT INTO achievement VALUES (1, '첫 10라운드 클리어', '1500_gold', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (2, '첫 20라운드 클리어', '1500_gold', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (3, '첫 30라운드 클리어', '1500_gold', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (4, '첫 40라운드 클리어', '1500_gold', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (5, '첫 50라운드 클리어', '1500_gold', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (6, '첫 60라운드 클리어', '1500_gold', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (7, '첫 70라운드 클리어', '2000_gold', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (8, '첫 80라운드 클리어', '2000_gold', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (9, '첫 90라운드 클리어', '2000_gold', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (10, '첫 100라운드 클리어', '3000_gold', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (11, '첫 110라운드 클리어', '3000_gold', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (12, '첫 120라운드 클리어', '3000_gold', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (13, '첫 난이도 1 클리어', '40_d', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (14, '첫 난이도 2 클리어', '40_d+20_c', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (15, '첫 난이도 3 클리어', '40_d+20_c+10_b', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (16, '첫 난이도 4 클리어', '40_d+30_c+20_b+10_a', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (17, '첫 난이도 5 클리어', '40_d+40_c+30_b+20_a+10_s', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (18, '첫 난이도 6 클리어', '40_d+40_c+40_b+30_a+20_s+10_ex', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (19, '첫 난이도 7 클리어', '50_d+40_c+40_b+40_a+30_s+20_ex', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (20, '첫 난이도 8 클리어', '50_d+50_c+40_b+40_a+40_s+30_ex', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (21, '첫 난이도 9 클리어', '50_d+50_c+50_b+40_a+40_s+40_ex', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (22, '첫 난이도 10 클리어', '60_d+50_c+50_b+50_a+40_s+40_ex', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (23, '첫 난이도 11 클리어', '60_d+60_c+50_b+50_a+50_s+40_ex', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (24, '첫 난이도 12 클리어', '60_d+60_c+60_b+50_a+50_s+50_ex', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (25, '첫 난이도 13 클리어', '60_d+60_c+60_b+60_a+50_s+50_ex', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (26, '첫 난이도 14 클리어', '60_d+60_c+60_b+60_a+60_s+50_ex', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (27, '첫 난이도 15 클리어', '60_d+60_c+60_b+60_a+60_s+60_ex', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (28, '10라운드 보스 n회 반복 클리어', '10.n_gold+10.n', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (29, '20라운드 보스 n회 반복 클리어', '10.n_gold+10.n', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (30, '30라운드 보스 n회 반복 클리어', '10.n_gold+10.n', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (31, '40라운드 보스 n회 반복 클리어', '10.n_gold+10.n', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (32, '50라운드 보스 n회 반복 클리어', '10.n_gold+10.n', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (33, '60라운드 보스 n회 반복 클리어', '10.n_gold+10.n', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (34, '70라운드 보스 n회 반복 클리어', '10.n_gold+10.n', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (35, '80라운드 보스 n회 반복 클리어', '10.n_gold+10.n', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (36, '90라운드 보스 n회 반복 클리어', '10.n_gold+10.n', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (37, '100라운드 보스 n회 반복 클리어', '10.n_gold+10.n', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (38, '110라운드 보스 n회 반복 클리어', '10.n_gold+10.n', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (39, '120라운드 보스 n회 반복 클리어', '10.n_gold+10.n', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (40, '강화 n회 완료', '100.n_gold+10.n', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (41, '유닛 생성 n회 완료', '10.n_gold+100.n', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (42, '1000점 가지고 있기', '10000_gold', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (43, '3000점 가지고 있기', '30000_gold', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (44, '5000점 가지고 있기', '50000_gold', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (45, 'n점 소모하기', '1.n_gold+1000.n', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (46, '미션 1 반복 클리어', '10.n_gold+10.n', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (47, '미션 2 반복 클리어', '15.n_gold+10.n', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (48, '미션 3 반복 클리어', '20.n_gold+10.n', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (49, '미션 4 반복 클리어', '25.n_gold+10.n', 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO achievement VALUES (50, '미션 5 반복 클리어', '30.n_gold+10.n', 0, 0)";
        command.ExecuteNonQuery();
        #endregion
    }

    private void Create_Daily_Quest() {
        //Debug.Log("create daily quest active");
        command.CommandText = "CREATE TABLE dailyquest('id' INT NOT NULL, 'name' CHAR NOT NULL, 'reward' CHAR NOT NULL DEFAULT 100, 'checker' INT NOT NULL DEFAULT 0, 'counter' INT NOT NULL DEFAULT 0, 'requestcounter' INT NOT NULL DEFAULT 0, PRIMARY KEY(id))";
        command.ExecuteNonQuery();

        #region daily quest
        command.CommandText = "INSERT INTO dailyquest VALUES (1, '출석하기', '1000_gold+10_exp', 0, 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO dailyquest VALUES (2, '에너지 20 소모', '1000_gold+10_exp', 0, 0, 20)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO dailyquest VALUES (3, '1000골드 소모하기', '1000_gold+10_exp', 0, 0, 1000)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO dailyquest VALUES (4, '아무 상자나 1회 열기', '1000_gold+10_exp', 0, 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO dailyquest VALUES (5, '100라운드 클리어', '1000_gold+10_exp', 0, 0, 100)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO dailyquest VALUES (6, 'S급 2회 조합하기', '1000_gold+10_exp', 0, 0, 2)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO dailyquest VALUES (7, '조각 3개 판매하기', '1000_gold+10_exp', 0, 0, 3)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO dailyquest VALUES (8, 'B급 임무 1회 성공하기', '1000_gold+10_exp', 0, 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO dailyquest VALUES (9, '5회 스킵하기', '1000_gold+10_exp', 0, 0, 5)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO dailyquest VALUES (10, '모든 퀘스트 완료하기', '1000_gold+10_exp', 0, 0, 9)";
        command.ExecuteNonQuery();
        #endregion
    }

    private void Create_Weekly_Quest() {
        //Debug.Log("Create weekly quest active");
        command.CommandText = "CREATE TABLE weeklyquest('id' INT NOT NULL, 'name' CHAR NOT NULL, 'reward' CHAR NOT NULL DEFAULT '3000_gold+30_exp', 'checker' INT NOT NULL DEFAULT 0, 'counter' INT NOT NULL DEFAULT 0, 'requestcounter' INT NOT NULL DEFAULT 0, PRIMARY KEY(id))";
        command.ExecuteNonQuery();

        #region weekly quest
        command.CommandText = "INSERT INTO weeklyquest VALUES (1, '4회 출석하기', '3000_gold+30_exp', 0, 0, 4)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO weeklyquest VALUES (2, '에너지 100 소모', '3000_gold+30_exp', 0, 0, 100)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO weeklyquest VALUES (3, '5만 골드 소모하기', '3000_gold+30_exp', 0, 0, 50000)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO weeklyquest VALUES (4, 'S급 상자 2회 열기', '3000_gold+30_exp', 0, 0, 2)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO weeklyquest VALUES (5, '종합 10000라운드 클리어', '3000_gold+30_exp', 0, 0, 10000)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO weeklyquest VALUES (6, 'EX급 3회 조합하기', '3000_gold+30_exp', 0, 0, 3)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO weeklyquest VALUES (7, 'S급 임무 1회 성공하기', '3000_gold+30_exp', 0, 0, 0)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO weeklyquest VALUES (8, '일일 퀘스트 4회 완료하기 ', '3000_gold+30_exp', 0, 0, 4)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO weeklyquest VALUES (9, '강화 3회 하기', '3000_gold+30_exp', 0, 0, 3)";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO weeklyquest VALUES (10, '모든 목표 완료하기', '3000_gold+30_exp', 0, 0, 9)";
        command.ExecuteNonQuery();
        #endregion

    }

    public void Connect_UserDB() {
        connector.Open();
        command = connector.CreateCommand();
        command.CommandText = "SELECT * FROM user";
        reader = command.ExecuteReader();
        User _user = new();
        while (reader.Read()){
            
            int level = reader.GetInt32(0);
            int experience = reader.GetInt32(1);
            int gold = reader.GetInt32(2);
            int skil_point = reader.GetInt32(3);
            int left_skill_point = reader.GetInt32(4);
            
            string tmp = reader.GetString(5);
            int[] invested = new int[5];
            invested[0] = int.Parse(tmp.Split(",")[0]);
            invested[1] = int.Parse(tmp.Split(",")[1]);
            invested[2] = int.Parse(tmp.Split(",")[2]);
            invested[3] = int.Parse(tmp.Split(",")[3]);
            invested[4] = int.Parse(tmp.Split(",")[4]);

            _user.Init(level, experience, gold, skil_point, left_skill_point, invested);
            break;
        }
        Game_Data.user = _user;
        reader.Close();
        connector.Close();
    }

    public void Connect_UnitDB() {
        connector.Open();
        command = connector.CreateCommand();
        // unit table의 데이터를 unit 에 옮기기
        command.CommandText = "SELECT * FROM unit";
        reader = command.ExecuteReader();
        bool recent_db_count_checker = false;
        int id = 0;
        //ArrayList array = new() { empty_unit };
        Dictionary<int, Unit> dic = new();
        Dictionary<int, Unit_Count_Checker> dic2 = new();

        while (reader.Read()) {
            if (reader.FieldCount >= 12) {
                recent_db_count_checker = true;
            }

            Unit unit = new();
            // 최신 버전일경우
            if (recent_db_count_checker) {
                unit.id = reader.GetInt32(0);
                unit.nick_name= reader.GetString(1);
                unit.name = reader.GetString(2);
                unit.attack = reader.GetInt32(3);
                unit.speed = reader.GetInt32(4);
                unit.material = reader.GetInt32(5);
                unit.ethereal = reader.GetInt32(6);
                unit.piece = reader.GetInt32(7);
                unit.grade = reader.GetString(8);
                unit.type = reader.GetString(9);
                unit.upgrade = reader.GetInt32(10);
                // 조합식 정제
                string tmp = reader.GetString(11);
                if (tmp != "0") {
                    string[] parts = tmp.Split(",");
                    int max = parts.Length;
                    List<CombineFunction> list = new(max + 1);
                    foreach (string part in parts) {
                        CombineFunction tmp_cmp = new();
                        string[] one_part = part.Split("_");
                        // 조합 결과
                        tmp_cmp.result = int.Parse(one_part[1]);
                        tmp_cmp.unit_id = unit.id;
                        // 조합에 나 이외에 추가로 필요한 유닛 번호
                        string[] details = one_part[0].Split("^");
                        int count = details.Length;
                        int tmp_val = 0;

                        tmp_val = int.Parse(details[0]);
                        if (tmp_val == 101) tmp_cmp.piece += 1;
                        else if (tmp_val == 102) tmp_cmp.crystal += 1;
                        else tmp_cmp.a = tmp_val;

                        if(count >= 2) {
                            tmp_val = int.Parse(details[1]);
                            if (tmp_val == 101) tmp_cmp.piece += 1;
                            else if (tmp_val == 102) tmp_cmp.crystal += 1;
                            else tmp_cmp.b = tmp_val;
                        }

                        if(count >= 3) {
                            tmp_val = int.Parse(details[2]);
                            if (tmp_val == 101) tmp_cmp.piece += 1;
                            else if (tmp_val == 102) tmp_cmp.crystal += 1;
                            else tmp_cmp.c = tmp_val;
                        }

                        // 4부터는 조각 or 진화의 돌만 나타남
                        if(count >= 4) {
                            tmp_val = int.Parse(details[3]);
                            if (tmp_val == 101) tmp_cmp.piece += 1;
                            else if (tmp_val == 102) tmp_cmp.crystal += 1;
                        }

                        if(count >= 5) {
                            tmp_val = int.Parse(details[4]);
                            if (tmp_val == 101) tmp_cmp.piece += 1;
                            else if (tmp_val == 102) tmp_cmp.crystal += 1;
                        }

                        tmp_cmp.need_count = count;
                        tmp_cmp.Set_Function();
                        // 조합식을 어레이에 저장
                        tmp_cmp.id = id;
                        id++;
                        list.Add(tmp_cmp);
                    }
                    // 정제된 arraylist를 list로 저장
                    unit.combine_function = list;
                }
                else {
                    unit.combine_function= new List<CombineFunction>(0);
                }

                dic.Add(unit.id, unit);
                dic2.Add(unit.id, new(0,false));
            }
          
        }

        Game_Data.unit = dic;
        Game_Data.unit_counter = dic2;

        reader.Close();
        connector.Close();
    }

    public void Connect_EnemyDB() {
        connector.Open();
        command = connector.CreateCommand();
        // unit table의 데이터를 unit 에 옮기기
        command.CommandText = "SELECT * FROM enemy";
        reader = command.ExecuteReader();

        //ArrayList array = new() { empty_unit };
        Dictionary<int, Enemy> dic = new();

        while (reader.Read()) {
            Enemy enemy;
            int id = reader.GetInt32(0);
            Enemy_Type type = Utility.Get_Enemy_Type(reader.GetString(1));
            float speed = reader.GetFloat(2);
            int hp_val = reader.GetInt32(3);
            int material = reader.GetInt32(4);
            int ethereal = reader.GetInt32(5);
            enemy = new(id, type, speed, hp_val, material, ethereal);

            dic.Add(enemy.id, enemy);
        }

        Game_Data.enemy = dic;

        reader.Close();
        connector.Close();
    }

    public void Connect_Achievement() {
        connector.Open();
        command = connector.CreateCommand();
        // 업적 데이터 받아오기
        command.CommandText = "SELECT * FROM achievement";
        IDataReader dataReader = command.ExecuteReader();
        // 0 번에 빈 값 넣기
        Dictionary<int, Achievement> temp_achievement_list = new();

        while (dataReader.Read()) {
            Achievement achievement = new() {
                id = dataReader.GetInt32(0),
                name = dataReader.GetString(1),
            };
            /*
             * +로 다수의 보상 구분
             * _로 reward 내용과 내용에 따른 보상 액 구분
             * . 으로 반복에 따른 배수 구분
             *   -> .이 있으면 .앞의 값 * checker를 reward val에 넣기
             * , 로 반복 달성 필요량의 증가분 구분
             *   -> , 가 있으면 매치값으로 구분 repeat를 true로 설정하고 
             *   -> reward_val[checker]에 해당하는 값이 필요 reward 값으로 들어가고
             * */
            string check_reward_string = dataReader.GetString(2);
            string[] rewards = check_reward_string.Split('+');
            int checker = dataReader.GetInt32(3);
            int counter = dataReader.GetInt32(4);
            int end_time = 1;
            int endless_value = 0;

            ArrayList reward_list = new();
            ArrayList reward_val = new();
            // reward 가공
            // .가 있으면 반복 보상이므로 여러 보상과는 다르게 가공
            if (check_reward_string.Contains(".")) {
                reward_list.Add("");
                reward_val.Add(0);
                achievement.repeat = true;
                // reward[0] == 보상 
                // reward[1] == checker에 따라 요구될 조건 

                // 보상 확인
                string[] temp_reward = rewards[0].Split("_");
                string[] reward_coefficient = temp_reward[0].Split(".");
                // 대부분 골드
                reward_list[0] = temp_reward[1];
                if (checker == 0) {
                    reward_val[0] = int.Parse(reward_coefficient[0]);
                }
                else {
                    reward_val[0] = int.Parse(reward_coefficient[0]) * checker;
                }

                // 조건 확인
                // 모든 반복퀘는 무한반복퀘
                if (rewards[1].Contains(".")) {
                    int reward_request_coefficient = int.Parse(rewards[1].Split(".")[0]);

                    end_time = -1;
                    endless_value = reward_request_coefficient;

                }
            }
            // ,가 없으면 +로 연결된 다수의 보상 or 단일 보상
            else {
                // 다수
                if (rewards.Length >= 2) {
                    // 각 리워드를 순서에 맞게 저장 -> 1번부터 (0번은 더미값)
                    int rewards_size = rewards.Length;
                    for (int i = 0; i < rewards_size; i++) {
                        string[] single_reward = rewards[i].Split("_");
                        reward_val.Add(int.Parse(single_reward[0]));
                        reward_list.Add(single_reward[1]);
                    }
                }
                // 단일
                else {
                    string[] single_reward = rewards[0].Split("_");
                    reward_val.Add(int.Parse(single_reward[0]));
                    reward_list.Add(single_reward[1]);
                }
            }

            // 보상을 받을 수 있는지 확인
            // 반복 보상
            if (achievement.repeat) {
                // 무한반복퀘 / 일반 반복퀘를 나눔
                int for_reward = endless_value * (checker + 1);

                //Debug.Log(achievement.Name + " >> for_reward : " + for_reward + "   // counter : " + counter);
                achievement.can_recive = counter >= for_reward && counter != 0;
            }
            //단일보상
            else {
                // counter 만 1 이상이라면 받을 수 있어야함
                // checker가 0 이 아니면 이미 완료된 업적이므로 받을 수 없어야함
                achievement.can_recive = counter > 0 && checker == 0;
            }

            // 보상 가공완료
            achievement.counter = counter;
            achievement.checker = checker;
            achievement.reward_list = reward_list;
            achievement.reward_val = reward_val;
            achievement.endless_value = endless_value;
            achievement.end_time = end_time;

            // 가공된 자료를 리스트에 저장
            temp_achievement_list.Add(achievement.id, achievement);
        }

        // 저장
        Game_Data.achievement = temp_achievement_list;

        if (updating_achievement) {
            foreach(var item in achieve_list) {
                Game_Data.achievement[item.id].counter = item.counter;
                Game_Data.achievement[item.id].checker = item.checker;
            }

            achieve_list.Clear();
            updating_achievement = false;
        }

        dataReader.Close();
        reader.Close();
        connector.Close();
    }

    public void Connect_Daily_Quest() {
        connector.Open();
        command = connector.CreateCommand();
        /*
        // 업적 데이터 받아오기
        command.CommandText = "SELECT COUNT(*) FROM dailyquest";

        int count = Convert.ToInt32(command.ExecuteScalar());
        if (count != 10) {
            //drop 하고 다시 넣기
            command.CommandText = "DROP TABLE dailyquest";
            command.ExecuteNonQuery();
            Create_Daily_Quest();
            Connect_Daily_Quest();
            return;
        }
        */
        command.CommandText = "SELECT * FROM dailyquest";
        IDataReader dataReader = command.ExecuteReader();

        // 0 번에 빈 값 넣기
        Dictionary<int, Quest> temp_list = new();
        bool incorrect_date_check = false;
        while (dataReader.Read()) {
            Quest daily = new(){
                id = dataReader.GetInt32(0),
                name = dataReader.GetString(1),
            };
            string[] reward = dataReader.GetString(2).Split("+");
            if(reward.Length != 2) {
                incorrect_date_check = true;
                break; 
            }
            int checker = dataReader.GetInt32(3);
            int counter = dataReader.GetInt32(4);
            int request = dataReader.GetInt32(5);

            // 보상 정리
            // 퀘스트는 골드와 경험치만 보상으로 주어짐
            ArrayList reward_list = new() { "", "" };
            ArrayList reward_val = new() { 0, 0 };

            string[] tmp = reward[0].Split("_");
            reward_list[0] = tmp[1];
            reward_val[0] = int.Parse(tmp[0]);

            tmp = reward[1].Split("_");
            reward_list[1] = tmp[1];
            reward_val[1] = int.Parse(tmp[0]);

            // 보상 요구치가 0이 아니면 counter가 request와 동일해야 받을 수 있음
            daily.can_recive = request > 0 ? counter >= request && checker == 0 : counter > 0 && checker == 0;

            daily.counter = counter;
            daily.checker = checker;
            daily.request_counter= request;
            daily.reward_val = reward_val;
            daily.reward_list = reward_list;
            daily.ended = daily.checker == 1;

            temp_list.Add(daily.id, daily);
        }
        dataReader.Close();
        Game_Data.daily_quest = temp_list;

        if (incorrect_date_check) {
            // 데이터가 잘못된 것이므로 다 지워버리고 다시 connect
            // drop 하고
            command.CommandText = "DROP TABLE dailyquest";
            command.ExecuteNonQuery();
            // create 한다음
            Create_Daily_Quest();
            connector.Close();

            Connect_Daily_Quest();
            return;
        }

        if (updating_daily_quest) {
            foreach (var item in daily_list) {
                Game_Data.daily_quest[item.id].counter = item.counter;
                Game_Data.achievement[item.id].checker = item.checker;
            }

            daily_list.Clear();
            updating_daily_quest = false;
        }
        
        reader.Close();
        connector.Close();
    }

    public void Connect_Weekly_Quest() {
        connector.Open();
        command = connector.CreateCommand();
        // 업적 데이터 받아오기
        command.CommandText = "SELECT * FROM weeklyquest";
        IDataReader dataReader = command.ExecuteReader();
        // 0 번에 빈 값 넣기
        Dictionary<int, Quest> temp_list = new();

        while (dataReader.Read()) {
            Quest weekly = new() {
                id = dataReader.GetInt32(0),
                name = dataReader.GetString(1),
            };
            string[] reward = dataReader.GetString(2).Split("+");
            int checker = dataReader.GetInt32(3);
            int counter = dataReader.GetInt32(4);
            int request = dataReader.GetInt32(5);

            // 보상 정리
            // 퀘스트는 골드와 경험치만 보상으로 주어짐
            ArrayList reward_list = new() { "", "" };
            ArrayList reward_val = new() { 0, 0 };

            string[] tmp = reward[0].Split("_");
            reward_list[0] = tmp[1];
            reward_val[0] = int.Parse(tmp[0]);

            tmp = reward[1].Split("_");
            reward_list[1] = tmp[1];
            reward_val[1] = int.Parse(tmp[0]);

            // 보상 요구치가 0이 아니면 counter가 request와 동일해야 받을 수 있음
            weekly.can_recive = request > 0 ? counter >= request && checker == 0 : counter > 0 && checker == 0;

            weekly.counter = counter;
            weekly.checker = checker;
            weekly.request_counter = request;
            weekly.reward_val = reward_val;
            weekly.reward_list = reward_list;
            weekly.ended = weekly.checker == 1;

            temp_list.Add(weekly.id, weekly);
        }

        Game_Data.weekly_quest = temp_list;

        if (updating_weekly_quest) {
            foreach (var item in weekly_list) {
                Game_Data.weekly_quest[item.id].counter = item.counter;
                Game_Data.weekly_quest[item.id].checker = item.checker;
            }

            weekly_list.Clear();
            updating_weekly_quest = false;
        }

        dataReader.Close();
        reader.Close();
        connector.Close();
    }

    public void End_Observing() {
        Game_State_Data.db_state = DB_STATE.Usable;
        Stamina_Observer.Instance.Observing();
    }
}