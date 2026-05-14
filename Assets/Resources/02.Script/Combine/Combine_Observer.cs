using UnityEngine;
using static CUSTOM_DATA.Game_Data;
using CUSTOM_DATA;
using System.Collections.Generic;
using System.Linq;

/// <summary>
///  유닛을 클릭했을 때 조합식을 불러오는 함수
/// </summary>
public class Combine_Observer : Scene_Singleton<Combine_Observer>
{
    private static readonly string require_pure_unit_count = "조합에 필요한\n 유닛이 모자랍니다.";
    private static readonly string require_hand_unit_count = "조합에 필요한\n 유닛 카드가 모자랍니다.";
    private static readonly string require_hand_combine = "기둥이 되는 유닛과\n 결과가 될 유닛의 토대가 다릅니다.";

    public GameObject combine_field_content;
    static readonly int[] combine_ids = new int[6] { -1, -1, -1, -1, -1, -1 };
    private static readonly Dictionary<int, int> combine_checker = new();

    private static void Clear_Combine_id_Checker() {
        // combine_ids 초기화
        for (int c = 0; c < 6; c++) combine_ids[c] = -1;
    }
    private void Start() {
        Combine_Function_Creater.Set_Combine_Content(combine_field_content);
    }
    // 손패에서 유닛을 클릭했을 때 조합표에 조합식을 표시해주기
    public void Set_Combine_Field_from_Click(int unit_id) {
        if( unit_id == 1000) {
            Combine_Function_Creater.Clean_Combine_Table();
            Combine_Function_Creater.Show_Combine_Table(1001, false, 1);
            Combine_Function_Creater.Show_Combine_Table(1002, false, 1);
            Combine_Function_Creater.Show_Combine_Table(1003, false, 1);
        }
        else {
            Combine_Function_Creater.Clean_Combine_Table();
            Combine_Function_Creater.Show_Combine_Table(unit_id, false, 1);
        }
    }

    // 조합식에서 결과가 아닌 유닛을 클릭했을 때 이 유닛을 만들 수 있는 조합 + 유닛으로 만들 수 있는 조합 보여주기
    public void Set_Combine_Field_from_Combine_Material(int unit_id) {
        Combine_Function_Creater.Clean_Combine_Table();
        Combine_Function_Creater.Show_Combine_Table(unit_id, false, 3);
    }

    // 필드에서 유닛을 클릭했을 때 조합식을 보여주기 위한 함수
    public void Set_Combine_Field_from_Field_Click(int unit_id) {
        // is_field는 반드시 true
        // unit_id로 만들 수 있는 유닛들 조합식 보여주기
        Combine_Function_Creater.Clean_Combine_Table();
        Combine_Function_Creater.Show_Combine_Table(unit_id, true, 1);
    }

    public void Set_Combine_Book_Click(int unit_id) {
        Combine_Function_Creater.Clean_Combine_Table();
        Combine_Function_Creater.Show_Combine_Table(unit_id, false, 3);
    }

    // 조합이 가능한지 확인하고 가능하면 조합 
    public void Combine_Checker(int function_id, bool is_field) {
        // 손패 조합이면 손패에 모든 조합 재료가 있어야함
        // 필드 조합이면 -> 필드의 클릭한 유닛 한개 를 제외한 나머지 유닛이 손패에 있어야함(다른 필드 유닛은 건드리지 않음
        // function_id는 필드에서 유닛 클릭 할 때 나타나는 식으로만 보여주기 때문에 반드시 _unit이 필드의 유닛일 것

        // function_id의 재료들이 전부 있는지 확인
        Unit _unit = new();
        // check
        int target_id = -1;
        bool find_function = false;
        foreach (var target_unit in unit) {
            int function_checker = target_unit.Value.combine_function.Count;
            for (int j = 0; j < function_checker; j++) {
                if (target_unit.Value.combine_function[j].id == function_id) {
                    _unit = target_unit.Value;
                    target_id = j;
                    find_function = true;
                    break;
                }

            }
            if (find_function) break;
        }

        if (target_id < 0) {
            // something is wrong
            //Debug.Log("combine_checker =>> dont have this function id > " + function_id);
            return;
        }

        int can_combine = 0;

        if (is_field) {
            combine_ids[0] = -1;
        }
        else {
            combine_ids[0] = _unit.id > 0 ? _unit.id : 0;
        }
        
        combine_ids[1] = _unit.combine_function[target_id].a > 0 ? _unit.combine_function[target_id].a : 0;
        combine_ids[2] = _unit.combine_function[target_id].b > 0 ? _unit.combine_function[target_id].b : 0;
        combine_ids[3] = _unit.combine_function[target_id].c > 0 ? _unit.combine_function[target_id].c : 0;
        combine_ids[4] = _unit.combine_function[target_id].piece > 0 ? 101 : 0;
        combine_ids[5] = _unit.combine_function[target_id].crystal > 0 ? 102 : 0;
        
        combine_checker.Clear();
        if (combine_ids[0] > 0) {
            combine_checker.Add(combine_ids[0], 1);
        }

        if (combine_checker.ContainsKey(_unit.combine_function[target_id].a)) combine_checker[_unit.combine_function[target_id].a] += 1;
        else combine_checker.Add(_unit.combine_function[target_id].a, 1);

        if (_unit.combine_function[target_id].b > 0) {
            if (combine_checker.ContainsKey(_unit.combine_function[target_id].b)) combine_checker[_unit.combine_function[target_id].b] += 1;
            else combine_checker.Add(_unit.combine_function[target_id].b, 1);
        }

        if (_unit.combine_function[target_id].c > 0) {
            if (combine_checker.ContainsKey(_unit.combine_function[target_id].c)) combine_checker[_unit.combine_function[target_id].c] += 1;
            else combine_checker.Add(_unit.combine_function[target_id].c, 1);
        }
        
        if (_unit.combine_function[target_id].piece > 0) combine_checker.Add(101, _unit.combine_function[target_id].piece);
        if (_unit.combine_function[target_id].crystal > 0) combine_checker.Add(102, _unit.combine_function[target_id].crystal);

        // field 면 _unit.id 와 동일한 원소 제외해야함
        if (is_field && combine_checker.ContainsKey(_unit.id)) combine_checker[_unit.id] -= 1;

        for(int i = 0; i < 6; i++) {
            if (combine_ids[i] > 0) {
                // unit_counter 체크
                if (combine_ids[i] == 101) {
                    if (piece_count < combine_checker[combine_ids[i]]) {
                        can_combine = 1;
                        break;
                    }
                }
                else if (combine_ids[i] == 102) {
                    if (stone_count < combine_checker[combine_ids[i]]) {
                        can_combine = 1;
                        break;
                    }
                }
                else {
                    if (combine_checker[combine_ids[i]] > unit_counter[combine_ids[i]].count) {
                        can_combine = 1;
                        continue;
                    }
                }
        
                // 손패 체크
                if (combine_ids[i] > 2000) {
                    if (!Card_Observer.Instance.Card_Check_in_Install(combine_ids[i], combine_checker[combine_ids[i]])) {
                        can_combine = 2;
                        break;
                    }
                }
            }
        }
        Clear_Combine_id_Checker();

        // 마지막 체크
        // field 에서 별/ 달 조합을 시도할 때 unit_id가 별 / 달이 아니면 조합불가 판정
        if (unit[_unit.combine_function[target_id].result].type.Contains("st") || unit[_unit.combine_function[target_id].result].type.Contains("mo")) {
            if( !(_unit.type.Contains("st") || _unit.type.Contains("mo")) && is_field){
                // 조합 불가
                can_combine = 3;
            }
        }
        
        // 조합 불가
        switch (can_combine) {
            // 조합 가능
            case 0:
                Combine.Instance.Combine_Unit(_unit.id, target_id, is_field);
                return;
            // 유닛 부족
            case 1:
                Pannel_Controller.Instance.Activate_for_Announce(require_pure_unit_count);
                break;
            // 손패에 유닛이 없음
            case 2:
                Pannel_Controller.Instance.Activate_for_Announce(require_hand_unit_count);
                break;
                // 필드에서 별 달이 아닌 유닛으로부터 별 달 을 조합하려고 시도함 (손패에서만 가능하다는 알림)
            case 3:
                Pannel_Controller.Instance.Activate_for_Announce(require_hand_combine);
                break;
        }        
    }
}
