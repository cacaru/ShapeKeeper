using CUSTOM_DATA;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static CUSTOM_DATA.Common_Data;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Game_State_Data;

public class EnemyHP : MonoBehaviour {
    [SerializeField]
    private GameObject material_bar;
    [SerializeField]
    private GameObject ethereal_bar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Vector3 reposing_y = new(0, -.18f, 0);

    public int Enmey_id { set; get; }
    public bool alive = true;

    public float material = 100;
    public float max_material = 100;

    public float ethereal = 100;
    public float max_ethereal = 100;
    private int spawn_round = -1;
    private bool poisoning = false;
    public void Init(float material, float ethereal, int spawn_round) {
        this.spawn_round = spawn_round;

        this.material = material;
        max_material = material;

        this.ethereal = ethereal;
        max_ethereal = ethereal;
        Set_hp(material);
        Set_Shield(ethereal);
    }

    public void Init_Pos() {
        material_bar.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + reposing_y);
        ethereal_bar.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + reposing_y);
    }

    public void Set_hp(float t_material) {
        material = t_material;

        if (material <= 0) {
            material_bar.GetComponent<Slider>().value = 0;
            Die();
            return;
        }

        float value = material / max_material;
        material_bar.GetComponent<Slider>().value = value;
    }

    public void Set_Shield(float t_ethereal) {
        ethereal = t_ethereal;

        if (ethereal <= 0) {
            ethereal_bar.GetComponent<Slider>().value = 0;
            Die();
            return;
        }

        float value = ethereal / max_ethereal;
        ethereal_bar.GetComponent<Slider>().value = value;
    }

    void Start() {
        material_bar.GetComponent<Slider>().interactable = false;
        ethereal_bar.GetComponent<Slider>().interactable = false;
    }

    // Update is called once per frame
    void FixedUpdate() {
        material_bar.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + reposing_y);
        ethereal_bar.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + reposing_y);
    }

    // damage 받기
    public void Get_Damage(Damage damage) {
        // type이 none 이면 일반 공격
        // OraType.Ice = freeze(마비)
        // Oratype.Fire = 폭발형
        // OraType.Poison = 독데미지
        // OraType.Water = 슬로우
        switch (damage.type) {
            case OraType.Ice:
                // 빙결 이펙트 켜기
                // 빙결 효과 켜기
                GetComponent<EnemyMover>().Ice_Active(stored_spell[OraType.Ice].level);

                break;
            case OraType.Fire:
                
                // 폭발 효과 켜기
                Fire_Active(damage);
                break;
            case OraType.Poison:
                // 독 이펙트 켜기
                // 독 효과 켜기
                Poison_Active(damage);
                break;
            case OraType.Water:
                // 둔화 이펙트 켜기
                // 둔화 효과 켜기
                GetComponent<EnemyMover>().Water_Active(stored_spell[OraType.Water].level);
                break;
        }
        // 이펙트 켜기
        On_Effect(damage.type);

        // ethereal이 남아있으면 ethereal만 깎고 넘기기
        // 0 이면 material 을 깎고 넘기기
        if (ethereal > 0) {
            Set_Shield(ethereal - damage.ethereal);
            return;
        }
        Set_hp(material - damage.material);
    }

    private void On_Effect(OraType type) {
        if (type == OraType.None) return;

        var effect_obj = Effect_Pooling.Instance.Get(type);
        effect_obj.transform.SetParent(transform, false);
        StartCoroutine(Effect_On(effect_obj, type));
    }

    IEnumerator Effect_On(GameObject obj, OraType type) {
        yield return wfs_1;
        Effect_Pooling.Instance.Return(type, obj);
    }


    private void Fire_Active(Damage damage) {
        // 범위 내 적 탐지 및 데미지 가하기
        // 데미지 계산
        var rate = stored_spell[OraType.Fire].level * 0.1f;
        Damage tmp_damage = new() {
            ethereal = damage.ethereal * rate,
            material = damage.material * rate,
            type = OraType.None
        };

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2f, LayerMask.GetMask("Enemy"));

        foreach (Collider2D hit in hits) {
            // 모든 hit 에너미에 데미지 가하기
            hit.gameObject.GetComponent<EnemyHP>().Get_Damage(tmp_damage);
        }
    }
    private int poisoning_time = 3;
    private int timer = 0;
    private void Poison_Active(Damage damage) {
        var rate = stored_spell[OraType.Ice].level * 0.05f;
        Damage tmp_damage = new() {
            ethereal = damage.ethereal * rate,
            material = damage.material * rate,
            type = OraType.None
        };
        // 갱신
        if (poisoning) {
            timer = 0;
        }
        // 실행
        else {
            poisoning = true;
            StartCoroutine(Poisoning(tmp_damage));
        }
    }

    IEnumerator Poisoning(Damage damage) {
        timer = 0;
        while(timer < poisoning_time) {
            GetComponent<EnemyHP>().Get_Damage(damage);
            timer++;
            yield return wfs_1;
        }
        timer = 0;
        poisoning = false;
    }

    private void Die() {
        // die check
        if(ethereal > 0 || material > 0) {
            return;
        }
        alive = false;
        gameObject.layer = 7;
        GetComponent<EnemyMover>().enabled = false;
        GetComponent<Animator>().SetBool("Die", true);

        // 보상
        float val;
        float skill_bonus_gain_rate = user.GetSkillValue(SkillType.GainGold);
        float skill_bonus_misson_gold_rate = user.GetSkillValue(SkillType.MissionGold);
        if (gameObject.name.Contains("boss")) {
            // mission boss 
            if (gameObject.name.Contains("mission")) {
                int number = int.Parse(gameObject.name.Split("_")[2]);

                float default_gain_gold = (number * 50) + (difficulty - 1) * 10;
                val = skill_bonus_misson_gold_rate > 0 ? default_gain_gold + default_gain_gold * skill_bonus_misson_gold_rate / 100 : default_gain_gold;

                if (number > 2) {
                    stone_count++;
                }
                piece_count++;
                Achievement_Observer.Instance.Mission_Boss_Kill_Achievement(spawn_round);
                if(spawn_round == 3) {
                    Achievement_Observer.Instance.Mission_Complete_Achievement("b");
                }
                else if(spawn_round == 5) {
                    Achievement_Observer.Instance.Mission_Complete_Achievement("s");
                }
                
            }
            else {
                // stage boss
                if (Round_counter >= 40) {
                    stone_count++;
                }
                piece_count++;
                float default_gain_gold = Round_counter * 10;
                val = skill_bonus_gain_rate > 0 ? default_gain_gold + default_gain_gold * skill_bonus_gain_rate / 100 : default_gain_gold;
                Achievement_Observer.Instance.Boss_Kill_Achievement(spawn_round);
            }
            In_Game_Setter.Instance.Set_Goods();
        }
        else {
            float default_gain_gold = 2 + Round_counter / 10;
            val = skill_bonus_gain_rate > 0 ? default_gain_gold + default_gain_gold * skill_bonus_gain_rate / 100 : default_gain_gold;
        }
        Set_dot(dot + val);
        // animation 끝나고 리턴
        StartCoroutine(Die_Effect());
    }

    IEnumerator Die_Effect() {
        yield return wfs_1;
        Spawner.Instance.Kill_Enemy(gameObject);
        Enemy_Pool.Instance.Return_Enemy(gameObject);
    }
}
