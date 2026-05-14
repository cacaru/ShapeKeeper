using static CUSTOM_DATA.Game_Value_Data;
using UnityEngine;
using CUSTOM_DATA;
using System.Collections.Generic;

public class Effect_Pooling : Scene_Singleton<Effect_Pooling>
{
    private readonly Dictionary<OraType, Queue<GameObject>> effect_queue = new();

    private readonly string effect_name = "effect";
    void Start() {

        effect_queue[OraType.Ice] = new();
        effect_queue[OraType.Fire] = new();
        effect_queue[OraType.Poison] = new();
        effect_queue[OraType.Water] = new();

        Initialize();
    }

    private void Initialize() {
        for (int i = 0; i < 20; i++) {
            Create_Effect(OraType.Ice);
            Create_Effect(OraType.Fire);
            Create_Effect(OraType.Poison);
            Create_Effect(OraType.Water);
        }
    }

    private void Create_Effect(OraType type) {
        var effect_obj = type switch {
            OraType.Ice => ice_effect,
            OraType.Fire => fire_effect,
            OraType.Poison => poison_effect,
            OraType.Water => water_effect,
            _ => null
        };
        if(effect_obj == null) { return; }
        GameObject instance = Instantiate(effect_obj, transform);
        instance.name = effect_name + type.ToString();
        instance.SetActive(false);
        effect_queue[type].Enqueue(instance);
    }

    public GameObject Get(OraType type) {
        if (effect_queue[type].Count < 1) {
            Create_Effect(type);
        }

        GameObject result = effect_queue[type].Dequeue();
        result.SetActive(true);
        return result;
    }

    public void Return(OraType type, GameObject obj) {
        obj.transform.SetParent(transform);
        obj.transform.position = Vector3.zero;
        obj.SetActive(false);
        effect_queue[type].Enqueue(obj);
    }
}
