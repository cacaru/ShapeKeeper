using CUSTOM_DATA;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Game_State_Data;
using static CUSTOM_DATA.Game_Value_Data;

public class Game_End : Scene_Singleton<Game_End>
{

    [SerializeField] private GameObject victory;
    [SerializeField] private GameObject victory_content;
    [SerializeField] private GameObject fail;
    [SerializeField] private GameObject End_Pannel;
    private bool ending = false;

    public bool Is_End { get { return ending; } }
    // game end
    public void Ending(bool _victory) {
        is_gaming = false;
        Data_Init();
        Spawner.Instance.End_Spawn();

        ending = true;

        // game end 
        // 골드, 조각, 경험치
        int gold;
        int exp;
        int piece;
        Init_Counter();

        if (_victory) {
            // 승리시 보상
            // 골드 100~500 + 난이도 * (40~60)
            // 조각 1 ~ 5 + 난이도 * 10
            // 경험치 5 ~ 10 + 난이도 * 5
            int default_gold = Random.Range(100, 501) + (difficulty) * Random.Range(40, 61);
            int skill_gain_gold_rate = user.GetSkillValue(SkillType.ClearGold);
            gold = skill_gain_gold_rate > 0 ? default_gold + default_gold * skill_gain_gold_rate / 100 : default_gold;
            exp = Random.Range(5, 11) + difficulty * Random.Range(5, 11);
            piece = Random.Range(1, 6) + difficulty * 10;
            // 조각 설정 ( 랜덤하게 반복하면서 갯수 를 랜덤 증정 최대조각 수 채워지면 끝
            while (piece > 0) {
                int front = Random.Range(2, 8) * 1000;
                int unit_count = front switch {
                    2000 => Random.Range(1, 10),
                    3000 => Random.Range(1, 12),
                    4000 => Random.Range(1, 12),
                    5000 => Random.Range(1, 12),
                    6000 => Random.Range(1, 12),
                    7000 => Random.Range(1, 6),
                    _ => Random.Range(1, 6)
                };
                int tmp_piece = Random.Range(1, piece+1);
                piece -= tmp_piece;
                unit_counter[front + unit_count].count += tmp_piece;
            }

            // 승리 오브젝트 설정
            // 유닛을 돌면서 counter가 1이상인 것을 unit_result로 표현해줘야함
            foreach (var item in unit_counter) {
                if (item.Value.count > 0) {
                    var instance = Instantiate(unit_result_prefab);
                    instance.transform.Find("Image").GetComponent<Image>().sprite = Utility.Get_sprite(item.Key);
                    instance.transform.Find("Type").GetComponent<TMP_Text>().text = Utility.Get_Type_String(unit[item.Key].type);
                    instance.transform.Find("Type").GetComponent<TMP_Text>().color = Utility.Get_Type_Color(item.Key);
                    instance.transform.Find("Outter").GetComponent<Image>().color = Utility.Get_Grade_Color(item.Key);
                    instance.transform.Find("Piece").GetComponent<TMP_Text>().text = unit_counter[item.Key].count.ToString();
                    instance.transform.SetParent(victory_content.transform, false);
                    instance.transform.localScale = Vector3.one;
                }
            }
            // 업적 갱신
            // 난이도 클리어 확인
            Achievement_Observer.Instance.Clear_Achievement();
            victory.transform.Find("gold").GetComponent<TMP_Text>().text = gold.ToString();
            victory.transform.Find("exp").GetComponent<TMP_Text>().text = exp.ToString();

            fail.SetActive(false);
            victory.SetActive(true);
        }
        // 실패시 보상
        else {
            // 골드 1 ~ 100 + 난이도 * (10~30)
            // 경험치 1 ~ 5 + 난이도 * 3
            int default_gold = Random.Range(1, 101) + (difficulty) * Random.Range(10, 31);
            int skill_gain_gold_rate = user.GetSkillValue(SkillType.ClearGold);
            gold = skill_gain_gold_rate > 0 ? default_gold + default_gold * skill_gain_gold_rate / 100 : default_gold;
            exp = Random.Range(1, 5) + difficulty * 3;

            //user 추가

            fail.transform.Find("gold").GetComponent<TMP_Text>().text = gold.ToString();
            fail.transform.Find("exp").GetComponent<TMP_Text>().text = exp.ToString();

            victory.SetActive(false);
            fail.SetActive(true);
        }

        Init_Counter();

        user.Gold = user.gold + gold;
        user.Experience += exp;

        End_Pannel.SetActive(true);
    }
}
