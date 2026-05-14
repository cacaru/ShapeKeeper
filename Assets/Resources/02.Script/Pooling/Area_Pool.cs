using static CUSTOM_DATA.Game_Value_Data;
using static CUSTOM_DATA.Game_Data;
using UnityEngine;
using CUSTOM_DATA;

public class Area_Pool : Scene_Singleton<Area_Pool> {
    private GameObject area;

    private Vector3 d_scale = new(2, 2, 2);
    private Vector3 c_scale = new(3f, 3f, 3f);
    private Vector3 b_scale = new(4, 4, 4);
    private Vector3 a_scale = new(5f, 5f, 5f);
    private Vector3 s_scale = new(6, 6, 6);
    private Vector3 ex_scale = new(7f, 7f, 7f);

    void Start() {
        area = Instantiate(attack_area_prefab, transform);
        area.transform.SetParent(transform, false);
        area.name = "area";
        area.SetActive(false);
    }

    public GameObject Activate(int unit_id, Vector3 pos) {
        // 등급에 따라 크기가 달라져야함
        var color = Utility.Get_Grade_Color(unit_id);
        color.a = 40 / 255f;
        area.GetComponent<SpriteRenderer>().color = color;
        area.transform.position = pos;
        var scaler = unit[unit_id].grade switch {
            "d" => d_scale,
            "c" => c_scale,
            "b" => b_scale,
            "a" => a_scale,
            "s" => s_scale,
            "ex" => ex_scale,
            _ => d_scale
        };
        area.transform.localScale = scaler;

        area.SetActive(true);
        return area;
    }

    public void Return() {
        area.SetActive(false);
        area.transform.localScale = d_scale;
    }
}
