using UnityEngine;
using static CUSTOM_DATA.Game_Value_Data;
using static CUSTOM_DATA.Common_Data;

public class Goal_Spawner : MonoBehaviour
{
    /// <summary>
    /// 적 유닛이 도달할 목표를 랜덤하게 생성하는 스크립트
    /// </summary>

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 위치를 잡고
        // 해당 tile을 79번으로 변경하고
        // tile 위치는 load_map y:-11 x : -6~5
        // 그 위에 목표 표시를 해둔다 표시는 statue 오브젝트를 생성하는것으로 한다
        int x = Random.Range(-6, 6);
        float ran_x = (x+6) * 0.4f + -2.2f;

        //statue
        GameObject instance = Instantiate(goal_prefab);
        instance.transform.position = load_map.GetCellCenterWorld(load_map.WorldToCell(new(ran_x, -4f, 0)));

        //tile
        load_map.SetTile(new(x, -11, 0), base_camp_tile);

        load_map.GetTile(new(x, -11, 0));
    }

}
