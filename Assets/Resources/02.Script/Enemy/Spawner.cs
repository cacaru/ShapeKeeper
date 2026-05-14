using System.Collections.Generic;
using UnityEngine;
using static CUSTOM_DATA.Game_State_Data;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Game_Value_Data;
using static CUSTOM_DATA.Common_Data;
using System.Collections;
using UnityEngine.UI;

public class Spawner : Scene_Singleton<Spawner> {
    [SerializeField] private Slider round_time_slider;

    private readonly List<GameObject> enemies = new();

    public int enemy_count = 0;

    private float default_speed = 0.0035f;
    public float Get_Speed() {
        return default_speed;
    }

    private Coroutine checker = null;
    private bool is_spawning = false;

    // 시작 버튼이 눌린 이후부터 
    // 1초마다 1마리씩 총 40마리 
    // 1라운드 60초
    // 10라운드마다 보스 생성
    // 일단 100라운드
    public void Start_Spawn() {
        is_gaming = true;
        StartCoroutine(Skip_Waiting());
        checker = StartCoroutine(Lound_process());
    }

    public void End_Spawn() {
        is_gaming = false;
        StopCoroutine(checker);
    }

    public void Skip_Round() {
        if (checker == null) {
            return;
        }
        
        // 기존 코루틴을 종료하고
        StopCoroutine(checker);
        // 새로 코루틴을 실행
        checker = StartCoroutine(Lound_process());
        // 업적 체크
        Achievement_Observer.Instance.Skip_Achievement();

        // 재화 추가
        int end_value = round_mob_counter - enemy_count + Round_counter + 10;
        if (end_value < 0) end_value = Round_counter + 10;
        float value = Random.Range(Round_counter, end_value);
        Set_dot(dot + value);

        // 5초동안 off
        StartCoroutine(Skip_Waiting());
    }

    IEnumerator Skip_Waiting() {
        Btn_Controller.Instance.Active_Skip_Btn();
        yield return wfs_5;
        Btn_Controller.Instance.Deactive_Skip_Btn();
    }

    IEnumerator Lound_process() {
        //Round_counter++;
        int timing_checker = 50;
        is_spawning = false;
        round_time_slider.value = timing_checker;
        while (true) {

            if(is_gaming) {
                // round - end check
                if (Round_counter == ROUND_MAX) {
                    // 모든 몬스터가 없어지면 게임 완료 판정
                    StartCoroutine(Ending_watcher());
                    break;
                }

                if(!is_spawning && timing_checker == 50) {
                    is_spawning = true;
                    // 업적 확인
                    Achievement_Observer.Instance.Clear_Round();

                    // 라운드 시작 이후 5초간 스킵 불가
                    StartCoroutine(Skip_Waiting());
                    StartCoroutine(One_Round_Spawn());
                }
                timing_checker--;
                round_time_slider.value = timing_checker;
            }

            if (timing_checker == 0) timing_checker = 50;

            yield return (game_speed <= 1) ? wfs_2 : wfs_1;
        }
    }

    IEnumerator One_Round_Spawn() {
        Round_counter++;
        bool boss_spawn = false;
        int i = 0;

        if (Round_counter % 10 == 0 && Round_counter > 0) boss_spawn = true;
        
        while(i < round_mob_counter) {

            if (is_gaming) {
                // boss spawn
                if (boss_spawn) {
                    // 한마리만 소환하고 말아야함
                    boss_spawn = false;
                    Spawn_Boss();
                }
                else {
                    Spawn_Normal();
                }

                enemy_count++;   
                i++;
            }

            yield return (game_speed <= 1) ? wfs_2 : wfs_1;
        }
        is_spawning = false;
    }

    IEnumerator Ending_watcher() {
        // 소환 대기
        yield return wfs_2;

        while (true) {
            if(GameObject.FindGameObjectsWithTag("Enemy").Length <= 0) {
                break;
            }
            yield return wfs_2;
        }

        Game_End.Instance.Ending(true);
    }

    private int last_boss_spawn_lound = -1;

    private void Spawn_Boss() {
        if (Round_counter == last_boss_spawn_lound || Round_counter % 10 != 0) return;
        last_boss_spawn_lound = Round_counter;
        //Debug.Log($"[Boss Spawn] Round: {Round_counter}");
        // 최상단 y == 5 고정
        // x == -2.2 ~ 2.2 단위 0.4
        float ran_x = Random.Range(0, 12) * 0.4f + -2.2f;

        int random_val = Random.Range(1, 4);
        var instance = Enemy_Pool.Instance.Get_Boss(random_val);
        //instance.transform.SetParent(transform);
        instance.transform.position = new Vector3(ran_x, 5.4f, 0);
        random_val += 100;

        var now_enemy = enemy[random_val];
        // speed setting
        instance.GetComponent<EnemyMover>().move_speed = default_speed * now_enemy.speed;
        instance.GetComponent<EnemyMover>().Start_Mapping();

        //현재라운드 * (1 + 0.1 * 난이도) * 체력증가율 * 오라변수
        float material_hp = Round_counter * (float)(1 + 0.1 * difficulty) * now_enemy.material_hp;
        float ethereal_hp = Round_counter * (float)(1 + 0.1 * difficulty) * now_enemy.ethereal_hp;
        
        instance.GetComponent<EnemyHP>().Init(material_hp, ethereal_hp, Round_counter);
        instance.GetComponent<EnemyHP>().Enmey_id = (Round_counter - 1) * 60 + enemy_count;
        enemies.Add(instance);
    }

    private void Spawn_Normal() {
        // 최상단 y == 5 고정
        // x == -2.2 ~ 2.2 단위 0.4
        float ran_x = Random.Range(0, 12) * 0.4f + -2.2f;

        int random_val = Random.Range(1, 4);
        var instance = Enemy_Pool.Instance.Get_Enemy(random_val);
        //instance.transform.SetParent(transform);
        instance.transform.position = new Vector3(ran_x, 5.5f, 0);

        var now_enemy = enemy[random_val];
        // speed setting
        instance.GetComponent<EnemyMover>().move_speed = default_speed * now_enemy.speed;
        instance.GetComponent<EnemyMover>().Start_Mapping();
        // hp setting
        //현재라운드 * (1 + 0.1 * 난이도) * 체력증가율 * 오라변수
        float material_hp = Round_counter * (1 + 0.1f * difficulty) * now_enemy.material_hp;
        float ethereal_hp = Round_counter * (1 + 0.1f * difficulty) * now_enemy.ethereal_hp;
        instance.GetComponent<EnemyHP>().Init(material_hp, ethereal_hp, Round_counter);
        instance.GetComponent<EnemyHP>().Enmey_id = (Round_counter - 1) * 60 + enemy_count;
        enemies.Add(instance);
    }

    public void Remapping() {
        if(enemies.Count > 0) {
            for (int i = 0; i < enemies.Count; i++) {
                //Debug.Log(enemies[i].name);
                enemies[i].GetComponent<EnemyMover>().Remapping();
            }
        }
    }

    public void Kill_Enemy(GameObject enemy) {
        enemies.Remove(enemy);
    }

    public void Spawn_Mission_Boss(int number) {
        float ran_x = Random.Range(0, 12) * 0.4f + -2.2f;

        var instance = Enemy_Pool.Instance.Get_Mission_Boss(number);
        instance.transform.position = new Vector3(ran_x, 5.4f, 0);

        instance.GetComponent<EnemyMover>().Start_Mapping();
        instance.GetComponent<EnemyHP>().Enmey_id = (Round_counter - 1) * 60 + 100000;
        enemies.Add(instance);
    }
}
