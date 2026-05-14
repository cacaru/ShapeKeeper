using UnityEngine;
using static CUSTOM_DATA.Common_Data;
using static CUSTOM_DATA.Game_Data;
using CUSTOM_DATA;

public class Recall : Scene_Singleton<Recall>
{
    // 타워를 회수하고 -> 카드로 보여주게 하기
    // 회수할 아이디를 받으면 회수 할 수 있게 하면될듯
    readonly TOWER_BASE tower_base = new();
    int space_count = 0;
    bool vertical = false;
    Vector2 pos;

    private Vector3 mouse_left_up = new();
    private Vector3 mouse_left_down = new();
    private Vector3 mouse_right_up = new();
    private Vector3 mouse_right_down = new();
    private Vector3 mouse_up = new();
    private Vector3 mouse_down = new();

    public void Recall_Unit(int unit_id, GameObject target_obj, int tower_id) {

        Card_Observer.Instance.Card_Hider(unit_id, true);
        installed_tower.Remove(tower_id);
        // field 치우기
        // target_obj의 근처를 살펴서 타워를 철거해야함
        pos = target_obj.transform.position;
        
        tower_base.space = unit[unit_id].type.Split("_")[1] switch {
            "1" => Tower_Space._4,
            "2" => Tower_Space._2v,
            "3" => Tower_Space._2h,
            "4" => Tower_Space._6h,
            "5" => Tower_Space._6v,
            _ => Tower_Space._4
        };

        if (tower_base.space == Tower_Space._2h || tower_base.space == Tower_Space._2v) space_count = 2;
        else if (tower_base.space == Tower_Space._6h || tower_base.space == Tower_Space._6v) space_count = 6;
        else space_count = 4;

        if (tower_base.space == Tower_Space._2v || tower_base.space == Tower_Space._6v) vertical = true;
        else vertical = false;

        float length_to_check_vertical = 0;
        float length_to_check_horizontal = 0;
        float length_to_check_up_down = 0;
        float length_to_check_left_right = 0;

        switch (space_count) {
            case 2:
                if (vertical) {
                    length_to_check_vertical = 0.2f;
                }
                else {
                    length_to_check_horizontal = 0.2f;
                }
                break;
            case 4:
                length_to_check_vertical = 0.2f;
                length_to_check_horizontal = 0.2f;
                length_to_check_up_down = 0.1f;
                length_to_check_left_right = 0.1f;
                break;
            case 6:
                if (vertical) {
                    length_to_check_vertical = 0.4f;
                    length_to_check_horizontal = 0.2f;
                    length_to_check_up_down = 0f;
                    length_to_check_left_right = 0.2f;
                }
                else {
                    length_to_check_vertical = 0.2f;
                    length_to_check_horizontal = 0.4f;
                    length_to_check_up_down = 0.2f;
                    length_to_check_left_right = 0f;
                }
                break;
        }

        // 현재 타워의 부지가 된 곳을 찾기
        mouse_left_up.Set(pos.x - length_to_check_horizontal, pos.y + length_to_check_vertical, 1);
        mouse_left_down.Set(pos.x - length_to_check_horizontal, pos.y - length_to_check_vertical, 1);
        mouse_right_up.Set(pos.x + length_to_check_horizontal, pos.y + length_to_check_vertical, 1);
        mouse_right_down.Set(pos.x + length_to_check_horizontal, pos.y - length_to_check_vertical, 1);
        mouse_up.Set(pos.x + length_to_check_left_right, pos.y + length_to_check_up_down, 1);
        mouse_down.Set(pos.x - length_to_check_left_right, pos.y - length_to_check_up_down, 1);

        tower_base.left_up = load_map.WorldToCell(mouse_left_up);
        tower_base.left_down = load_map.WorldToCell(mouse_left_down);
        tower_base.right_up = load_map.WorldToCell(mouse_right_up);
        tower_base.right_down = load_map.WorldToCell(mouse_right_down);
        tower_base.up = load_map.WorldToCell(mouse_up);
        tower_base.down = load_map.WorldToCell(mouse_down);

        TowerInstaller.Instance.UnInstall_Tower(tower_base);

        // tower base reset
        tower_base.left_up.Set(0, 0, 0);
        tower_base.left_down.Set(0, 0, 0);
        tower_base.right_up.Set(0, 0, 0);
        tower_base.right_down.Set(0, 0, 0);
        tower_base.up.Set(0, 0, 0);
        tower_base.down.Set(0, 0, 0);

        // target obj 아래 ora가 있는지 확인하고 있으면 return

        foreach (Transform child in target_obj.transform) {
            if (!child.name.Contains("Ora")) continue;

            OraType type = child.GetComponent<SpriteRenderer>().sprite.name.Split("_")[0] switch {
                "ice" => OraType.Ice,
                "fire" => OraType.Fire,
                "poison" => OraType.Poison,
                "water" => OraType.Water,
                _ => OraType.None
            };
            
            Ora_Pool.Instance.Return(type, child.gameObject);
            Spell_Generate.Instance.Recall(type);
        }

        Destroy(target_obj);

        // 모든 적에게 길 새로 파게 하기
        Spawner.Instance.Remapping();
    }

}
