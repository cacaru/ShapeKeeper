using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Common_Data;
using CUSTOM_DATA;
using System.Collections.Generic;

public class Achievement_Recive_Setter : Scene_Singleton<Achievement_Recive_Setter>
{

    [SerializeField] GameObject field;
    [SerializeField] GameObject result_field;
    [SerializeField] GameObject result_field_piece;
    [SerializeField] GameObject result_field_piece_content;
    [SerializeField] GameObject result_field_normal;

    public bool loading_end = false;
    private Dictionary<int, Unit_Count_Checker> piece_result;
    public void Setting(int id, int type) {
        // type - 1 daily, 2 weekly, 3 achieve

        Mission quest = type switch {
            1 => daily_quest[id],
            2 => weekly_quest[id],
            3 => achievement[id],
            _ => null
        };

        if (quest == null) return;
        
        // title - name
        field.transform.Find("title").GetComponent<TMP_Text>().text = quest.name;
        // icon - can recive
        field.transform.Find("icon").GetComponent<Image>().sprite = quest.can_recive ? achieve_can_recive_icon : achieve_normal_icon;
        // reward - 보상
        int reward_size = quest.reward_list.Count;
        Utility.builder.Clear();
        for (int i = 0; i < reward_size; i++) {
            Utility.builder.Append(quest.reward_list[i])
                           .Append("+")
                           .Append(quest.reward_val[i])
                           .Append("\n");
        }

        field.transform.Find("reward").GetComponent<TMP_Text>().text = Utility.builder.ToString();
        Utility.builder.Clear();
        // counter - 현재 달성된 횟수
        field.transform.Find("counter").GetComponent<TMP_Text>().text = quest.counter.ToString();
        // checker - 완료에 필요한 횟수
        if(type == 3) {
            string str = "1";
            if (achievement[id].repeat) {
                str = (achievement[id].endless_value * (achievement[id].checker + 1)).ToString();

            }
            field.transform.Find("checker").GetComponent<TMP_Text>().text = str;
        }
        else {
            field.transform.Find("checker").GetComponent<TMP_Text>().text = quest.request_counter == 0 ? "1" : quest.request_counter.ToString();
        }
        
        // 받기 버튼 -> can recive 면 활성화
        // 활성화 시 add listener -> recive
        if (quest.can_recive) {
            field.transform.Find("Confirm").GetComponent<Button>().onClick.RemoveAllListeners();
            field.transform.Find("Confirm").GetComponent<Button>().onClick.AddListener(() => {
                Recive_Mission(id, type);
            });

            field.transform.Find("Confirm").gameObject.SetActive(true);
        }
        else {
            field.transform.Find("Confirm").gameObject.SetActive(false);
        }

        Achieve_Loading.Instance.Open_Recive_Check();
    }

    public void Recive_Mission(int id, int type) {
        loading_end = false;
        // 타입에 따라 받기
        piece_result = Utility.Mission_Recive(id, type);

        // 로딩 띄우기
        Achieve_Loading.Instance.Start_Load(id, type);
    }

    public void All_Recive_Mission(int type) {
        //Debug.Log(type + " click - all recive mission");
        loading_end = false;

        piece_result = Utility.All_Mission_Recive(type);

        // 로딩 띄우기
        Achieve_Loading.Instance.All_Recive_Load(type);
    }

    // single recive
    public void Setting_Result(int id, int type) {

        // content 내부 치우기
        foreach (Transform item in result_field_piece_content.transform) {
            if (item != result_field_piece_content.transform) {
                Destroy(item.gameObject);
            }
        }

        result_field_normal.SetActive(false);
        result_field_piece.SetActive(false);
        Mission quest = type switch {
            1 => daily_quest[id],
            2 => weekly_quest[id],
            3 => achievement[id],
            _ => null
        };

        // piece 보상인지 아닌지 확인
        bool normal = quest.reward_list[0].Equals("gold");

        if (normal) {
            // 일반 영역 채우기
            result_field_normal.transform.Find("gold_reward").GetComponent<TMP_Text>().text = "골드 + " + quest.reward_val[0];
            if(quest.reward_list.Count > 1) {
                result_field_normal.transform.Find("exp_reward").GetComponent<TMP_Text>().text = "경험치 + " + quest.reward_val[1];
            }
            result_field_normal.SetActive(true);
        }
        else {
            // 보상 만큼 채우기
            foreach (var item in piece_result) {
                //Debug.Log(item.Value.count);
                if (item.Value.count > 0) {
                    var instance = Instantiate(chest_prefab, result_field_piece_content.transform);
                    instance.transform.SetParent(result_field_piece_content.transform , false);

                    instance.transform.Find("image").GetComponent<Image>().sprite = Utility.Get_sprite(item.Key);
                    instance.transform.Find("image").GetComponent<Image>().color = Utility.Get_Type_Color(item.Key);
                    instance.transform.Find("border").GetComponent<Image>().color = Utility.Get_Grade_Color(item.Key);

                    instance.transform.Find("name").GetComponent<TMP_Text>().text = unit[item.Key].nick_name;
                    instance.transform.Find("type").GetComponent<TMP_Text>().text = Utility.Get_Type_String(unit[item.Key].type);

                    instance.transform.Find("count").GetComponent<TMP_Text>().text = "x" + item.Value.count;
                }
            }

            result_field_piece.SetActive(true);
        }

        loading_end = true;
    }

    // all recive
    public void Setting_Result() {
        // content 내부 치우기
        foreach (Transform item in result_field_piece_content.transform) {
            if (item != result_field_piece_content.transform) {
                Destroy(item.gameObject);
            }
        }
        result_field_normal.SetActive(false);
        result_field_piece.SetActive(false);

        // piece_result[0] == gold
        // piece_result[1] == normal
        // => 0과 1에 값이 있으면 normal도 소환
        // 그외 다른 유닛들의 count가 0이상이면 piece_field 도 소환
        // 보상 만큼 채우기
        bool has_piece_result = false;
        foreach (var item in piece_result) {
            if (item.Value.count > 0) {
                if (item.Key == 0 && item.Value.count > 0) {
                    // 일반 영역 채우기
                    result_field_normal.transform.Find("gold_reward").GetComponent<TMP_Text>().text = "골드 + " + piece_result[0].count;
                    result_field_normal.SetActive(true);
                }
                else if (item.Key == 1 && item.Value.count > 1) {
                    result_field_normal.transform.Find("exp_reward").GetComponent<TMP_Text>().text = "경험치 + " + piece_result[1].count;
                }
                else {
                    has_piece_result = true;
                    var instance = Instantiate(chest_prefab, result_field_piece_content.transform);
                    instance.transform.SetParent(result_field_piece_content.transform, false);

                    instance.transform.Find("image").GetComponent<Image>().sprite = Utility.Get_sprite(item.Key);
                    instance.transform.Find("image").GetComponent<Image>().color = Utility.Get_Type_Color(item.Key);
                    instance.transform.Find("border").GetComponent<Image>().color = Utility.Get_Grade_Color(item.Key);

                    instance.transform.Find("name").GetComponent<TMP_Text>().text = unit[item.Key].nick_name;
                    instance.transform.Find("type").GetComponent<TMP_Text>().text = Utility.Get_Type_String(unit[item.Key].type);

                    instance.transform.Find("count").GetComponent<TMP_Text>().text = "x" + item.Value.count;
                }
                
            }
        }

        if(has_piece_result)
            result_field_piece.SetActive(true);

        loading_end = true;


    }
}
