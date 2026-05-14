using UnityEngine;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Game_Value_Data;

public class Unit_Page_Book_Setter : Scene_Singleton<Unit_Page_Book_Setter>
{
    [SerializeField] private GameObject D;
    [SerializeField] private GameObject C;
    [SerializeField] private GameObject B;
    [SerializeField] private GameObject A;
    [SerializeField] private GameObject S;
    [SerializeField] private GameObject EX;

    public void Init() {
        // 모든 유닛의 조각과 현재 소지 골드를 비교해서 강화 가능하다면 unit의 back을 켜기
        foreach(Transform item in D.transform) {
            if(item != D.transform) {
                Setting(int.Parse(item.name), item);
            }
        }
        foreach (Transform item in C.transform) {
            if (item != C.transform) {
                Setting(int.Parse(item.name), item);
            }
        }
        foreach (Transform item in B.transform) {
            if (item != B.transform) {
                Setting(int.Parse(item.name), item);
            }
        }
        foreach (Transform item in A.transform) {
            if (item != A.transform) {
                Setting(int.Parse(item.name), item);
            }
        }
        foreach (Transform item in S.transform) {
            if (item != S.transform) {
                Setting(int.Parse(item.name), item);
            }
        }
        foreach (Transform item in EX.transform) {
            if (item != EX.transform) {
                Setting(int.Parse(item.name), item);
            }
        }

    }


    private void Setting(int unit_id, Transform target) {
        var _unit = unit[unit_id];

        int upgrade_cost_figure = _unit.grade switch {
            "d" => D_UPGRADE_GOLD_COEFFICENT,
            "c" => C_UPGRADE_GOLD_COEFFICENT,
            "b" => B_UPGRADE_GOLD_COEFFICENT,
            "a" => A_UPGRADE_GOLD_COEFFICENT,
            "s" => S_UPGRADE_GOLD_COEFFICENT,
            "ex" => EX_UPGRADE_GOLD_COEFFICENT,
            _ => 0
        };

        int upgrade_piece_figure = _unit.grade switch {
            "d" => D_UPGRADE_PIECE_COEFFICENT,
            "c" => C_UPGRADE_PIECE_COEFFICENT,
            "b" => B_UPGRADE_PIECE_COEFFICENT,
            "a" => A_UPGRADE_PIECE_COEFFICENT,
            "s" => S_UPGRADE_PIECE_COEFFICENT,
            "ex" => EX_UPGRADE_PIECE_COEFFICENT,
            _ => 0
        };

        int need_gold = upgrade_cost_figure * (_unit.upgrade + 1);
        int need_piece = upgrade_piece_figure * (_unit.upgrade + 1);

        target.Find("Back").GetComponent<Animator>().SetBool(ANI_ACTIVATE, (user.gold >= need_gold && _unit.piece >= need_piece ));

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

}
