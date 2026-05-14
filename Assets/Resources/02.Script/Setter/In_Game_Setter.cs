using UnityEngine;
using CUSTOM_DATA;
using UnityEngine.Tilemaps;
using TMPro;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Game_Value_Data;
using static CUSTOM_DATA.Game_State_Data;
using static CUSTOM_DATA.Common_Data;

/// <summary>
/// 게임을 시작할 때 오브젝트들을 세팅해둘 변수
/// >> 세팅을 마치면 게임을 시작할 준비가 되었다고 알리기
/// </summary>
public class In_Game_Setter : Scene_Singleton<In_Game_Setter>
{
    [SerializeField] private TMP_Text now_dot;
    [SerializeField] private TMP_Text piece;
    [SerializeField] private TMP_Text stone;

    [SerializeField] private TMP_Text mission_cost_d;
    [SerializeField] private TMP_Text mission_cost_c;
    [SerializeField] private TMP_Text mission_cost_b;
    [SerializeField] private TMP_Text mission_cost_a;
    [SerializeField] private TMP_Text mission_cost_s;

    [SerializeField] private TMP_Text mission_hp_d;
    [SerializeField] private TMP_Text mission_hp_c;
    [SerializeField] private TMP_Text mission_hp_b;
    [SerializeField] private TMP_Text mission_hp_a;
    [SerializeField] private TMP_Text mission_hp_s;
    

    private readonly string monster_load = "monster_load";
    private readonly string show = "SHOW";
    private readonly string back = "Back";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // map 설정
        string plannet = Now_plannet switch {
            Plannet.Green => "Green_",
            Plannet.Blue => "Blue_",
            Plannet.Gray => "Gray_",
            Plannet.Ancient => "Ancient_",
            _ => "Green_"
        };
        
        load_map_prefab = Resources.Load<GameObject>("03.Prefab/UI/"+ plannet + monster_load);
        show_map_prefab = Resources.Load<GameObject>("03.Prefab/UI/"+ plannet + show);
        back_map_prefab = Resources.Load<GameObject>("03.Prefab/UI/"+ plannet + back);

        var load_map_instance = Instantiate(load_map_prefab);
        load_map_instance.transform.SetParent(transform);
        load_map = load_map_instance.GetComponent<Tilemap>();

        var show_map_instance = Instantiate(show_map_prefab);
        show_map_instance.transform.SetParent(transform);
        show_map = show_map_instance.GetComponent<Tilemap>();

        var back_map_instance = Instantiate(back_map_prefab);
        back_map_instance.transform.SetParent(transform);

        // 저항생성 확인
        if(difficulty > 1) Generate_Resistance_Field();
        

        data_setting_complete = true;
        is_gaming = true;
        // 각 재화 컨트롤
        Set_Goods();

        // 미션 보상 컨트롤
        mission_cost_d.text = (50 + (difficulty - 1) * 10).ToString();
        mission_cost_c.text = (100 + (difficulty - 1) * 10).ToString();
        mission_cost_b.text = (150 + (difficulty - 1) * 10).ToString();
        mission_cost_a.text = (200 + (difficulty - 1) * 10).ToString();
        mission_cost_s.text = (250 + (difficulty - 1) * 10).ToString();

        // 미션 몹 체력 표기
        int hp_value = 1 * 5000 + ((difficulty - 1) * 500 * 1);
        mission_hp_d.text = hp_value.ToString() + " / " + hp_value.ToString();
        hp_value = 2 * 5000 + ((difficulty - 1) * 500 * 2);
        mission_hp_c.text = hp_value.ToString() + " / " + hp_value.ToString();
        hp_value = 3 * 5000 + ((difficulty - 1) * 500 * 3);
        mission_hp_b.text = hp_value.ToString() + " / " + hp_value.ToString();
        hp_value = 4 * 5000 + ((difficulty - 1) * 500 * 4);
        mission_hp_a.text = hp_value.ToString() + " / " + hp_value.ToString();
        hp_value = 5 * 5000 + ((difficulty - 1) * 500 * 5);
        mission_hp_s.text = hp_value.ToString() + " / " + hp_value.ToString();
    }
 

    public void Set_Goods() {
        now_dot.text = Utility.FloatFormatter(dot);
        piece.text = piece_count.ToString();
        stone.text = stone_count.ToString();
    }

    private void Generate_Resistance_Field() {
        // 난이도에 따라 맵에 랜덤한 저항지형 생성
        int counter = difficulty - 1;
        var prefab = green_resistance_show_prefab;

        for (int i = 0; i < counter; i++) {
            GameObject instance = Instantiate(prefab);
            bool setting = false;
            while (!setting) {
                // 랜덤 위치 생성
                // x -6~5 y 9 ~ -10
                int x = Random.Range(-6, 6);
                int y = Random.Range(-10, 10);
                Vector3Int tile_pos = new(x, y, 0);
                var target_tile = load_map.GetTile(tile_pos);
                // target_tile이 이미 변경되어 있으면 넘어감
                if (target_tile == null) continue;
                if (target_tile.name.Contains("74")) continue;

                var load_prefab = load_map_back_tile;

                var show_prefab = Now_plannet switch {
                    Plannet.Green => green_show_map_back_tile,
                    Plannet.Blue => blue_show_map_back_tile,
                    Plannet.Gray => gray_show_map_back_tile,
                    Plannet.Ancient => ancient_show_map_back_tile,
                    _ => green_show_map_back_tile
                };

                // 변경
                load_map.SetTile(tile_pos, load_prefab);
                show_map.SetTile(tile_pos, show_prefab);

                // 해당 위치에 프리팹 위치시키기
                var ins_pos = load_map.GetCellCenterWorld(tile_pos);
                ins_pos.y += .175f;
                instance.transform.position = ins_pos;
                setting = true;
            }
        }
        
    }


}
