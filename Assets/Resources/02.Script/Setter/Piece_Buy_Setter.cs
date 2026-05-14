using UnityEngine;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Common_Data;
using UnityEngine.UI;
using CUSTOM_DATA;
using TMPro;
using System.Collections;



public class Piece_Buy_Setter : Scene_Singleton<Piece_Buy_Setter>
{
    // 시간을 확인하고
    // 1시간 이 지나있으면 리롤 아니면 기존 그대로 출력
    // 저장한건 프리팹에 넣어두자 ( string 형식 )

    [SerializeField] private GameObject chest_area;

    [SerializeField] private GameObject result_panel;
    [SerializeField] private GameObject border;
    [SerializeField] private GameObject confirm_btn;

    private int[] temp = { -1, -1, -1, -1, -1 };
    private int need_price = 0;
    public void Card_Setting() {
        // 총 5개의 조각 
        // 5개로 쪼개서 저장

        System.DateTime now = System.DateTime.Now;
        System.DateTime end_time = System.Convert.ToDateTime(PlayerPrefs.GetString("Piece_Sell_Start_Time") == "" ? now.ToString() : PlayerPrefs.GetString("Piece_Sell_Start_Time"));
        System.TimeSpan dif = now - end_time;
        int min_dif = (int)dif.TotalMinutes;

        // 1 시간 지난거 확인

        if (min_dif > 0 && min_dif < 60) {
            // 안지났으므로 기존것
            //Debug.Log("normal");
            temp[0] = PlayerPrefs.GetInt("Piece_Sell_1");
            temp[1] = PlayerPrefs.GetInt("Piece_Sell_2");
            temp[2] = PlayerPrefs.GetInt("Piece_Sell_3");
            temp[3] = PlayerPrefs.GetInt("Piece_Sell_4");
            temp[4] = PlayerPrefs.GetInt("Piece_Sell_5");
        }
        else {
            // 지났으므로 새로 뽑기
            //Debug.Log("create new");
            int[] percents = { 400, 700, 850, 950, 990, 1000 };
            // 유닛 뽑기
            for (int i = 0; i < 5; i++) {
                int random_number = Random.Range(1, 1001);

                // d
                if (0 <= random_number && random_number < percents[0]) {
                    temp[i] = Random.Range(D_start, D_end_1);
                }
                // c
                else if (percents[0] <= random_number && random_number < percents[1]) {
                    temp[i] = Random.Range(C_start, C_end_1); 
                }
                // b
                else if (percents[1] <= random_number && random_number < percents[2]) {
                    temp[i] = Random.Range(B_start, B_end_1);
                }
                // a
                else if (percents[2] <= random_number && random_number < percents[3]) {
                    temp[i] = Random.Range(A_start, A_end_1);
                }
                // s
                else if (percents[3] <= random_number && random_number < percents[4]) {
                    temp[i] = Random.Range(S_start, S_end_1);
                }
                // ex
                else if (percents[4] <= random_number && random_number < percents[5]) {
                    temp[i] = Random.Range(EX_start, EX_end_1);
                }
            }

            // 새로 뽑은 숫자 저장
            PlayerPrefs.SetInt("Piece_Sell_1", temp[0]);
            PlayerPrefs.SetInt("Piece_Sell_2", temp[1]);
            PlayerPrefs.SetInt("Piece_Sell_3", temp[2]);
            PlayerPrefs.SetInt("Piece_Sell_4", temp[3]);
            PlayerPrefs.SetInt("Piece_Sell_5", temp[4]);

            // 새로 뽑은 시간 저장
            PlayerPrefs.SetString("Piece_Sell_Start_Time", System.DateTime.Now.ToString());
        }
        //Debug.Log(temp[0] + "," + temp[1] + "," + temp[2] + "," + temp[3] + "," + temp[4]);

        StartCoroutine(Check_Card());
    }
    IEnumerator Check_Card() {
        while (temp[4] == -1) {
            yield return null;
        }
        Setting_Card_image();
    }

    private void Setting_Card_image() {

        // temp에 저장된 값을 통해 piece 설정하기
        // btn setting
        chest_area.transform.Find("piece_1").GetComponent<Button>().onClick.RemoveAllListeners();
        chest_area.transform.Find("piece_2").GetComponent<Button>().onClick.RemoveAllListeners();
        chest_area.transform.Find("piece_3").GetComponent<Button>().onClick.RemoveAllListeners();
        chest_area.transform.Find("piece_4").GetComponent<Button>().onClick.RemoveAllListeners();
        chest_area.transform.Find("piece_5").GetComponent<Button>().onClick.RemoveAllListeners();

        chest_area.transform.Find("piece_1").GetComponent<Button>().onClick.AddListener(() => Buy_Confirm_Setting(temp[0]));
        chest_area.transform.Find("piece_2").GetComponent<Button>().onClick.AddListener(() => Buy_Confirm_Setting(temp[1]));
        chest_area.transform.Find("piece_3").GetComponent<Button>().onClick.AddListener(() => Buy_Confirm_Setting(temp[2]));
        chest_area.transform.Find("piece_4").GetComponent<Button>().onClick.AddListener(() => Buy_Confirm_Setting(temp[3]));
        chest_area.transform.Find("piece_5").GetComponent<Button>().onClick.AddListener(() => Buy_Confirm_Setting(temp[4]));

        // border
        chest_area.transform.Find("piece_1").GetComponent<Image>().color = Utility.Get_Grade_Color(temp[0]);
        chest_area.transform.Find("piece_2").GetComponent<Image>().color = Utility.Get_Grade_Color(temp[1]);
        chest_area.transform.Find("piece_3").GetComponent<Image>().color = Utility.Get_Grade_Color(temp[2]);
        chest_area.transform.Find("piece_4").GetComponent<Image>().color = Utility.Get_Grade_Color(temp[3]);
        chest_area.transform.Find("piece_5").GetComponent<Image>().color = Utility.Get_Grade_Color(temp[4]);

        // image
        chest_area.transform.Find("piece_1").Find("image").GetComponent<Image>().sprite = Utility.Get_sprite(temp[0]);
        chest_area.transform.Find("piece_2").Find("image").GetComponent<Image>().sprite = Utility.Get_sprite(temp[1]);
        chest_area.transform.Find("piece_3").Find("image").GetComponent<Image>().sprite = Utility.Get_sprite(temp[2]);
        chest_area.transform.Find("piece_4").Find("image").GetComponent<Image>().sprite = Utility.Get_sprite(temp[3]);
        chest_area.transform.Find("piece_5").Find("image").GetComponent<Image>().sprite = Utility.Get_sprite(temp[4]);

        // image type color
        chest_area.transform.Find("piece_1").Find("image").GetComponent<Image>().color = Utility.Get_Type_Color(temp[0]);
        chest_area.transform.Find("piece_2").Find("image").GetComponent<Image>().color = Utility.Get_Type_Color(temp[1]);
        chest_area.transform.Find("piece_3").Find("image").GetComponent<Image>().color = Utility.Get_Type_Color(temp[2]);
        chest_area.transform.Find("piece_4").Find("image").GetComponent<Image>().color = Utility.Get_Type_Color(temp[3]);
        chest_area.transform.Find("piece_5").Find("image").GetComponent<Image>().color = Utility.Get_Type_Color(temp[4]);

        // name
        chest_area.transform.Find("piece_1").Find("name").GetComponent<TMP_Text>().text = unit[temp[0]].nick_name;
        chest_area.transform.Find("piece_2").Find("name").GetComponent<TMP_Text>().text = unit[temp[1]].nick_name;
        chest_area.transform.Find("piece_3").Find("name").GetComponent<TMP_Text>().text = unit[temp[2]].nick_name;
        chest_area.transform.Find("piece_4").Find("name").GetComponent<TMP_Text>().text = unit[temp[3]].nick_name;
        chest_area.transform.Find("piece_5").Find("name").GetComponent<TMP_Text>().text = unit[temp[4]].nick_name;
        // type
        chest_area.transform.Find("piece_1").Find("type").GetComponent<TMP_Text>().text = Utility.Get_Type_String(unit[temp[0]].type);
        chest_area.transform.Find("piece_2").Find("type").GetComponent<TMP_Text>().text = Utility.Get_Type_String(unit[temp[1]].type);
        chest_area.transform.Find("piece_3").Find("type").GetComponent<TMP_Text>().text = Utility.Get_Type_String(unit[temp[2]].type);
        chest_area.transform.Find("piece_4").Find("type").GetComponent<TMP_Text>().text = Utility.Get_Type_String(unit[temp[3]].type);
        chest_area.transform.Find("piece_5").Find("type").GetComponent<TMP_Text>().text = Utility.Get_Type_String(unit[temp[4]].type);
    }

    public void Buy_Confirm_Setting(int unit_id) {
        var _unit = unit[unit_id];
        var type_str = Utility.Get_Type_String(_unit.type);
        need_price = _unit.grade switch {
            "d" => 100,
            "c" => 200,
            "b" => 400,
            "a" => 800,
            "s" => 1200,
            "ex" => 2000,
            _ => 0
        };

        // setting confirm panel
        border.transform.Find("buy_announce").GetComponent<TMP_Text>().text = _unit.nick_name + " - " + type_str;
        border.transform.Find("buy_item").GetComponent<Image>().sprite = Utility.Get_sprite(unit_id);
        border.transform.Find("buy_item").GetComponent<Image>().color = Utility.Get_Type_Color(unit_id);
        border.transform.Find("buy_item+border").GetComponent<Image>().color = Utility.Get_Grade_Color(unit_id);
        border.transform.Find("price").GetComponent<TMP_Text>().text = need_price.ToString();
        border.transform.Find("price").GetComponent<TMP_Text>().color = need_price <= user.Gold ? Color.green : Color.red;

        confirm_btn.GetComponent<Button>().onClick.RemoveAllListeners();
        confirm_btn.GetComponent<Button>().onClick.AddListener(() => {
            Buy_Piece_Confirm(unit_id);
        });

        // setting result panel
        result_panel.transform.Find("Simple_Border").Find("Item_Obj").Find("image").GetComponent<Image>().sprite = Utility.Get_sprite(unit_id);
        result_panel.transform.Find("Simple_Border").Find("Item_Obj").Find("Item").GetComponent<Image>().color = Utility.Get_Grade_Color(unit_id);
        result_panel.transform.Find("Simple_Border").Find("Item_Obj").Find("image").GetComponent<Image>().color = Utility.Get_Type_Color(unit_id);
        result_panel.transform.Find("Simple_Border").Find("Item_Obj").Find("name").GetComponent<TMP_Text>().text = _unit.nick_name;
        result_panel.transform.Find("Simple_Border").Find("Item_Obj").Find("type").GetComponent<TMP_Text>().text = type_str;
        result_panel.transform.Find("Simple_Border").Find("piece_before").GetComponent<TMP_Text>().text = _unit.piece.ToString();
        result_panel.transform.Find("Simple_Border").Find("piece_after").GetComponent<TMP_Text>().text = (_unit.piece + 1).ToString();
        result_panel.transform.Find("Simple_Border").gameObject.SetActive(true);

        // panel on
        Shop_Seting_Intergrator.Instance.Confirm_Field_On();
    }

    private void Buy_Piece_Confirm(int unit_id) {
        if (need_price > user.Gold) return;

        user.Gold -= need_price;
        Header_Setter.Instance.Set_Head();
        // 골드 소모 업적
        Achievement_Observer.Instance.Use_Gold_Achievement(need_price);

        // piece ++
        Utility.builder.Clear();
        var query = Utility.builder.Append("UPDATE unit SET piece=")
                                   .Append(unit[unit_id].piece + 1)
                                   .Append(" WHERE id=")
                                   .Append(unit_id)
                                   .ToString();
        Utility.builder.Clear();
        ModifyDB.Instance.ModifySet(query, "unit");

        Shop_Seting_Intergrator.Instance.Loading_On(0);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Card_Setting();
    }
}
