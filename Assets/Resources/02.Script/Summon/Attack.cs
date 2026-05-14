using CUSTOM_DATA;
using System.Collections;
using UnityEngine;
using static CUSTOM_DATA.Game_State_Data;
using static CUSTOM_DATA.Game_Value_Data;
using static CUSTOM_DATA.Common_Data;
using static CUSTOM_DATA.Game_Data;

public class Attack : MonoBehaviour
{
    // 공격범위
    // 투사체 ( 기존의 sprite 이미지의 색을 등급으로 바꿔 발사하기
    public Damage damage = new();
    public float attack_range;

    public WaitForSeconds attack_time;

    private Color bullet_color;
    private Sprite bullet_img;
    private int target_layer;
    private int count;
    private float target_dis;

    private bool can_material;
    private bool can_etherial;
    private bool can_all;

    private Unit _unit;
    private Installed_Tower tower;

    private OraType type = OraType.None;

    private void Start() {
        target_layer = LayerMask.GetMask("Enemy");
    }

    public OraType Type {
        get { return type; }
        set {
            type = value;
            Reset_Attack();
        }
    }

    public void Init(int unit_id, int tower_id) {
        _unit = unit[unit_id];
        tower = installed_tower[tower_id];

        Reset_Attack();

        attack_range = _unit.grade switch {
            "d" => 1,
            "c" => 1.5f,
            "b" => 2f,
            "a" => 2.5f,
            "s" => 3f,
            "ex" => 4f,
            _ => 0
        };

        if(Utility.Is_Special(unit_id)) {
            attack_range += 0.5f;
        }

        bullet_color = Utility.Get_Grade_Color(_unit.grade);
        bullet_img = Utility.Get_sprite(unit_id);
        // additional setting
        // speed setting
        attack_time = _unit.speed switch {
            1 => wfs_1,
            2 => wfs_0_5,
            3 => wfs_0_3,
            4 => wfs_0_25,
            5 => wfs_0_2,
            6 => wfs_0_18,
            7 => wfs_0_12,
            _ => wfs_1
        };

        StartCoroutine(Attacking());
    }

    public void Reset_Attack() {
        int figure = _unit.grade switch {
            "d" => D_UPGRADE_FIGURE * upgrade_value_d,
            "c" => C_UPGRADE_FIGURE * upgrade_value_c,
            "b" => B_UPGRADE_FIGURE * upgrade_value_b,
            "a" => A_UPGRADE_FIGURE * upgrade_value_a,
            "s" => S_UPGRADE_FIGURE * upgrade_value_s,
            "ex" => EX_UPGRADE_FIGURE * upgrade_value_ex,
            _ => 0
        };
        
        float temp_value = tower.attack + figure;        
        int skill_value = user.GetSkillValue(SkillType.AttackIncrease);
        float now_attack = skill_value > 0 ? temp_value + temp_value * skill_value / 100 : temp_value;

        damage.material = tower.material > 0 ? (now_attack * tower.material / 100) : 0;
        damage.ethereal = tower.ethereal > 0 ? (now_attack * tower.ethereal / 100) : 0;
        damage.type = type;
        Debug.Log(damage.material);
        can_material = damage.material > 0;
        can_etherial = damage.ethereal > 0;
        can_all = can_material && can_etherial;
    }
    /*
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, attack_range);
    }
    */
    IEnumerator Attacking() {
        GameObject tmp_target;
        while (true) {
            if (is_gaming) {
                tmp_target = null;
                // 공격범위 내 타깃 탐색
                var targets = Physics2D.OverlapCircleAll(transform.position, attack_range, target_layer);
                count = targets.Length;
                if (count > 0) {
                    // 범위 내 가장 가까우며 공격 가능한 개체를 찾기
                    target_dis = 999;
                    foreach (var col in targets) {
                        float dis = Vector3.Distance(transform.position, col.transform.position);
                        // 공격 가능 판단
                        if (can_all) {
                            if( dis < target_dis) {
                                target_dis = dis;
                                tmp_target = col.gameObject;
                            }
                        }
                        else if (col.gameObject.GetComponent<EnemyHP>().ethereal > 0) {
                            if (can_etherial && dis < target_dis) {
                                target_dis = dis;
                                tmp_target = col.gameObject;
                            }
                        }
                        else if (col.gameObject.GetComponent<EnemyHP>().material > 0) {
                            if (can_material && dis < target_dis) {
                                target_dis = dis;
                                tmp_target = col.gameObject;
                            }
                        }
                    }
                }
                else {
                    tmp_target = null;
                }

                // target이 있으면 공격
                if(tmp_target != null) {
                    var bullet = Bullet_Pool.Instance.Get();
                    bullet.transform.SetParent(transform, false);
                    bullet.transform.position = transform.position;
                    bullet.GetComponent<SpriteRenderer>().sprite = bullet_img;
                    bullet.transform.GetComponent<Bullet_Mover>().Set_Target(tmp_target);
                    bullet.transform.GetComponent<Bullet_Mover>().Set_Color(bullet_color);
                    bullet.transform.GetComponent<Bullet_Mover>().Set_Damage(damage);
                }

            }
            // attack speed만큼 1초를 쪼개서 반복
            yield return attack_time;
        }
    }
}
