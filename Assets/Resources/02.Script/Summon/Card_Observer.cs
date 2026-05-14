using UnityEngine;
using UnityEngine.UI;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Common_Data;
using CUSTOM_DATA;
using TMPro;
using System.Collections.Generic;

/// <summary>
///  unit_counter가 추가될때마다 unit 카드 오브젝트를 추가해주는 스크립트
/// </summary>


public class Card_Observer : Scene_Singleton<Card_Observer>
{
    public GameObject content;

    //private readonly string material_txt = "일반";
    //private readonly string ethereal_txt = "특수";
    //private readonly string hybrid_txt = "하이브리드";

    private readonly string ONE = "1";

    // card관리 
    // install 는 손패에 있는 친구들
    // field 는 필드로 소환한 친구들

    // 소환하면 install -> field
    // 회수하면 field -> install

    private class Install_Card {
        public int id = 0;
        public int count = 0;
        public GameObject unit;
        public Install_Card(int id, int count, GameObject unit) {
            this.id = id;
            this.count = count;
            this.unit = unit;
        }
        public void Unlock_unit() { unit = null; }
    }

    private readonly Dictionary<int, Install_Card> card_install = new();
    private readonly Dictionary<int, int> card_field = new();

    public void New_Card(int unit_id) {
        // unit_id가 dic key에 있는지 검사 => 있으면 해당 오브젝트의 counter 항목을 증가시켜줌
        // 없으면 새로 생성
        if (card_install.ContainsKey(unit_id)) {
            card_install[unit_id].count++;
            card_install[unit_id].unit.transform.Find("Piece").GetComponent<TMP_Text>().text = card_install[unit_id].count.ToString();
        }
        else {
            card_install.Add(unit_id, Create_Unit_Card(unit_id));
        }
    }

    private Install_Card Create_Unit_Card(int unit_id) {
        // 현재 unit_id를 가진 card가 이미 있다면 -> unit_card에 쓰인 counter를 증가시켜줌
        var card = Unit_Card_Pool.Instance.Get_Card();
        card.transform.SetParent(content.transform, false);

        card.GetComponent<Card_To_Summon>().Unit_Id = unit_id;
        card.transform.Find("Outter").GetComponent<Image>().color = Utility.Get_Grade_Color(unit_id);
        card.transform.Find("Image").GetComponent<Image>().sprite = Utility.Get_sprite(unit_id);
        card.transform.Find("Image").GetComponent<Image>().color = Utility.Get_Type_Color(unit_id);
        /*
        card.transform.Find("Type").GetComponent<TMP_Text>().text = unit[unit_id].type.Split("_")[1] switch {
            "1" => material_txt,
            "2" => hybrid_txt,
            "3" => ethereal_txt,
            _ => "불명"
        };
        card.transform.Find("Type").GetComponent<TMP_Text>().color = Utility.Get_Type_Color(unit_id);
        */
        card.transform.Find("Field_Img").GetComponent<Image>().sprite = Utility.Get_Field_Type(unit_id);
        card.transform.Find("Piece").GetComponent<TMP_Text>().text = ONE;

        return new(unit_id, 1, card);
    }

    public void Delete_Unit_Card_in_Install(int unit_id) {
        unit_counter[unit_id].count--;

        // e 등급은 text를 변경해주면 끝남
        if (unit_id < 2000) { 
            Summon.Instance.E_Unit_Display();
            return;
        }
        card_install[unit_id].count--;
        if (card_install[unit_id].count <= 0) {
            // card delete
            GameObject card = card_install[unit_id].unit;
            card_install[unit_id].Unlock_unit();
            Unit_Card_Pool.Instance.Return_Card(card);
            card_install.Remove(unit_id);
        }
        else {
            card_install[unit_id].unit.transform.Find("Piece").GetComponent<TMP_Text>().text = card_install[unit_id].count.ToString();
        }
    }

    public void Card_Hider(int unit_id, bool show) {
        
        // field -> install (화면(필드)에서 손패로)
        if (show) {
            // field에 나가있는 카드 회수
            if (card_field.ContainsKey(unit_id)) {
                card_field[unit_id]--;
            }
            
            // unit_id 카드가 있으면 ++
            if (card_install.ContainsKey(unit_id)) {                
                card_install[unit_id].count++;
                card_install[unit_id].unit.transform.Find("Piece").GetComponent<TMP_Text>().text = card_install[unit_id].count.ToString();
            }
            // 없으면 생성
            else {
                card_install.Add(unit_id, Create_Unit_Card(unit_id));
            }

        }

        // install > field (손패 -> 화면)
        else {

            // install의 카운터를 확인하고 카운터가 0이되면 컷
            card_install[unit_id].count -= 1;

            if (card_install[unit_id].count >= 1) {
                card_install[unit_id].unit.transform.Find("Piece").GetComponent<TMP_Text>().text = card_install[unit_id].count.ToString();
            }
            else {
                Unit_Card_Pool.Instance.Return_Card(card_install[unit_id].unit);
                card_install[unit_id].Unlock_unit();
                card_install.Remove(unit_id);
            }

            // field에 나가있는 카드 목록에 추가
            if (card_field.ContainsKey(unit_id)) {
                card_field[unit_id]++;
            }
            else {
                card_field.Add(unit_id, 1);
            }
        }
    }

    public bool Card_Check_in_Install(int unit_id, int counter) {
        // unit_id가 손패에 있는지 확인
        bool result = false;
        if(card_install.ContainsKey(unit_id)) {
            if (card_install[unit_id].count >= counter) {
                result = true;
            }
        }
        return result;
    }

    public void Field_Card_Combine_Checker(int unit_id) {
        if (card_field.ContainsKey(unit_id)) card_field[unit_id]++;
        else card_field.Add(unit_id, 1);
    }
}
