using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Common_Data;

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using CUSTOM_DATA;

public class Spell_Active : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler , ISpell_Recognition_Observer
{
    public OraType type;

    private readonly float check_time = 0.6f;
    bool spell_install_active;
    bool spell_in_tower_locking = false;
    float hold_time = 0;

    private Vector3 click_pos;
    private GameObject target_tower;

    private GameObject instance_lock;
    private Rigidbody2D rb;

    void Start() {

    }

    public void OnDrag(PointerEventData eventData) {
        if (!spell_install_active) return;

        click_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        click_pos.z = 1;
        click_pos.y += .8f;

        // 충돌 중이면 스냅되지만, 드래그는 허용
        if (spell_in_tower_locking && target_tower != null) {
            if (Is_Mouse_Over_Tower(click_pos)) {
                rb.MovePosition(target_tower.transform.position);
            }
            else {
                rb.gameObject.GetComponent<Target_Recognition>().Remove_Current_Tower();
                Tower_Lock_Off();
                rb.MovePosition(click_pos);
            }
        }
        else {
            rb.MovePosition(click_pos);
        }

    }

    private bool Is_Mouse_Over_Tower(Vector3 click_pos) {
        Collider2D col = target_tower.GetComponent<Collider2D>();
        if (col == null) return false;
        return col.OverlapPoint(click_pos);
    }

    public void Tower_Lock_On(GameObject tower) {
        if (spell_install_active) {
            spell_in_tower_locking = true;
            // target tower저장
            target_tower = tower;

            // 위치 파악
            rb.MovePosition(target_tower.transform.position);
        }
    }

    public void Tower_Lock_Off() {
        if (spell_install_active) {
            spell_in_tower_locking = false;
            target_tower = null;
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        spell_install_active = false;
        StopAllCoroutines();

        // tower check
        if(target_tower != null) {
            // 다른 오라가 있으면 교체해야함
            if(target_tower.GetComponent<Attack>().Type != OraType.None) {
                //Debug.Log("ora replace");

                return;
            }
            // 없으면 붙이기
            stored_spell[type].count--;
            var ora = Ora_Pool.Instance.Get_Ora(type);
            ora.transform.SetParent(target_tower.transform, false);

            // add attack type
            target_tower.GetComponent<Attack>().Type = type;

            // count show
            Spell_Setter.Instance.Spell_Counter_Set();

        }
        else {
            //Debug.Log("nothing");
        }

        if (instance_lock != null) {
            instance_lock.GetComponent<Target_Recognition>().Remove_Observer(this);
            instance_lock = null;
        }
        target_tower = null;
        Lock_On_Pool.Instance.Return();
        Pannel_Controller.Instance.Deactivate_for_Spell_Install();
    }

    public void OnPointerDown(PointerEventData eventData) {
        spell_install_active = false;
        hold_time = 0;
        // 현재 눌린 스펠이 남아있는지 ( 적용할 수 있는 갯수가 있는지 ) 확인
        if (stored_spell[type].count <= 0) return;

        StartCoroutine(DownChecker());
    }

    IEnumerator DownChecker() {
        while (hold_time <= 1.0f) {
            hold_time += Time.deltaTime / check_time;
            yield return null;
        }
        spell_install_active = true;

        // all pannel down
        Pannel_Controller.Instance.Activate_for_Install();
        click_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        click_pos.z = 1;
        click_pos.y += .8f;
        // 타겟 이미지 불러오기
        instance_lock = Lock_On_Pool.Instance.Activate(type, click_pos);
        instance_lock.GetComponent<Target_Recognition>().Add_Observer(this);
        rb = instance_lock.GetComponent<Rigidbody2D>();
    }


}
