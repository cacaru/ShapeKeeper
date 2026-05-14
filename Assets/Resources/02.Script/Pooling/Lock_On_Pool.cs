using static CUSTOM_DATA.Game_Value_Data;
using static CUSTOM_DATA.Common_Data;
using static CUSTOM_DATA.Game_Data;
using UnityEngine;
using CUSTOM_DATA;

public class Lock_On_Pool : Scene_Singleton<Lock_On_Pool> {

    private GameObject area;

    void Start() {
        area = Instantiate(lock_on_prefab, transform);
        area.transform.SetParent(transform, false);
        area.name = "lock_on";
        area.SetActive(false);
    }

    public GameObject Activate(OraType type, Vector3 pos) {
        // 타입에 따라 색을 변경
        // 빙결 / 불 / 독 / 물
        var color = type switch {
            OraType.Ice => ice_color,
            OraType.Fire => fire_color,
            OraType.Poison => poison_color,
            OraType.Water => water_color,
            _ => Color.white
        };

        area.GetComponent<SpriteRenderer>().color = color;
        area.transform.position = pos;

        area.SetActive(true);
        return area;
    }

    public void Return() {
        area.GetComponent<SpriteRenderer>().color = Color.white;
        area.SetActive(false);
    }
}
