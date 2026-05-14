using CUSTOM_DATA;
using static CUSTOM_DATA.Game_Value_Data;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Card_Pool : Scene_Singleton<Unit_Card_Pool>
{
    private readonly Queue<GameObject> pool = new();
    private readonly string card_name = "unit";

    void Start() {
        Initialize();
    }

    private void Initialize() {
        // À¯´Ö¼ö
        //int size = Game_Data.unit.Count;
        int size = 62;
        for (int i = 0; i < size; i++) {
            Create_Card();
        }
    }

    private void Create_Card() {
        var function = Instantiate(unit_card_prefab, transform);
        function.name = card_name;
        function.SetActive(false);
        pool.Enqueue(function);
    }


    public GameObject Get_Card() {
        if (pool.Count <= 0) Create_Card();
        var result = pool.Dequeue();
        result.SetActive(true);
        return result;
    }

    public void Return_Card(GameObject return_obj) {
        return_obj.transform.SetParent(transform);
        return_obj.SetActive(false);

        pool.Enqueue(return_obj);
    }
}
