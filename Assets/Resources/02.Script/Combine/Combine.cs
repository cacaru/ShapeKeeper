using UnityEngine;
using static CUSTOM_DATA.Game_Data;

public class Combine : Scene_Singleton<Combine>
{

    // 현재 unit id와 combine id를 받아서
    // unit[unit_id].combine_function[i].id == combine_id 인 조합식을 찾아
    // unit_counter에서 unit_id a b c -- // result ++ 

    // 현재 클릭한 유닛이 field에서 눌린것이라면
    // field에 유닛 위치에 조합하기
     
    // field에서 눌린 것이 아니라면 -> 손패에 조합하기

    public void Combine_Unit(int unit_id, int function_id, bool is_field) {
        // 조합 -> 갯수 를 빼고
        // 죽인 갯수의 친구들을 손패에서 제거해야함
        // e등급이면 unit_counter에서만 줄이면 될듯
        // counter가 변화한 것을 알려서 손패에 있는 것들을 다시 그리게 할 수 있어야함
        // 손패 목록을 감시하는 observer를 생성해줘야함

        // 조합될 위치
        if (is_field) {
            // 필드
            // 필드의 유닛을 변경
            // target이 별달이면 변경 불가
            // 필드 타겟 유닛의 카운터 내리기
            unit_counter[unit_id].count--;
            // 손패의 유닛들의 카운터 내리기
            if (unit[unit_id].combine_function[function_id].a > 0) {
                Card_Observer.Instance.Delete_Unit_Card_in_Install(unit[unit_id].combine_function[function_id].a);
            }
            if (unit[unit_id].combine_function[function_id].b > 0) {
                Card_Observer.Instance.Delete_Unit_Card_in_Install(unit[unit_id].combine_function[function_id].b);
            }
            if (unit[unit_id].combine_function[function_id].c > 0) {
                Card_Observer.Instance.Delete_Unit_Card_in_Install(unit[unit_id].combine_function[function_id].c);
            }
            if (unit[unit_id].combine_function[function_id].piece > 0) {
                piece_count -= unit[unit_id].combine_function[function_id].piece;
            }
            if (unit[unit_id].combine_function[function_id].crystal > 0) {
                stone_count -= unit[unit_id].combine_function[function_id].crystal;
            }

            // 조합한 결과물 result의 카운터 증가
            unit_counter[unit[unit_id].combine_function[function_id].result].count += 1;
            Card_Observer.Instance.Field_Card_Combine_Checker(unit_id);

            // 현재 클릭된 유닛을 변경하기
            // 현재 클릭한 유닛을 완전히 제거하고 result 등급의 새로운 prefab을 생성해야함
            Transform target = Field_Unit_Click_Setter.Instance.target.transform;
            var target_pos = target.position;
            
            // ora가 있는지 검사하고 있으면 회수하기
            foreach(Transform t in target) {
                if (t.name.Contains("Ora")) {
                    var type = target.gameObject.GetComponent<Attack>().Type;
                    Ora_Pool.Instance.Return(type, t.gameObject);
                    Spell_Generate.Instance.Recall(type);
                }
            }

            Destroy(Field_Unit_Click_Setter.Instance.target);

            TowerInstaller.Instance.Install_Tower(unit[unit_id].combine_function[function_id].result, target_pos);
        }
        else {
            // 손패
            // 조합식의 유닛들의 유닛 카운터를 내리기
            Card_Observer.Instance.Delete_Unit_Card_in_Install(unit_id);

            if (unit[unit_id].combine_function[function_id].a > 0) {
                Card_Observer.Instance.Delete_Unit_Card_in_Install(unit[unit_id].combine_function[function_id].a);
            }
            if (unit[unit_id].combine_function[function_id].b > 0) {
                Card_Observer.Instance.Delete_Unit_Card_in_Install(unit[unit_id].combine_function[function_id].b);
            }   
            if (unit[unit_id].combine_function[function_id].c > 0) {
                Card_Observer.Instance.Delete_Unit_Card_in_Install(unit[unit_id].combine_function[function_id].c);
            }
            if (unit[unit_id].combine_function[function_id].piece > 0) {
                piece_count -= unit[unit_id].combine_function[function_id].piece;
            }   
            if (unit[unit_id].combine_function[function_id].crystal > 0) {
                stone_count -= unit[unit_id].combine_function[function_id].crystal;
            }
            
            // 조합한 결과물 result의 카운터 증가
            unit_counter[unit[unit_id].combine_function[function_id].result].count += 1;

            Card_Observer.Instance.New_Card(unit[unit_id].combine_function[function_id].result);
        }

        // 업적 확인    
        Achievement_Observer.Instance.Combine_Achievement(unit[unit[unit_id].combine_function[function_id].result].grade);

        In_Game_Setter.Instance.Set_Goods();
        // combine이 완료되면 조합식을 끄기
        Pannel_Controller.Instance.Deactivate_for_Combine();
        Pannel_Controller.Instance.Deactivate_for_UnitInfo();
    }
}
