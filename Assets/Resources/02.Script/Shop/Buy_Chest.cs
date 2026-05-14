using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Common_Data;
using CUSTOM_DATA;
using System.Collections.Generic;
using DG.Tweening;

public class Buy_Chest : Scene_Singleton<Buy_Chest>
{
    [SerializeField] private GameObject alert_panel;
    [SerializeField] private GameObject result_panel;
    [SerializeField] private GameObject result_content;
    [SerializeField] private GameObject border;
    [SerializeField] private GameObject confirm_btn;
    [SerializeField] private GameObject percent_area;
    [SerializeField] private GameObject retry_btn;

    private readonly string R_GRADE_CHEST_NAME = "랜덤 상자";
    private readonly string D_GRADE_CHEST_NAME = "D급 상자";
    private readonly string C_GRADE_CHEST_NAME = "C급 상자";
    private readonly string B_GRADE_CHEST_NAME = "B급 상자";
    private readonly string A_GRADE_CHEST_NAME = "A급 상자";
    private readonly string S_GRADE_CHEST_NAME = "S급 상자";

    private int chest_value = 0;
    private string grade;
    public bool free_chest = false;
    public bool Chest_Setting = false;

    private Vector3 close = new(90, 0, 0);

    // d  d70 c20 b10 
    // c  d60 c30 b10 
    // b  d50 c30 b15 a5 
    // a  d40 c30 b15 a13 s2 
    // s  d30 c30 b20 a13 s5 ex2
    private readonly Dictionary<string, (int count, int[] percents)> chest_state = new() {
        { "d", (10, new[] { 700, 900, 1000, 0, 0, 0 }) },
        { "c", (20, new[] { 600, 900, 1000, 0, 0, 0 }) },
        { "r", (20, new[] { 600, 900, 1000, 0, 0, 0 }) },
        { "b", (30, new[] { 500, 800, 950, 1000, 0, 0 }) },
        { "a", (50, new[] { 400, 700, 850, 980, 1000, 0 }) },
        { "s", (100, new[] { 300, 600, 800, 930, 980, 1000 }) },
    };
    private int[] Normalize_chest_percent(int[] percents) {
        int[] weights = new int[percents.Length];
        int prev = 0;

        for (int i = 0; i < percents.Length; i++) {
            if (percents[i] == 0) {
                weights[i] = 0;
            }
            else {
                weights[i] = percents[i] / 10 - prev;
                prev += weights[i];
            }
        }

        return weights;
    }

    public void Setting_Buy_Grade_Chest(string _grade) {
        grade = _grade;

        chest_value = grade switch {
            "d" => 1000,
            "c" => 5000,
            "b" => 10000,
            "a" => 15000,
            "s" => 30000,
            "r" => 0,
            _ => 9999999
        };

        // r(free chest) time check
        if (grade.Equals("r")) {
            System.DateTime now = System.DateTime.Now;
            
            System.DateTime end_time = System.Convert.ToDateTime( PlayerPrefs.GetString("Free_Unit_Chest_RemainTime") == "" ? now : PlayerPrefs.GetString("Free_Unit_Chest_RemainTime") );
            System.TimeSpan dif = now - end_time;

            int sec_dif = (int)System.Math.Round(dif.TotalSeconds);

            // 3시간이 지나지 않았으면 받기 불가
            if (sec_dif > 0 && sec_dif < 10800) {
                //Debug.Log(min_dif);
                // 받기 불가 안내를 따로 해주도록 합시다.
                // 180 -  min dif를 시간으로 표현해봅시다.
                //Debug.Log(Utility.FormatLeftTime(10800 - sec_dif));
                Alert(Utility.FormatLeftTime(10800 - sec_dif));
                // announce alert
                return;
            }

            free_chest = true;
        }

        // setting confirm panel
        border.transform.Find("buy_announce").GetComponent<TMP_Text>().text = grade switch {
            "d" => D_GRADE_CHEST_NAME,
            "c" => C_GRADE_CHEST_NAME,
            "b" => B_GRADE_CHEST_NAME,
            "a" => A_GRADE_CHEST_NAME,
            "s" => S_GRADE_CHEST_NAME,
            "r" => R_GRADE_CHEST_NAME,
            _ => "오류상자"
        };
        border.transform.Find("buy_item").GetComponent<Image>().sprite = chest_sprite;
        border.transform.Find("buy_item").GetComponent<Image>().color = Utility.Get_Grade_Color(grade);

        border.transform.Find("price").GetComponent<TMP_Text>().text = chest_value.ToString();
        confirm_btn.GetComponent<Button>().onClick.RemoveAllListeners();

        // 확률 세팅
        int[] tmp_percents = { 0, 0, 0, 0, 0, 0 };
        int count = 0;
        (count , tmp_percents )= chest_state[grade];
        var percents = Normalize_chest_percent(tmp_percents);

        percent_area.transform.Find("D").GetComponent<TMP_Text>().text = percents[0].ToString() + "%";
        percent_area.transform.Find("C").GetComponent<TMP_Text>().text = percents[1].ToString() + "%";
        percent_area.transform.Find("B").GetComponent<TMP_Text>().text = percents[2].ToString() + "%";
        percent_area.transform.Find("A").GetComponent<TMP_Text>().text = percents[3].ToString() + "%";
        percent_area.transform.Find("S").GetComponent<TMP_Text>().text = percents[4].ToString() + "%";
        percent_area.transform.Find("EX").GetComponent<TMP_Text>().text = percents[5].ToString() + "%";
        percent_area.SetActive(true);

        if (user.gold >= chest_value) {
            confirm_btn.GetComponent<Image>().color = Color.white;
            confirm_btn.GetComponent<Button>().onClick.AddListener(() => {
                Buy_Chest_Confirm();
            });
        }
        else {
            confirm_btn.GetComponent<Image>().color = Color.black;
        }

        // panel on
        Shop_Seting_Intergrator.Instance.Confirm_Field_On();
    }

    private void Alert(string announce) {
        alert_panel.transform.Find("Border").Find("announce").GetComponent<TMP_Text>().text = announce;
        alert_panel.transform.DORotate(open, .25f);
    }
    
    public void Alert_End() {
        alert_panel.transform.DORotate(close, .25f);
    }


    public void Retry_Buy_Chest() {
        retry_btn.transform.DORotate(new(0, 90, 0), .25f);
        // 구매 확정되면 확률로 유닛을 뽑기
        var (count, percents) = chest_state[grade];

        // 유닛 뽑기
        for (int i = 0; i < count; i++) {
            int random_number = Random.Range(1, 1001);
            // d
            if (0 <= random_number && random_number < percents[0]) {
                unit_counter[Random.Range(D_start, D_end_1)].count++;
            }
            // c
            else if (percents[0] <= random_number && random_number < percents[1]) {
                unit_counter[Random.Range(C_start, C_end_1)].count++;
            }
            // b
            else if (percents[1] <= random_number && random_number < percents[2]) {
                unit_counter[Random.Range(B_start, B_end_1)].count++;
            }
            // a
            else if (percents[2] <= random_number && random_number < percents[3]) {
                unit_counter[Random.Range(A_start, A_end_1)].count++;
            }
            // s
            else if (percents[3] <= random_number && random_number < percents[4]) {
                unit_counter[Random.Range(S_start, S_end_1)].count++;
            }
            // ex
            else if (percents[4] <= random_number && random_number < percents[5]) {
                unit_counter[Random.Range(EX_start, EX_end_1)].count++;
            }
        }

        // result pannel setting
        // content clear
        foreach (Transform item in result_content.transform) {
            Destroy(item.gameObject);
        }

        var dic_keys = new List<int>(unit_counter.Keys);
        int keys_size = dic_keys.Count;
        string query;
        for (int i = 0; i < keys_size; i++) {
            if (unit_counter[dic_keys[i]].count <= 0) continue;

            var _unit = unit[dic_keys[i]];
            // prefab
            var unit_prefab = Instantiate(chest_prefab);
            unit_prefab.transform.Find("border").GetComponent<Image>().color = Utility.Get_Grade_Color(dic_keys[i]);
            unit_prefab.transform.Find("image").GetComponent<Image>().sprite = Utility.Get_sprite(dic_keys[i]);
            unit_prefab.transform.Find("image").GetComponent<Image>().color = Utility.Get_Type_Color(dic_keys[i]);
            unit_prefab.transform.Find("name").GetComponent<TMP_Text>().text = _unit.nick_name;
            unit_prefab.transform.Find("type").GetComponent<TMP_Text>().text = Utility.Get_Type_String(_unit.type);

            unit_prefab.transform.Find("count").GetComponent<TMP_Text>().text = "x" + unit_counter[dic_keys[i]].count.ToString();
            unit_prefab.transform.SetParent(result_content.transform, false);

            // modify db
            Utility.builder.Clear();
            _unit.piece += unit_counter[dic_keys[i]].count;
            query = Utility.builder.Append("UPDATE unit SET piece=")
                                   .Append(_unit.piece)
                                   .Append(" WHERE id=")
                                   .Append(_unit.id)
                                   .ToString();
            ModifyDB.Instance.ModifySet(query, "unit");
            Utility.builder.Clear();
        }
        result_panel.transform.Find("Slider_Border").gameObject.SetActive(true);

        Chest_Setting = true;

        // 업적 확인
        if (grade.Equals("r")) grade = "c";
        Achievement_Observer.Instance.Open_Chest(grade);
    }

    public void Buy_Chest_Confirm() {

        Shop_Seting_Intergrator.Instance.Loading_On(1);

        if (free_chest) {
            PlayerPrefs.SetString("Free_Unit_Chest_RemainTime", System.DateTime.Now.ToString());
            PlayerPrefs.Save();
        }
        else {
            user.Gold -= chest_value;
            Header_Setter.Instance.Set_Head();
            Achievement_Observer.Instance.Use_Gold_Achievement(chest_value);
        }
        
        Chest_Setting = false;

        // 구매 확정되면 확률로 유닛을 뽑기
        var (count, percents) = chest_state[grade];

        // 유닛 뽑기
        for(int i = 0; i < count; i++) {
            int random_number = Random.Range(1, 1001);
            // d
            if (0 <= random_number && random_number < percents[0]) {
                unit_counter[Random.Range(D_start, D_end_1)].count++;
            }
            // c
            else if (percents[0] <= random_number && random_number < percents[1]) {
                unit_counter[Random.Range(C_start, C_end_1)].count++;
            }
            // b
            else if (percents[1] <= random_number && random_number < percents[2]) {
                unit_counter[Random.Range(B_start, B_end_1)].count++;
            }
            // a
            else if (percents[2] <= random_number && random_number < percents[3]) {
                unit_counter[Random.Range(A_start, A_end_1)].count++;
            }
            // s
            else if (percents[3] <= random_number && random_number < percents[4]) {
                unit_counter[Random.Range(S_start, S_end_1)].count++;
            }
            // ex
            else if (percents[4] <= random_number && random_number < percents[5]) {
                unit_counter[Random.Range(EX_start, EX_end_1)].count++;
            }
        }

        // result pannel setting
        // content clear
        foreach (Transform item in result_content.transform) {
            Destroy(item.gameObject);
        }

        var dic_keys = new List<int>(unit_counter.Keys);
        int keys_size = dic_keys.Count;
        string query;
        for(int i = 0; i < keys_size; i++) {
            if (unit_counter[dic_keys[i]].count > 0) {
                var _unit = unit[dic_keys[i]];
                // prefab
                var unit_prefab = Instantiate(chest_prefab);
                unit_prefab.transform.Find("border").GetComponent<Image>().color = Utility.Get_Grade_Color(dic_keys[i]);
                unit_prefab.transform.Find("image").GetComponent<Image>().sprite = Utility.Get_sprite(dic_keys[i]);
                unit_prefab.transform.Find("image").GetComponent<Image>().color = Utility.Get_Type_Color(dic_keys[i]);
                unit_prefab.transform.Find("name").GetComponent<TMP_Text>().text = _unit.nick_name;
                unit_prefab.transform.Find("type").GetComponent<TMP_Text>().text = Utility.Get_Type_String(_unit.type); 
                unit_prefab.transform.Find("count").GetComponent<TMP_Text>().text = "x" + unit_counter[dic_keys[i]].count.ToString();
                unit_prefab.transform.SetParent(result_content.transform, false);

                // modify db
                Utility.builder.Clear();
                _unit.piece += unit_counter[dic_keys[i]].count;
                query = Utility.builder.Append("UPDATE unit SET piece=")
                                       .Append(_unit.piece)
                                       .Append(" WHERE id=")
                                       .Append(_unit.id)
                                       .ToString();
                ModifyDB.Instance.ModifySet(query, "unit");
                Utility.builder.Clear();
            }
        }
        result_panel.transform.Find("Slider_Border").gameObject.SetActive(true);

        Chest_Setting = true;

        // 업적 확인
        if (grade.Equals("r")) grade = "c";
        Achievement_Observer.Instance.Open_Chest(grade);
    }
}
