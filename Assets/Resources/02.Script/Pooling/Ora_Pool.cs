using static CUSTOM_DATA.Game_Value_Data;
using static CUSTOM_DATA.Common_Data;
using static CUSTOM_DATA.Game_Data;
using UnityEngine;
using CUSTOM_DATA;
using System.Collections.Generic;

public class Ora_Pool : Scene_Singleton<Ora_Pool>
{
    private readonly Dictionary<OraType, Queue<GameObject>> ora_queue = new();
    private readonly Dictionary<OraType, Sprite> ora_sprite = new();
    private readonly Dictionary<OraType, Color> ora_color = new();

    void Start() {
        ora_sprite.Clear();
        ora_sprite[OraType.Ice] = ice_ora;
        ora_sprite[OraType.Fire] = fire_ora;
        ora_sprite[OraType.Poison] = poison_ora;
        ora_sprite[OraType.Water] = water_ora;

        ora_queue[OraType.Ice] = new();
        ora_queue[OraType.Fire] = new();
        ora_queue[OraType.Poison] = new();
        ora_queue[OraType.Water] = new();

        ora_color[OraType.Ice] = ice_color;
        ora_color[OraType.Fire] = fire_color;
        ora_color[OraType.Poison] = poison_color;
        ora_color[OraType.Water] = water_color;

        Initialize();
    }

    private void Initialize() {
        for (int i = 0; i < 5; i++) {
            Create_Ora(OraType.Ice);
            Create_Ora(OraType.Fire);
            Create_Ora(OraType.Poison);
            Create_Ora(OraType.Water);
        }
    }

    private void Create_Ora(OraType type) {
        GameObject instance = Instantiate(ora_prefab, transform);
        instance.GetComponent<SpriteRenderer>().sprite = ora_sprite[type];
        instance.GetComponent<SpriteRenderer>().color = ora_color[type];
        instance.SetActive(false);
        ora_queue[type].Enqueue(instance);
    }

    public GameObject Get_Ora(OraType type) {
        if (ora_queue[type].Count < 1) {
            Create_Ora(type);
        }

        GameObject result = ora_queue[type].Dequeue();
        result.SetActive(true);
        return result;
    }

    public void Return(OraType type, GameObject obj) {
        obj.transform.SetParent(transform);
        obj.SetActive(false);
        ora_queue[type].Enqueue(obj);
    }
}
