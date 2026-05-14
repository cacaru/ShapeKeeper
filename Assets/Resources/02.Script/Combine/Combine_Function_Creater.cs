using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CUSTOM_DATA.Game_Data;

namespace CUSTOM_DATA {
    public class Combine_Function_Creater : MonoBehaviour {

        private readonly static string Combine_from_this_Title = "만들 수 있는 조합";
        private readonly static string Combine_to_this_Title = "만드는 방법";
        private readonly static string Combine_Default_Title = "소환을 통해 랜덤으로 획득할 수 있습니다.";
        private readonly static string Combine_Item_Title = "보스를 잡아 획득 할 수 있습니다.";

        private readonly static string[] card_title_arr = new string[3];
        private readonly static Dictionary<string, int> key_checker = new();

        private readonly static Dictionary<int, int> duple = new();
        private readonly static List<int> keys = new(unit.Keys);

        static readonly bool[] combine_checker = new bool[6] { false, false, false, false, false, false };
        static readonly int[] combine_ids = new int[6] { -1, -1, -1, -1, -1, -1 };
        static int combine_material;

        private static int unit_id = 0;
        private static GameObject combine_content = null;
        private static bool is_field = false;
        private static int type = 0;

        public static void Set_Combine_Content(GameObject _content) {
            combine_content = _content;
        }

        private static void Clear_Key_Checker() {
            key_checker.Clear();
        }

        private static void Clear_Duple() {
            int size = keys.Count;
            for(int i = 0; i < size; i++) {
                duple[keys[i]] = 0;
            }
        }

        private static void Clear_Combine_id_Checker() {
            // combine_checker 초기화
            for (int c = 0; c < 6; c++) combine_checker[c] = false;
            // combine_ids 초기화
            for (int c = 0; c < 6; c++) combine_ids[c] = -1;
        }
        private static void Get_Card_Title(int num) {
            switch (num) {
                case 0:
                    card_title_arr[0] = "Self";
                    card_title_arr[1] = "SelfBorder";
                    card_title_arr[2] = "SelfBack";
                    break;
                case 1:
                    card_title_arr[0] = "Material_1";
                    card_title_arr[1] = "MaterialBorder_1";
                    card_title_arr[2] = "MaterialBack_1";
                    break;
                case 2:
                    card_title_arr[0] = "Material_2";
                    card_title_arr[1] = "MaterialBorder_2";
                    card_title_arr[2] = "MaterialBack_2";
                    break;
                case 3:
                    card_title_arr[0] = "Material_3";
                    card_title_arr[1] = "MaterialBorder_3";
                    card_title_arr[2] = "MaterialBack_3";
                    break;
                case 4:
                    card_title_arr[0] = "Material_4";
                    card_title_arr[1] = "MaterialBorder_4";
                    card_title_arr[2] = "MaterialBack_4";
                    break;
                case 5:
                    card_title_arr[0] = "Result";
                    card_title_arr[1] = "ResultBorder";
                    card_title_arr[2] = "ResultBack";
                    break;
            }
        }

        public static void Clean_Combine_Table() {
            var list = combine_content.GetComponentsInChildren<Transform>();
            foreach (var item in list) {
                if (item != combine_content.transform && item.CompareTag("CombineTable")) {
                    Combine_Pool.Instance.Return_Function(item.gameObject);
                }
            }
        }

        // 가지고 있는지 확인
        private static void Check_Has(Unit uni, int function_id) {
            int _unit_id = uni.id;
            
            for(int i = 0; i < 4; i++) {
                if (combine_ids[i] > 0) {
                    if (unit_counter[combine_ids[i]].count > duple[combine_ids[i]]) {
                        duple[combine_ids[i]]++;
                        combine_checker[i] = true;
                    }
                }
                else {
                    combine_checker[i] = true;
                }
            }

            // 4는 조각과 크리스탈이므로 따로 검사
            if (combine_ids[4] > 0) {
                if (unit[_unit_id].combine_function[function_id].piece > 0) {
                    combine_checker[4] = piece_count >= unit[_unit_id].combine_function[function_id].piece;
                }
                else if (unit[_unit_id].combine_function[function_id].crystal > 0) {
                    combine_checker[4] = piece_count >= unit[_unit_id].combine_function[function_id].crystal;
                }
            }
            else {
                combine_checker[4] = true;
            }

            combine_checker[5] = combine_checker[0] && combine_checker[1] && combine_checker[2] && combine_checker[3] && combine_checker[4];
        }

        // combine function 설정
        private static void Setting_Combine_Function(Unit unit, int i) {
            Clear_Duple();

            if (key_checker.ContainsKey(unit.combine_function[i].c_function_string)) {
                return;
            }
            key_checker.Add(unit.combine_function[i].c_function_string, 1);

            combine_ids[0] = unit.id;
            combine_ids[1] = unit.combine_function[i].a;
            combine_ids[2] = unit.combine_function[i].b;
            combine_ids[3] = unit.combine_function[i].c;
            combine_ids[4] = unit.combine_function[i].piece > 0 ? 101 : (unit.combine_function[i].crystal > 0 ? 102 : -1);
            combine_ids[5] = unit.combine_function[i].result;

            //Debug.Log(combine_ids[0] + " + " + combine_ids[1] + " + " + combine_ids[2] + " + " + combine_ids[3] + " + " + combine_ids[4] + " == " + combine_ids[5]);

            // 보여줄 object 만들기
            GameObject _instance = Combine_Pool.Instance.Get_Function(2);
            _instance.transform.SetParent(combine_content.transform, false);
            _instance.transform.localScale = Vector3.one;

            Check_Has(unit, i);

            for (int target = 0; target <= 5; target++) {
                Get_Card_Title(target);
                // 유닛 번호는 어떻게 순서대로 가져오지
                // 순서대로 가져올 수 있게 위에서 넣어두면 되지
                // 0 1 에서 nesting check

                // material_1 확인 - 돌 일 수도 있음
                if (combine_ids[target] > 0) combine_material = combine_ids[target];
                else if (combine_ids[target] < 0) combine_material = -1;
                else if (target == 1) combine_material = unit.combine_function[i].crystal > 0 ? 102 : -1;
                else combine_material = unit.combine_function[i].piece > 0 ? 101 : -1;

                if (combine_material > 0) {
                    _instance.transform.Find(card_title_arr[0]).gameObject.SetActive(true);
                    _instance.transform.Find(card_title_arr[1]).gameObject.SetActive(true);
                    if (target > 0 && target < 5) {
                        _instance.transform.Find("plus_" + target).gameObject.SetActive(true);
                    }

                    _instance.transform.Find(card_title_arr[0]).GetComponent<Image>().sprite = Utility.Get_sprite(combine_material);
                    _instance.transform.Find(card_title_arr[0]).GetComponent<Image>().color = Utility.Get_Type_Color(combine_material);
                    _instance.transform.Find(card_title_arr[1]).GetComponent<Image>().color = Utility.Get_Grade_Color(combine_material);

                    // border의 board에 속성 설정하기
                    _instance.transform.Find(card_title_arr[1]).GetComponent<Combine_Board>().Id = combine_material;
                    _instance.transform.Find(card_title_arr[1]).GetComponent<Combine_Board>().function_id = unit.combine_function[i].id;
                    _instance.transform.Find(card_title_arr[1]).GetComponent<Combine_Board>().is_material = target != 5;
                    _instance.transform.Find(card_title_arr[1]).GetComponent<Combine_Board>().is_field = is_field;

                    // result의 공격력을 계산해 조합식에 표시하기
                    float material_attack = Game_Data.unit[unit.combine_function[i].result].material > 0 ? (Game_Data.unit[unit.combine_function[i].result].attack * Game_Data.unit[unit.combine_function[i].result].material / 100) : 0;
                    float ethereal_attack = Game_Data.unit[unit.combine_function[i].result].ethereal > 0 ? (Game_Data.unit[unit.combine_function[i].result].attack * Game_Data.unit[unit.combine_function[i].result].ethereal / 100) : 0;
                    _instance.transform.Find("material").GetComponent<TMP_Text>().text = material_attack.ToString();
                    _instance.transform.Find("ethereal").GetComponent<TMP_Text>().text = ethereal_attack.ToString();

                    // piece or stone이면 갯수 표기를 해줘야함
                    if (combine_material == 101) {
                        _instance.transform.Find("piece_" + target).gameObject.SetActive(true);
                        _instance.transform.Find("piece_" + target).GetComponent<TMP_Text>().text = unit.combine_function[i].piece.ToString();
                    }
                    if (combine_material == 102) {
                        _instance.transform.Find("piece_" + target).gameObject.SetActive(true);
                        _instance.transform.Find("piece_" + target).GetComponent<TMP_Text>().text = unit.combine_function[i].crystal.ToString();
                    }

                    if (Utility.Is_Special(combine_material)) {
                        _instance.transform.Find(card_title_arr[0]).GetComponent<Combine_Card_Color_Changer>().Set_Changing(true);
                    }

                    // background 
                    // 기본은 투명
                    _instance.transform.Find(card_title_arr[2]).GetComponent<Image>().color = Common_Data.background_off_color;

                    if (combine_checker[target]) {
                        _instance.transform.Find(card_title_arr[2]).GetComponent<Image>().color = Utility.Get_Grade_Color(combine_material);
                        _instance.transform.Find(card_title_arr[2]).gameObject.SetActive(true);
                    }
                }
                else
                    combine_checker[target] = true;
            }

            Clear_Combine_id_Checker();
        }


        //  ( field에서 클릭된 유닛에서부터 시작된건지 아닌지도 받음
        /// <summary>
        ///조합식을 설치할 때 type에 맞는 조합식을 설치할 함수 
        /// </summary>
        /// <param name="_unit_id">주요 유닛</param>
        /// <param name="_is_field">게임 필드에서 눌린지 여부</param>
        /// <param name="type">
        ///  1 > id로 만들 수 있는 조합식만 \n
        ///  2 > id를 만들 수 있는 조합식만
        ///  3 > id를 만들 수 있는 조합식 + id로 만들 수 있는 조합식 (순서대로)
        /// </param>
        public static void Show_Combine_Table(int _unit_id, bool _is_field, int _type) {
            unit_id = _unit_id;
            is_field = _is_field;
            type = _type;
            
            switch (type) {
                case 1:
                    Show_Combine_from_this_Function();
                    break;
                case 2:
                    Show_Combine_to_this_Function();
                    break;
                case 3:
                    Show_Combine_to_this_Function();
                    Show_Combine_from_this_Function();
                    break;
            }
        }

        // 현재 소지중인 정보가 있으면 => 표시해주기 // 없으면 하지말기
        public static void Show_Again_Now() {
            if (unit_id < 1000 || combine_content == null || type == 0) return;

            Clean_Combine_Table();
            switch (type) {
                case 1:
                    if(unit_id == 1003) {
                        Show_Combine_Table(1001, false, 1);
                        Show_Combine_Table(1002, false, 1);
                        Show_Combine_Table(1003, false, 1);
                    }
                    else 
                        Show_Combine_from_this_Function();
                    break;
                case 2:
                    Show_Combine_to_this_Function();
                    break;
                case 3:
                    Show_Combine_to_this_Function();
                    Show_Combine_from_this_Function();
                    break;
            }
        }

        // unit_id로 만들 수 있는 조합
        private static void Show_Combine_from_this_Function() {
            // unit id가 101 102 ( 아이템 ) 이면 title만 보여주고 보여주지 말까
            if(unit_id < 1000) {
                GameObject _title = Combine_Pool.Instance.Get_Function(1);
                _title.transform.SetParent(combine_content.transform, false);
                _title.transform.localScale = Vector3.one;
                _title.transform.Find("text").gameObject.GetComponent<TMP_Text>().text = Combine_Item_Title;
                return;
            }

            // 중복 검사
            Unit unit = Game_Data.unit[unit_id];

            int size = unit.combine_function.Count;

            // size 가 1 이상이면 title을 생성해줘야함
            if(size >= 1) {
                GameObject _title = Combine_Pool.Instance.Get_Function(1);
                _title.transform.SetParent(combine_content.transform, false);
                _title.transform.localScale = Vector3.one;
                _title.transform.Find("text").gameObject.GetComponent<TMP_Text>().text = Combine_from_this_Title;
            }
            
            // 도감에서는 배경없이 그대로 진행해줌
            // 인게임 도감에서는 현재 만들 수있는것도 표기해줌
            // 외부(홈화면) 도감에서는 counter를 0으로 바꿔주며 어짜피 배경이 뜨지 않을 것
            for (int i = 0; i < size; i++) {
                Setting_Combine_Function(unit, i);
            }
            Clear_Key_Checker();
        }

        // unit_id를 만들 수 있는 조합
        private static void Show_Combine_to_this_Function() {
            GameObject _title;
            // unit_id의 등급 미만만 검색해서 result가 unit_id인 친구를 보여주면 됨

            // unit_id가 1000대면 
            // 조합할 수 있는 유닛이 없는것이므로 소환으로 획득가능하다는 문구 보여주기
            if (unit_id < 2000 && unit_id > 1000) {
                _title = Combine_Pool.Instance.Get_Function(1);
                _title.transform.SetParent(combine_content.transform, false);
                _title.transform.localScale = Vector3.one;
                _title.transform.Find("text").GetComponent<TMP_Text>().text = Combine_Default_Title;
                return;
            }
            // 아이템일 경우
            else if(unit_id < 1000) {
                _title = Combine_Pool.Instance.Get_Function(1);
                _title.transform.SetParent(combine_content.transform, false);
                _title.transform.localScale = Vector3.one;
                _title.transform.Find("text").GetComponent<TMP_Text>().text = Combine_Item_Title;
                return;
            }

            // 미만 등급 검사를 위해서는 unit을 돌면서 이하 등급의 유닛들을 살펴봐야함
            // 등급 이하 표시
            Dictionary<string, bool> grade_checker = new() {
                { "e", false },
                { "d", false },
                { "c", false },
                { "b", false },
                { "a", false },
                { "s", false },
                { "ex", false }
            };

            // grade_num 표기
            // e = 0, d = 1 c = 2 .. ex = 6

            switch (unit[unit_id].grade) {
                case "d":
                    grade_checker["e"] = true;
                    break;
                case "c":
                    grade_checker["e"] = true;
                    grade_checker["d"] = true;
                    break;
                case "b":
                    grade_checker["e"] = true;
                    grade_checker["d"] = true;
                    grade_checker["c"] = true;
                    break;
                case "a":
                    grade_checker["e"] = true;
                    grade_checker["d"] = true;
                    grade_checker["c"] = true;
                    grade_checker["b"] = true;
                    break;
                case "s":
                    grade_checker["e"] = true;
                    grade_checker["d"] = true;
                    grade_checker["c"] = true;
                    grade_checker["b"] = true;
                    grade_checker["a"] = true;
                    break;
                case "ex":
                    grade_checker["e"] = true;
                    grade_checker["d"] = true;
                    grade_checker["c"] = true;
                    grade_checker["b"] = true;
                    grade_checker["a"] = true;
                    grade_checker["s"] = true;
                    break;
            }

            // title
            _title = Combine_Pool.Instance.Get_Function(1);
            _title.transform.SetParent(combine_content.transform, false);
            _title.transform.localScale = Vector3.one;
            _title.transform.Find("text").gameObject.GetComponent<TMP_Text>().text = Combine_to_this_Title;


            // 전체 유닛을 돌며 해당 등급을 확인하기
            foreach (var unit in unit) {

                // grade에 해당하는 번호가 true면 확인하고 false 면 넘기기
                if (!grade_checker[unit.Value.grade]) continue;

                int size = unit.Value.combine_function.Count;
                for(int i = 0; i < size; i++) {
                    if (unit.Value.combine_function[i].result != unit_id) continue;

                    Setting_Combine_Function(unit.Value, i);
                }
            }

            Clear_Key_Checker();
        }
    }
}
