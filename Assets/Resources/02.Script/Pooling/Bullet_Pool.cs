using static CUSTOM_DATA.Game_Value_Data;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Pool : Scene_Singleton<Bullet_Pool> {

    private readonly Queue<GameObject> pool = new();
    private readonly string BULLET = "bullet";
    private readonly Vector3 scale = new(.25f, .25f, .25f);

    void Start() {
        Initialize();
    }

    private void Initialize() {
        // 유닛수
        //int size = Game_Data.unit.Count;
        int size = 100;
        for (int i = 0; i < size; i++) {
            Create();
        }
    }

    private void Create() {
        var function = Instantiate(bullet_prefab, transform);
        function.GetComponent<Transform>().localScale = scale;
        function.name = BULLET;
        function.SetActive(false);
        pool.Enqueue(function);
    }


    public GameObject Get() {
        if (pool.Count <= 0) Create();
        var result = pool.Dequeue();
        result.SetActive(true);
        return result;
    }

    public void Return(GameObject return_obj) {
        return_obj.transform.SetParent(transform, false);
        // 현 포지션 초기화
        return_obj.transform.position = Vector3.zero;
        return_obj.SetActive(false);

        pool.Enqueue(return_obj);
    }
}
