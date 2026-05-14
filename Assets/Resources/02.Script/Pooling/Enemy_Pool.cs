using CUSTOM_DATA;
using static CUSTOM_DATA.Game_Value_Data;
using static CUSTOM_DATA.Game_State_Data;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Pool : Scene_Singleton<Enemy_Pool> {
    private Vector3 ori_pos = new(0, 5.4f, 0);

    private readonly Queue<GameObject> ori_pool = new();
    private readonly Queue<GameObject> shield_pool = new();
    private readonly Queue<GameObject> bust_pool = new();

    private GameObject ori_boss;
    private GameObject shield_boss;
    private GameObject bust_boss;

    private GameObject mission_boss_obj_1;
    private GameObject mission_boss_obj_2;
    private GameObject mission_boss_obj_3;
    private GameObject mission_boss_obj_4;
    private GameObject mission_boss_obj_5;

    private readonly string NAME = "enemy";
    private readonly string BOSS_NAME = "boss";
    private readonly string MISSION = "_mission";
    private readonly string ORI = "_ori";
    private readonly string SHIELD = "_shield";
    private readonly string BUST = "_bust";

    private int enemy_layer;

    void Start() {
        // 행성 따라 달라야함
        Initialize();
        enemy_layer = LayerMask.NameToLayer("Enemy");
    }

    private void Initialize() {
        int size = 40;
        for (int i = 0; i < size; i++) {
            Create_ori();
            Create_shield();
            Create_bust();
        }

        Create_Boss_ori();
        Create_Boss_shield();
        Create_Boss_bust();

        Create_Mission();
    }

    // mission boss create
    private void Create_Mission() {
        mission_boss_obj_1 = Instantiate(mission_boss_1, transform);
        mission_boss_obj_2 = Instantiate(mission_boss_2, transform);
        mission_boss_obj_3 = Instantiate(mission_boss_3, transform);
        mission_boss_obj_4 = Instantiate(mission_boss_4, transform);
        mission_boss_obj_5 = Instantiate(mission_boss_5, transform);

        mission_boss_obj_1.name = BOSS_NAME + MISSION + "_1";
        mission_boss_obj_2.name = BOSS_NAME + MISSION + "_2";
        mission_boss_obj_3.name = BOSS_NAME + MISSION + "_3";
        mission_boss_obj_4.name = BOSS_NAME + MISSION + "_4";
        mission_boss_obj_5.name = BOSS_NAME + MISSION + "_5";

        // hp setting 
        // hp / shield 비율은 50  50 
        int hp = 1 * 5000 + ((difficulty - 1) * 500 * 1);
        mission_boss_obj_1.GetComponent<EnemyHP>().Init(hp, hp, 1);

        hp = 2 * 5000 + ((difficulty - 1) * 500 * 2);
        mission_boss_obj_2.GetComponent<EnemyHP>().Init(hp, hp, 2);

        hp = 3 * 5000 + ((difficulty - 1) * 500 * 3);
        mission_boss_obj_3.GetComponent<EnemyHP>().Init(hp, hp, 3);

        hp = 4 * 5000 + ((difficulty - 1) * 500 * 4);
        mission_boss_obj_4.GetComponent<EnemyHP>().Init(hp, hp, 4);        

        hp = 5 * 5000 + ((difficulty - 1) * 500 * 5);
        mission_boss_obj_5.GetComponent<EnemyHP>().Init(hp, hp, 5);

        // speed setting
        float default_speed = Spawner.Instance.Get_Speed();
        mission_boss_obj_1.GetComponent<EnemyMover>().move_speed = default_speed * 3;
        mission_boss_obj_2.GetComponent<EnemyMover>().move_speed = default_speed * 3;
        mission_boss_obj_3.GetComponent<EnemyMover>().move_speed = default_speed * 3;
        mission_boss_obj_4.GetComponent<EnemyMover>().move_speed = default_speed * 3;
        mission_boss_obj_5.GetComponent<EnemyMover>().move_speed = default_speed * 3;

        mission_boss_obj_1.SetActive(false);
        mission_boss_obj_2.SetActive(false);
        mission_boss_obj_3.SetActive(false);
        mission_boss_obj_4.SetActive(false);
        mission_boss_obj_5.SetActive(false);
    }


    // normal monster pooling
    private void Create_ori() {
        var prefab = Now_plannet switch {
            Plannet.Green => slime_prefab_ori,
            Plannet.Blue => wisp_prefab_ori,
            Plannet.Gray => core_prefab_ori,
            Plannet.Ancient => ancient_core_prefab_ori,
            _ => slime_prefab_ori
        };
        var function = Instantiate(prefab, transform);
        function.name = NAME + ORI;
        function.SetActive(false);
        ori_pool.Enqueue(function);
    }
    private void Create_shield() {
        var prefab = Now_plannet switch {
            Plannet.Green => slime_prefab_shield,
            Plannet.Blue => wisp_prefab_shield,
            Plannet.Gray => core_prefab_shield,
            Plannet.Ancient => ancient_core_prefab_shield,
            _ => slime_prefab_shield,
        };
        var function = Instantiate(prefab, transform);
        function.name = NAME + SHIELD;
        function.SetActive(false);
        shield_pool.Enqueue(function);
    }
    private void Create_bust() {

        var prefab = Now_plannet switch {
            Plannet.Green => slime_prefab_bust,
            Plannet.Blue => wisp_prefab_bust,
            Plannet.Gray => core_prefab_bust,
            Plannet.Ancient => ancient_core_prefab_bust,
            _ => slime_prefab_bust,
        };
        var function = Instantiate(prefab, transform);
        function.name = NAME + BUST;
        function.SetActive(false);
        bust_pool.Enqueue(function);
    }

    // boss monster pooling
    private void Create_Boss_ori() {
        var prefab = Now_plannet switch {
            Plannet.Green => slime_prefab_boss_ori,
            Plannet.Blue => wisp_prefab_boss_ori,
            Plannet.Gray => core_prefab_boss_ori,
            Plannet.Ancient => ancient_core_prefab_boss_ori,
            _ => slime_prefab_boss_ori
        };
        ori_boss = Instantiate(prefab, transform);
        ori_boss.name = BOSS_NAME + ORI;
        ori_boss.SetActive(false);
    }
    private void Create_Boss_shield() {
        var prefab = Now_plannet switch {
            Plannet.Green => slime_prefab_boss_shield,
            Plannet.Blue => wisp_prefab_boss_shield,
            Plannet.Gray => core_prefab_boss_shield,
            Plannet.Ancient => ancient_core_prefab_boss_shield,
            _ => slime_prefab_boss_shield,
        };
        shield_boss = Instantiate(prefab, transform);
        shield_boss.name = BOSS_NAME + SHIELD;
        shield_boss.SetActive(false);
    }
    private void Create_Boss_bust() {
        var prefab = Now_plannet switch {
            Plannet.Green => slime_prefab_boss_bust,
            Plannet.Blue => wisp_prefab_boss_bust,
            Plannet.Gray => core_prefab_boss_bust,
            Plannet.Ancient => ancient_core_prefab_boss_bust,
            _ => slime_prefab_boss_bust,
        };
        bust_boss = Instantiate(prefab, transform);
        bust_boss.name = BOSS_NAME + BUST;
        bust_boss.SetActive(false);
    }

    public GameObject Get_Enemy(int type = 0) {
        GameObject result = null;
        if (type > 3 || type < 1) type = Random.Range(1, 4);

        switch (type) {
            case 1:
                if (ori_pool.Count <= 0) Create_ori();
                result = ori_pool.Dequeue();
                break;
            case 2:
                if (shield_pool.Count <= 0) Create_shield();
                result = shield_pool.Dequeue();
                break;
            case 3:
                if (bust_pool.Count <= 0) Create_bust();
                result = bust_pool.Dequeue();
                break;
        }
        
        result.SetActive(true);
        return result;
    }

    public GameObject Get_Boss(int type = 0) {

        if (type > 3 || type < 1) type = Random.Range(1, 4);

        GameObject result = type switch {
            1 => ori_boss,
            2 => shield_boss,
            3 => bust_boss,
            _ => null
        };

        result.SetActive(true);
        return result;
    }

    public GameObject Get_Mission_Boss(int number) {
        var return_value = number switch {
            1 => mission_boss_obj_1,
            2 => mission_boss_obj_2,
            3 => mission_boss_obj_3,
            4 => mission_boss_obj_4,
            5 => mission_boss_obj_5,
            _ => null
        };
        if (return_value != null) return_value.SetActive(true);
        return return_value;
    }

    public void Return_Enemy(GameObject return_obj) {
        return_obj.transform.SetParent(transform);
        return_obj.transform.position = ori_pos;
        return_obj.SetActive(false);
        return_obj.layer = enemy_layer;
        return_obj.GetComponent<EnemyMover>().enabled = true;
        return_obj.GetComponent<EnemyHP>().alive = true;
        // hp shield init
        return_obj.GetComponent<EnemyHP>().Set_hp(100);
        return_obj.GetComponent<EnemyHP>().Set_Shield(100);
        return_obj.GetComponent<EnemyHP>().Init_Pos();
        return_obj.GetComponent<Animator>().SetBool("Die", false);

        // effect 가 있으면 지우기
        
        foreach(Transform effect in return_obj.transform) {
            if (effect.name.Contains("effect")) {
                Destroy(effect.gameObject);
            }
        }
        
       if (return_obj.name.Contains(BOSS_NAME)) {
            // default -- nothing
            // mission -> hp reset
            if (return_obj.name.Contains(MISSION)) {
                int number = int.Parse(return_obj.name.Split("_")[2]);
                int hp = number * 5000 + ((difficulty - 1) * 500 * number);
                return_obj.GetComponent<EnemyHP>().Set_hp(hp);
                return_obj.GetComponent<EnemyHP>().Set_Shield(hp);
            }
        }
        else if (return_obj.name.Contains(ORI)) {
            ori_pool.Enqueue(return_obj);
        }
        else if (return_obj.name.Contains(SHIELD)) {
            shield_pool.Enqueue(return_obj);
        }
        else if (return_obj.name.Contains(BUST)) {
            bust_pool.Enqueue(return_obj);
        }
        else {
            // 가질 수 없으면 죽이기
            Destroy(return_obj);
        }
    }
}
