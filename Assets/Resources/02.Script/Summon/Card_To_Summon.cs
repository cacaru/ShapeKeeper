using CUSTOM_DATA;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

using static CUSTOM_DATA.Common_Data;
using static CUSTOM_DATA.Game_Value_Data;
using static CUSTOM_DATA.Game_State_Data;
using static CUSTOM_DATA.Game_Data;
using System.Collections.Generic;

public class Card_To_Summon : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    // 유닛 card의 정보를 바탕으로 필드에 유닛을 생성하는 스크립트
    // 꾹 2초간 누르고 있으면 소환 가능하게 만들어야함

    // 한번만 누르면 조합창이 보여야함

    // 2초간 누르고있었으면 패널을 모두 치우고
    // 유닛에 해당하는 instance를 생성
    // 드랍으로 설치 검사
    public int unit_id = 0; // 프리팹을 생성할 때 적용해줌
    public int Unit_Id {
        get { return unit_id; }
        set {
            unit_id = value;
            Init(value);
            path_checker.Init();
        }
    }

    private readonly float check_time = 0.6f;
    private float hold_time = 0f;
    private bool install_active = false;

    public GameObject translucent_obj;
    public GameObject tower_prefab;
    public string tower_grade;

    private GameObject instance_translucent_obj;
    private GameObject instance_attack_area;
    private Vector3 click_pos;
    private readonly TOWER_BASE tower_base = new();
    private Vector3Int mouse_left_up = new();
    private Vector3Int mouse_left_down = new ();
    private Vector3Int mouse_right_up = new();
    private Vector3Int mouse_right_down = new();
    private Vector3Int mouse_up = new();
    private Vector3Int mouse_down = new();

    private Vector3 center = new();

    private bool can_install_tower = false;
    private bool can_shift_tower = false;

    private TowerInstaller installer;

    private readonly EnemyPathChecker path_checker = new();
    //private readonly Enemy_Path_Checker path_checker = new();

    private int space_count = 0;
    private bool vertical = false;
    private Color can_install_field_color = new(170 / 255f, 170 / 255f, 170 / 255f, 150 / 255f);

    private void Init(int unit_id) {
        if (unit_id < 2000) return;
        space_count = 0;
        vertical = false;

        // 설정된 id를 기반으로 prefab 불러오기
        // id에서 타입으로 접근하여 생성
        GameObject prefab;
        Unit unit_data = unit[unit_id];
        tower_grade = unit_data.grade;
        tower_prefab = prefab_container.TryGetValue(tower_grade, out prefab) ? prefab : d_prefab;
        translucent_obj = translucent_container.TryGetValue(tower_grade, out prefab) ? prefab : d_translucent_prefab;

        // tower base 결정
        tower_base.space = unit_data.type.Split("_")[1] switch {
            "1" => Tower_Space._4,
            "2" => Tower_Space._2v,
            "3" => Tower_Space._2h,
            "4" => Tower_Space._6h,
            "5" => Tower_Space._6v,
            _ => Tower_Space._4
        };

        //Debug.Log(unit_id + ", " + unit_data.type);
        space_count = tower_base.space switch {
            Tower_Space._2v or Tower_Space._2h => 2,
            Tower_Space._6v or Tower_Space._6h => 6,
            _ => 4
        };

        vertical = tower_base.space == Tower_Space._2v || tower_base.space == Tower_Space._6v;

        installer = TowerInstaller.Instance;

        click_pos = Vector3.zero;

        // 맵에 따라 설치될 구역의 색을 변경해줌
        can_install_field_color = Now_plannet switch {
            Plannet.Gray => new(100 / 255f, 255 / 255f, 0, 1f),
            Plannet.Green => new(75 / 255f, 75 / 255f, 75 / 255f, 1f),
            _ => new(170 / 255f, 170 / 255f, 170 / 255f, 150 / 255f)
        };
    }

    public void OnDrag(PointerEventData eventData) {
        if(!install_active) return;

        // 기존 영역 표시를 해제
        tower_base.Set_All_Color(MapID.show_map, Color.white);

        // 현재 마우스 위치의 cell을 찾고
        // 해당 셀의 좌 우 좌상 좌하 우상 우하 를 선택
        // 검사한다
        click_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        click_pos.z = 1;
        click_pos.y += .8f;

        var load_click_pos = load_map.WorldToCell(click_pos);

        int length_to_check_left = 0;
        int length_to_check_right = 0;
        int length_to_check_up = 0;
        int length_to_check_down = 0;
        int length_to_check_middle_up = 0;
        int length_to_check_middle_down = 0;
        int length_to_check_middle_left = 0;
        int length_to_check_middle_right = 0;

        switch (space_count) {
            case 2:
                if (vertical) {
                    length_to_check_up = 1;
                }
                else {
                    length_to_check_right = 1;
                }
                break;
            case 4:
                length_to_check_up = 1;
                length_to_check_left = 1;
                break;
            case 6:
                if (vertical) {
                    length_to_check_down = 1;
                    length_to_check_up = 1;
                    length_to_check_left = 1;
                    length_to_check_middle_left = 1;
                }
                else {
                    length_to_check_up = 1;
                    length_to_check_right = 1;
                    length_to_check_left = 1;
                    length_to_check_middle_up = 1;
                }
                break;
        }

        mouse_left_up.Set(load_click_pos.x - length_to_check_left, load_click_pos.y + length_to_check_up, 0);
        mouse_left_down.Set(load_click_pos.x - length_to_check_left, load_click_pos.y - length_to_check_down, 0);
        mouse_right_up.Set(load_click_pos.x + length_to_check_right, load_click_pos.y + length_to_check_up, 0);
        mouse_right_down.Set(load_click_pos.x + length_to_check_right, load_click_pos.y - length_to_check_down, 0);

        mouse_up.Set(load_click_pos.x + length_to_check_middle_right, load_click_pos.y + length_to_check_middle_up, 0);
        mouse_down.Set(load_click_pos.x - length_to_check_middle_left, load_click_pos.y - length_to_check_middle_down, 0);
            
        var left_up_tile = load_map.GetTile(mouse_left_up);
        var left_down_tile = load_map.GetTile(mouse_left_down);
        var right_up_tile = load_map.GetTile(mouse_right_up);
        var right_down_tile = load_map.GetTile(mouse_right_down);
        bool check_left_up = false,
            check_right_up = false,
            check_left_down = false,
            check_right_down = false,
            check_up = false,
            check_down = false;
            
        if (left_up_tile == null || left_down_tile == null || right_up_tile == null || right_down_tile == null) {
            click_pos.z = 1;
            center = click_pos;
            can_install_tower = false;
        }
        else {
            // 설치가능한지 확인 
            // 가능하면 색을 초록색으로 변경해줘야함
            // 불가하면 빨간색으로

            // 우선 설치 가능 영역을 저장해두기
            tower_base.left_up = mouse_left_up;
            tower_base.left_down = mouse_left_down;
            tower_base.right_up = mouse_right_up;
            tower_base.right_down = mouse_right_down;
            tower_base.up = mouse_up;
            tower_base.down = mouse_down;

            tower_base.Set_All_Color(MapID.show_map, Color.red);

            // field에 따라 확인해야하는 타일 번호가 다름
            string tile_str = Now_plannet switch {
                Plannet.Green => "54",
                Plannet.Blue => "71",
                Plannet.Gray => "801",
                Plannet.Ancient => "72",
                _ => "54"
            };

            can_shift_tower = false;
            // 완전히 동일한 형태의 유닛을 겹치면 교환되게 해야함
            if (load_map.GetTile(tower_base.left_up).name.Contains(tile_str)) {
                show_map.SetColor(tower_base.left_up, can_install_field_color);
                check_left_up = true;
            }

            if (load_map.GetTile(tower_base.left_down).name.Contains(tile_str)) {
                show_map.SetColor(tower_base.left_down, can_install_field_color);
                check_left_down = true;
            }

            if (load_map.GetTile(tower_base.right_up).name.Contains(tile_str)) {
                show_map.SetColor(tower_base.right_up, can_install_field_color);
                check_right_up = true;
            }

            if (load_map.GetTile(tower_base.right_down).name.Contains(tile_str)) {
                show_map.SetColor(tower_base.right_down, can_install_field_color);
                check_right_down = true;
            }

            if (load_map.GetTile(tower_base.up).name.Contains(tile_str)) {
                show_map.SetColor(tower_base.up, can_install_field_color);
                check_up = true;
            }

            if (load_map.GetTile(tower_base.down).name.Contains(tile_str)) {
                show_map.SetColor(tower_base.down, can_install_field_color);
                check_down = true;
            }

            if (show_map.GetTile(tower_base.left_up).name.Contains("Tower") &&
                show_map.GetTile(tower_base.left_down).name.Contains("Tower") &&
                show_map.GetTile(tower_base.right_up).name.Contains("Tower") &&
                show_map.GetTile(tower_base.right_down).name.Contains("Tower") &&
                show_map.GetTile(tower_base.up).name.Contains("Tower") &&
                show_map.GetTile(tower_base.down).name.Contains("Tower")
                ) {
                can_shift_tower = true;
            }

            can_install_tower = (check_left_up && check_left_down && check_right_up && check_right_down && check_up && check_down);

            var world_1 = load_map.CellToWorld(mouse_left_up);
            var world_2 = load_map.CellToWorld(mouse_right_down);

            float x = (world_1.x + world_2.x + .4f) / 2;
            float y = (world_1.y + world_2.y + .4f) / 2;

            // 좌상 / 우하의 2 좌표의 중앙값 -> 타워의 중앙이 될 부분을 기준으로 잡기
            center.Set(x, y, 1);
        }

        instance_translucent_obj.transform.position = center;
        instance_attack_area.transform.position = center;
    }

    public void OnPointerClick(PointerEventData eventData) {
        //Debug.Log(unit_id);
        // 조합식 화면 보여주기
        Combine_Observer.Instance.Set_Combine_Field_from_Click(unit_id);
        Pannel_Controller.Instance.Activate_for_Combine();
        // 유닛 상세 정보 보여주기
        if(unit_id > 2000) {
            Unit_Info_Pannel_Setter.Instance.Set_Info(unit_id);
            Pannel_Controller.Instance.Activate_for_UnitInfo();
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (unit_id < 2000) return;

        install_active = false;
        hold_time = 0;

        StartCoroutine(DownChecker());
    }

    public void OnPointerUp(PointerEventData eventData) {
        StopAllCoroutines();
        hold_time = 0;
        
        if (install_active) {
            install_active = false;
            tower_base.Set_All_Color(MapID.show_map, Color.white);

            bool is_correct_install;
            // 설치 가능한 지역인지 확인 (맵에 따라 살펴봐야할 타일이 다름)
            if (can_install_tower) {

                // tile을 토대로 변경하기
                installer.Temporary_Install_Tower(tower_base);
                // 현재 이동중인 적들의 경로 재설정
                // 적의 이동 경로를 막는지 확인 _ 2차 설치 가능 확인
                is_correct_install = path_checker.Path_Finding();

                if (is_correct_install) {
                    installer.Confirm_Install_Tower(tower_base);
                    installer.Install_Tower(unit_id, instance_translucent_obj.transform.position);
                    // 카드 목록에서 제외하기
                    Card_Observer.Instance.Card_Hider(unit_id, false);
                }
                else {
                    // 원복
                    installer.UnInstall_Tower(tower_base);
                }
            }

            // 교환
            if (can_shift_tower) {
                Shift.Instance.Shift_tower(unit_id, instance_translucent_obj.transform.position);
            }

            Destroy(instance_translucent_obj);
            instance_attack_area = null;
            Area_Pool.Instance.Return();
            can_install_tower = false;

            // pannel activate
            Pannel_Controller.Instance.Deactivate_for_Install();
        }

        install_active = false;
    }

    IEnumerator DownChecker() {
        while ( hold_time <= 1.0f)
        {
            hold_time += Time.deltaTime / check_time;
            yield return null;
        }
        install_active = true;

        // all pannel down
        Pannel_Controller.Instance.Activate_for_Install();
        instance_translucent_obj = Instantiate(translucent_obj);
        instance_translucent_obj.transform.Find("Ori").GetComponent<SpriteRenderer>().sprite = Utility.Get_sprite(unit_id);
        click_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        click_pos.z = 1;
        click_pos.y += .8f;
        instance_translucent_obj.transform.position = click_pos;

        // 공격 범위 보이기
        instance_attack_area = Area_Pool.Instance.Activate(unit_id, click_pos);
    }
}
