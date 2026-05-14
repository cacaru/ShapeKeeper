using CUSTOM_DATA;
using UnityEngine;
using UnityEngine.Tilemaps;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Game_State_Data;
using static CUSTOM_DATA.Common_Data;
using System.Collections.Generic;


public class TowerInstaller : Scene_Singleton<TowerInstaller>
{
    private TOWER_BASE tmp_tower_base;
    private int tower_id = 0;

    private readonly Dictionary<(Tower_Space, string), Vector2> size_table = new() {
        { (Tower_Space._2h, "d"), new Vector2(4f, 2f) },
        { (Tower_Space._2h, "c"), new Vector2(2.5f, 1.3f) },
        { (Tower_Space._2h, "b"), new Vector2(2.2f, 1.1f) },
        { (Tower_Space._2h, "a"), new Vector2(1.9f, 1f) },
        { (Tower_Space._2h, "s"), new Vector2(1.72f, 0.87f) },
        { (Tower_Space._2h, "ex"), new Vector2(1.5f, 0.7f) },

        { (Tower_Space._2v, "d"), new Vector2(2f, 4f) },
        { (Tower_Space._2v, "c"), new Vector2(1.3f, 2.5f) },
        { (Tower_Space._2v, "b"), new Vector2(1.1f, 2.2f) },
        { (Tower_Space._2v, "a"), new Vector2(1f, 1.9f) },
        { (Tower_Space._2v, "s"), new Vector2(0.87f, 1.72f) },
        { (Tower_Space._2v, "ex"), new Vector2(0.7f, 1.5f) },

        { (Tower_Space._4, "d"), new Vector2(4f, 4f) },
        { (Tower_Space._4, "c"), new Vector2(2.5f, 2.5f) },
        { (Tower_Space._4, "b"), new Vector2(2.2f, 2.2f) },
        { (Tower_Space._4, "a"), new Vector2(1.9f, 1.9f) },
        { (Tower_Space._4, "s"), new Vector2(1.72f, 1.72f) },
        { (Tower_Space._4, "ex"), new Vector2(1.5f, 1.5f) },

        { (Tower_Space._6h, "c"), new Vector2(3.8f, 2.56f) },
        { (Tower_Space._6h, "b"), new Vector2(3.17f, 2.18f) },
        { (Tower_Space._6h, "a"), new Vector2(2.72f, 1.88f) },
        { (Tower_Space._6h, "s"), new Vector2(2.46f, 1.64f) },
        { (Tower_Space._6h, "ex"), new Vector2(2.19f, 1.45f) },

        { (Tower_Space._6v, "c"), new Vector2(2.56f, 3.8f) },
        { (Tower_Space._6v, "b"), new Vector2(2.18f, 3.17f) },
        { (Tower_Space._6v, "a"), new Vector2(1.88f, 2.72f) },
        { (Tower_Space._6v, "s"), new Vector2(1.64f, 2.46f) },
        { (Tower_Space._6v, "ex"), new Vector2(1.45f, 2.19f) },
    };

    private readonly Dictionary<string, string> typeToImg = new() {
                                                                       { "ci", "_circle" },
                                                                       { "tr", "_triangle" },
                                                                       { "sq", "_square" },
                                                                       { "st", "_star" },
                                                                       { "mo", "_moon" },
                                                                  };

    private Dictionary<Tower_Space, Color> tower_color_map;

    void Start() {
        tower_color_map = new() {
            { Tower_Space._2h, ethereal_color },
            { Tower_Space._2v, hybrid_color },
            { Tower_Space._4, material_color },
            { Tower_Space._6h, special_material_color },
            { Tower_Space._6v, special_material_color },
        };
    }

    public void Temporary_Install_Tower(TOWER_BASE tower_base) {
        tmp_tower_base = tower_base;

        load_map.SetTile(tmp_tower_base.left_up, load_map_back_tile);
        load_map.SetTile(tmp_tower_base.left_down, load_map_back_tile);
        load_map.SetTile(tmp_tower_base.right_up, load_map_back_tile);
        load_map.SetTile(tmp_tower_base.right_down, load_map_back_tile);
        load_map.SetTile(tmp_tower_base.up, load_map_back_tile);
        load_map.SetTile(tmp_tower_base.down, load_map_back_tile);
    }

    public void Confirm_Install_Tower(TOWER_BASE tower_base) {
        tmp_tower_base = tower_base;
        // 타워를 설치해야하므로 설치할 타일을 골라야함 (rule이었으면 좋았으련만 추후 찾아보기
        switch (tmp_tower_base.space) {
            case Tower_Space._2v:
                // vertical up / down
                show_map.SetTile(tmp_tower_base.left_up, tower_base_vertical_up);
                show_map.SetTile(tmp_tower_base.right_down, tower_base_vertical_down);
                break;
            case Tower_Space._2h:
                // horizontal left / right
                show_map.SetTile(tmp_tower_base.left_up, tower_base_horizontal_left);
                show_map.SetTile(tmp_tower_base.right_up, tower_base_horizontal_right);
                break;
            case Tower_Space._4:
                // left up / left down / right up / right down
                show_map.SetTile(tmp_tower_base.left_up, tower_base_left_up);
                show_map.SetTile(tmp_tower_base.right_up, tower_base_right_up);
                show_map.SetTile(tmp_tower_base.left_down, tower_base_left_down);
                show_map.SetTile(tmp_tower_base.right_down, tower_base_right_down);
                break;
            case Tower_Space._6v:
                // left up, center, down / right up, center , down
                show_map.SetTile(tmp_tower_base.left_up, tower_base_left_up);
                show_map.SetTile(tmp_tower_base.left_down, tower_base_left_down);

                show_map.SetTile(tmp_tower_base.right_up, tower_base_right_up);
                show_map.SetTile(tmp_tower_base.right_down, tower_base_right_down);

                show_map.SetTile(tmp_tower_base.down, tower_base_left_center);
                show_map.SetTile(tmp_tower_base.up, tower_base_right_center);
                break;
            case Tower_Space._6h:
                // left up, down / center up, down / right up, down
                show_map.SetTile(tmp_tower_base.left_up, tower_base_left_up);
                show_map.SetTile(tmp_tower_base.left_down, tower_base_left_down);

                show_map.SetTile(tmp_tower_base.right_up, tower_base_right_up);
                show_map.SetTile(tmp_tower_base.right_down, tower_base_right_down);

                show_map.SetTile(tmp_tower_base.up, tower_base_center_up);
                show_map.SetTile(tmp_tower_base.down, tower_base_center_down);
                break;
        }

        Spawner.Instance.Remapping();
    }

    public void UnInstall_Tower(TOWER_BASE tower_base) {

        TileBase map_base_tile = Now_plannet switch {
            Plannet.Green => map_base_green_tile,
            Plannet.Blue => map_base_blue_tile,
            Plannet.Gray => map_base_gray_tile,
            Plannet.Ancient => map_base_ancient_tile,
            _ => map_base_green_tile,
        };

        load_map.SetTile(tower_base.left_up, map_base_tile);
        load_map.SetTile(tower_base.left_down, map_base_tile);
        load_map.SetTile(tower_base.right_up, map_base_tile);
        load_map.SetTile(tower_base.right_down, map_base_tile);
        load_map.SetTile(tower_base.up, map_base_tile);
        load_map.SetTile(tower_base.down, map_base_tile);

        show_map.SetTile(tower_base.left_up, map_base_tile);
        show_map.SetTile(tower_base.left_down, map_base_tile);
        show_map.SetTile(tower_base.right_up, map_base_tile);
        show_map.SetTile(tower_base.right_down, map_base_tile);
        show_map.SetTile(tower_base.up, map_base_tile);
        show_map.SetTile(tower_base.down, map_base_tile);
    }

    public void Install_Tower(int _unit_id, Vector3 pos ) {
        var _unit = unit[_unit_id];
        GameObject tower_prefab = _unit.grade switch {
            "d" => Game_Value_Data.d_prefab,
            "c" => Game_Value_Data.c_prefab,
            "b" => Game_Value_Data.b_prefab,
            "a" => Game_Value_Data.a_prefab,
            "s" => Game_Value_Data.s_prefab,
            "ex" => Game_Value_Data.ex_prefab,
            _ => Game_Value_Data.d_prefab
        };


        var type_parts = _unit.type.Split("_");
        // unit type에 따라 ci / tr / sq / st / mo 이미지 생성

        
        string img_type = typeToImg.TryGetValue(type_parts[0], out var value) ? value : "_circle";

        Sprite img = Utility.Get_sprite(_unit.grade + img_type);

        var tower_obj = Instantiate(tower_prefab, pos, Quaternion.identity);
        tower_obj.transform.Find("Ori").gameObject.GetComponent<SpriteRenderer>().sprite = img;
        tower_obj.GetComponent<Combine_Field>().id = _unit_id;
        tower_obj.GetComponent<Combine_Field>().tower_id = tower_id;
        // colider 크기를 바닥 크기만큼 늘려야함
        // base의 크기에 따라 달라져야함
        Vector2 custom_size = new(1, 1);
        Color color;

        var grade = _unit.grade;
        var space = type_parts[1] switch {
            "1" => Tower_Space._4,
            "2" => Tower_Space._2v,
            "3" => Tower_Space._2h,
            "4" => Tower_Space._6h,
            "5" => Tower_Space._6v,
            _ => Tower_Space._4
        };
        
        // 타워별 분류
        if (size_table.TryGetValue((space, grade), out var size)) {
            custom_size = size;
        }

        if (tower_color_map.TryGetValue(space, out var col)) {
            color = col;
        }
        else {
            color = material_color;
        }

        tower_obj.GetComponent<BoxCollider2D>().size = custom_size;
        tower_obj.GetComponent<BoxCollider2D>().offset = Vector2.zero;

        var shadow = tower_obj.transform.Find("Shadow");
        if (shadow) {
            shadow.GetComponent<SpriteRenderer>().sprite = img;
        }

        var satellite = tower_obj.transform.Find("Satellite");
        if (satellite) {
            img = _unit.grade.Equals("a") || _unit.grade.Equals("s") ? Utility.Get_sprite("d" + img_type) : img;
            satellite.GetComponent<SpriteRenderer>().sprite = img;
            satellite.GetComponent<SpriteRenderer>().color = color;
        }
        var satellite_2 = tower_obj.transform.Find("Satellite_2");
        if (satellite_2) {
            satellite_2.GetComponent<SpriteRenderer>().sprite = img;
            satellite_2.GetComponent<SpriteRenderer>().color = color;
        }

        tower_obj.transform.Find("Ori").gameObject.GetComponent<SpriteRenderer>().color = color;

        // 현재 설치된 타워를 저장해둠
        // 각각 고유 id 부여
        Installed_Tower tower = new(tower_id, _unit.attack, _unit.material, _unit.ethereal, tower_obj, pos);
        installed_tower.Add(tower_id, tower);

        // attack 설정
        tower_obj.GetComponent<Attack>().Init(_unit_id, tower_id);

        tower_id++;
    }
}
