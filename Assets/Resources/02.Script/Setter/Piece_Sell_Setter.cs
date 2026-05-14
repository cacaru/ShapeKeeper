using CUSTOM_DATA;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Common_Data;

public class Piece_Sell_Setter : MonoBehaviour
{

    [SerializeField] private Transform sell_panel;
    [SerializeField] private GameObject all_unit_panel;

    [SerializeField] private GameObject result_panel;
    [SerializeField] private GameObject border;
    [SerializeField] private GameObject confirm_btn;

    private int count_max = 0;
    private int now_count = 0;

    private int price = 0; // now_count * grade_price
    private int grade_price = 0;
    private int unit_id = -1;

    private readonly Vector3 close = new(90, 0, 0);

    private readonly string DEFAULT_NAME = "판매할 조각";
    private readonly string DEFAULT_TYPE = "타입";

    // count 
    // price
    // Sell onClick

    public void Set_Sell_Unit_Open() {
        // unit을 선택할 수 있는 창을 보여줘야함
        all_unit_panel.transform.DORotate(open, .25f);
    }

    public void Set_Sell_Unit(int _unit_id) {
        unit_id = _unit_id;
        Setting_Icon();
        all_unit_panel.transform.DORotate(close, .25f);
    }

    private void Setting_Icon() {
        var _unit = unit[unit_id];
        // sell panel
        sell_panel.Find("piece").GetComponent<Image>().color = Utility.Get_Grade_Color(unit_id);
        sell_panel.Find("piece").Find("image").GetComponent<Image>().sprite = Utility.Get_sprite(unit_id);
        sell_panel.Find("piece").Find("image").GetComponent<Image>().color = Utility.Get_Type_Color(unit_id);
        sell_panel.Find("piece").Find("name").GetComponent<TMP_Text>().text = _unit.nick_name;
        sell_panel.Find("piece").Find("type").GetComponent<TMP_Text>().text = Utility.Get_Type_String(_unit.type);

        now_count = _unit.piece;
        count_max = now_count;
        grade_price = _unit.grade switch {
            "d" => 50,
            "c" => 100,
            "b" => 200,
            "a" => 400,
            "s" => 600,
            "ex" => 1000,
            _ => 0
        };
        price = now_count * grade_price;

        sell_panel.Find("price").GetComponent<TMP_Text>().text = price.ToString();
        sell_panel.Find("count").GetComponent<TMP_Text>().text = now_count.ToString();

        sell_panel.Find("Sell").GetComponent<Button>().onClick.RemoveAllListeners();
        sell_panel.Find("Sell").GetComponent<Button>().onClick.AddListener(() => {
            Sell_Confirm_Check_Move();
        });

        Set_Confirm_Panel();

        // all unit page down
        all_unit_panel.transform.DORotate(close, .25f);
    }

    private void Set_Confirm_Panel() {
        var _unit = unit[unit_id];
        var type_str = Utility.Get_Type_String(_unit.type); 

        // setting confirm panel
        border.transform.Find("buy_announce").GetComponent<TMP_Text>().text = _unit.nick_name + " - " + type_str;
        border.transform.Find("buy_announce_title").GetComponent<TMP_Text>().text = "판매 예정 내역";
        border.transform.Find("buy_item").GetComponent<Image>().sprite = Utility.Get_sprite(unit_id);
        border.transform.Find("buy_item").GetComponent<Image>().color = Utility.Get_Type_Color(unit_id);
        border.transform.Find("buy_item+border").GetComponent<Image>().color = Utility.Get_Grade_Color(unit_id);
        border.transform.Find("price").GetComponent<TMP_Text>().text = price.ToString();

        confirm_btn.GetComponent<Button>().onClick.RemoveAllListeners();
        confirm_btn.GetComponent<Button>().onClick.AddListener(() => {
            Sell_Confirm();
        });

        // setting result panel
        result_panel.transform.Find("Simple_Border").Find("Item_Obj").Find("image").GetComponent<Image>().sprite = Utility.Get_sprite(unit_id);
        result_panel.transform.Find("Simple_Border").Find("Item_Obj").Find("Item").GetComponent<Image>().color = Utility.Get_Grade_Color(unit_id);
        result_panel.transform.Find("Simple_Border").Find("Item_Obj").Find("image").GetComponent<Image>().color = Utility.Get_Type_Color(unit_id);
        result_panel.transform.Find("Simple_Border").Find("Item_Obj").Find("name").GetComponent<TMP_Text>().text = _unit.nick_name;
        result_panel.transform.Find("Simple_Border").Find("Item_Obj").Find("type").GetComponent<TMP_Text>().text = type_str;
        result_panel.transform.Find("Simple_Border").Find("piece_before").GetComponent<TMP_Text>().text = _unit.piece.ToString();
        result_panel.transform.Find("Simple_Border").Find("piece_after").GetComponent<TMP_Text>().text = (_unit.piece - now_count).ToString();
        result_panel.transform.Find("Simple_Border").gameObject.SetActive(true);
    }

    private void Sell_Confirm_Check_Move() {
        // panel on
        Shop_Seting_Intergrator.Instance.Confirm_Field_On();
    }

    private void Sell_Confirm() {
        // gold ++
        user.Gold += price;
        Header_Setter.Instance.Set_Head();

        Utility.builder.Clear();
        var query = Utility.builder.Append("UPDATE unit SET piece=")
                                   .Append(unit[unit_id].piece - now_count)
                                   .Append(" WHERE id=")
                                   .Append(unit_id)
                                   .ToString();
        Utility.builder.Clear();
        ModifyDB.Instance.ModifySet(query, "unit");

        // 업적 체크
        Achievement_Observer.Instance.Sell_Achievement(now_count);

        Sell_Panel_Init();
        Shop_Seting_Intergrator.Instance.Loading_On(0);
    }

    public void Sell_Panel_Init() {
        // sell panel
        sell_panel.Find("piece").GetComponent<Image>().color = Color.white;
        sell_panel.Find("piece").Find("image").GetComponent<Image>().sprite = sell_default_icon;
        sell_panel.Find("piece").Find("image").GetComponent<Image>().color = Color.white;
        sell_panel.Find("piece").Find("name").GetComponent<TMP_Text>().text = DEFAULT_NAME;
        sell_panel.Find("piece").Find("type").GetComponent<TMP_Text>().text = DEFAULT_TYPE;

        now_count = 0;
        count_max = 0;
        grade_price = 0;
        price = 0;

        sell_panel.Find("price").GetComponent<TMP_Text>().text = price.ToString();
        sell_panel.Find("count").GetComponent<TMP_Text>().text = now_count.ToString();

        sell_panel.Find("Sell").GetComponent<Button>().onClick.RemoveAllListeners();
    }

    public void Sell_piece_count_Controll(int count) {
        
        if (grade_price <= 0) return;

        now_count += count;
        if (now_count > count_max) {
            now_count = count_max;
        }
        else if (now_count < 0) { now_count = 0; }

        price = now_count * grade_price;

        sell_panel.Find("count").GetComponent<TMP_Text>().text = now_count.ToString();
        sell_panel.Find("price").GetComponent<TMP_Text>().text = price.ToString();

        Set_Confirm_Panel();
    }
}
