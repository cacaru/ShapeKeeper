using System.Collections;
using UnityEngine;
using static CUSTOM_DATA.Common_Data;
using static CUSTOM_DATA.Game_Value_Data;
using static CUSTOM_DATA.Game_State_Data;
using CUSTOM_DATA;

public class Bullet_Mover : MonoBehaviour
{
    // target 
    // target을 향해 가야함
    // is_gaming이 false 면 멈춰야함
    // 게임 배속에 따라 영향을 받아야함
    private GameObject target;
    private int target_id = -1;
    private Damage damage = new();

    public void Set_Target(GameObject _target) {
        target = _target;

        if (target != null) {
            target_id = target.GetComponent<EnemyHP>().Enmey_id;
            StartCoroutine(Moving());
        }
    }
    public void Set_Color(Color color) {
        GetComponent<SpriteRenderer>().color = color;
    }

    public void Set_Damage(Damage _damage) {
        damage.material = _damage.material;
        damage.ethereal = _damage.ethereal;
        damage.type = _damage.type;
    }

    IEnumerator Moving() {
        while (true) {
            if (is_gaming) {

                // move
                transform.position = Vector2.MoveTowards(transform.position, target.transform.position, Bullet_Speed * game_speed);

                // target이 없으면 자체 리턴
                if(!target.GetComponent<EnemyHP>().alive) {
                    Returnning();
                }
            }
            yield return wffu;
        }
    }
    
    // 타겟에 도달하면 데미지를 가해야함
    private void OnTriggerEnter2D(Collider2D collision) {
        // target에 충돌하면
        if (collision.TryGetComponent<EnemyHP>(out var target_tag_comp)) {
            if (target_tag_comp.Enmey_id == target_id) {
                
                // 데미지 가하기
                collision.gameObject.GetComponent<EnemyHP>().Get_Damage(damage);
                //Debug.Log("Target hit");
                // 이 bullet 삭제
                Returnning();
            }
        }
    }

    private void Returnning() {
        target = null;
        target_id = -1;
        Bullet_Pool.Instance.Return(gameObject);
    }

}
