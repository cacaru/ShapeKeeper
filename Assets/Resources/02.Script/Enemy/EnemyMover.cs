using UnityEngine;
using CUSTOM_DATA;
using static CUSTOM_DATA.Game_State_Data;
using static CUSTOM_DATA.Common_Data;
using System.Collections;


public class EnemyMover : MonoBehaviour
{
    private bool moving = false;
    public float move_speed;
    public float ori_speed;
    private readonly EnemyPathChecker enemy_path = new();
    //private readonly Enemy_Path_Checker enemy_path = new();
    // 타일맵이 기록되는 순서는 좌하단 부터...!
    // 타일에 타워를 배치하게 되면
    // 배치된 타워의 위치에 monster_load의 상응하는 위치에 동일한 크기의 다른 tile로 교체하여
    // 길을 계산할 때 피해갈 수 있어야 한다


    /// <summary>
    ///  1. 시작 or 타워가 설치될 때마다 맵을 공용변수에 저장한다
    ///  2. 저장된 그리드를 따라 현재 위치에서 길을 찾는다
    ///  3. 찾은 길을 따라 이동한다.
    ///  
    ///  ! 맵 공용변수는 x , y 의 크기를 저장해두고 사용한다
    ///  !  -> x y의 크기를 정해둘 때는 tilemap 기준의 최소 최대값을 저장해둔다.
    ///  ! 맵 공용변수의 형식은 변수명[y,x]이다 (행, 열)
    /// </summary>    
    /// 

    private void Start() {
        Start_Mapping();
    }

    public void Remapping() {
        enemy_path.Re_Path_Finding(transform.position);
    }

    public void Start_Mapping() {
        enemy_path.Set_start_position(transform.position);
        enemy_path.Path_Finding();
        moving = true;
    }
    
    void Update() {
        if (enemy_path.Final_way.Count == 0 && !enemy_path.Check_End(transform.position)) {
            enemy_path.Set_start_position(transform.position);
            enemy_path.Path_Finding();
        }

        if (moving && is_gaming) {
            Vector2 target;
            // target이 있는 곳을 탐색 하는데 
            // 타겟이 없으면(final_way가 없으면 무작정 위로 올라가 다시 pathfinding을 진행
            if (enemy_path.Final_way.Count <= 0) {
                target.x = transform.position.x; target.y = transform.position.y + 0.4f;
                enemy_path.Set_start_position(transform.position);
                enemy_path.Path_Finding();
            }
            else {
                target = load_map.CellToWorld(new((int)enemy_path.Final_way[0].grid_pos.x, (int)enemy_path.Final_way[0].grid_pos.y));
            }

            target.x += .2f;
            target.y += .2f;

            transform.position = Vector2.MoveTowards(transform.position, target, move_speed * game_speed);

            if (transform.position.x == target.x && transform.position.y == target.y) {
                enemy_path.Final_way.RemoveAt(0);
            }

            if (enemy_path.Final_way.Count == 0 && enemy_path.Check_End(transform.position)) {
                // 제거를 spawner에 알려야함
                // 맵마다 바뀌는 스포너를 고려해서 작성해야함
                Spawner.Instance.Kill_Enemy(gameObject);
                Enemy_Pool.Instance.Return_Enemy(gameObject);

                // 그리고 게임 종료(패배)
                Game_End.Instance.Ending(false);
            }
        }
    }

    public void Ice_Active(int level) {
        StartCoroutine(Freeze(level));
    }

    IEnumerator Freeze(int level) {
        moving = false;
        yield return level switch { 
            1 => wfs_0_1,
            2 => wfs_0_2,
            3 => wfs_0_3,
            4 => wfs_0_4,
            _ => null
        };
        moving = true;
    }

    private bool is_slowed = false;
    private int timer = 0;
    public void Water_Active(int level) {
        if (is_slowed) {
            timer = 0;
        }
        else {
            is_slowed = true;
            StartCoroutine(Slow(level));
        }
    }

    IEnumerator Slow(int level) {
        ori_speed = move_speed;
        move_speed /= 5f;
        while(timer < level) {
            timer++;
            yield return wfs_1;
        }
        move_speed = ori_speed;
        timer = 0;
        is_slowed = false;
    }

    private void OnDrawGizmos() {
        if (enemy_path.Final_way.Count != 0) {
            for (int i = 0; i < enemy_path.Final_way.Count - 1; i++) {
                Gizmos.color = Color.red;
                var first = load_map.CellToWorld(new((int)enemy_path.Final_way[i].grid_pos.x, (int)enemy_path.Final_way[i].grid_pos.y));
                var second = load_map.CellToWorld(new((int)enemy_path.Final_way[i+1].grid_pos.x, (int)enemy_path.Final_way[i+1].grid_pos.y));
                //Debug.Log(first.x + " , " + first.y);
                first.x += .2f;
                first.y += .2f;
                second.x += .2f;
                second.y += .2f;
                Gizmos.DrawLine(first,second);
            }
        }
    }
}
