using System.Collections;
using UnityEngine;
using static CUSTOM_DATA.Game_Value_Data;
using static CUSTOM_DATA.Common_Data;
using DG.Tweening;

public class Start_Activate : MonoBehaviour
{
    [SerializeField] private GameObject Start_Btn;
    [SerializeField] private GameObject Stamina;
    [SerializeField] private GameObject Skill_Btn;
    [SerializeField] private GameObject Select_Stage;
    [SerializeField] private GameObject SKill_Page;

    private Animator stamina_ani;
    private Animator select_stage_ani;

    private Vector3 close = new(90, 0, 0);

    void Start() {
        stamina_ani = Stamina.GetComponent<Animator>();
        select_stage_ani = Select_Stage.GetComponent<Animator>();
    }



    public void Stage_Select_Click() {
        // btn 을 안보이게
        Home_Setter.Instance.Btn_Show_Toggle();
        Start_Btn.transform.Find("Gori").GetComponent<Start_Hole_Spinner>().enabled = false;

        StartCoroutine(Open_Stage());
    }

    public void Home_Btn_Show() {
        select_stage_ani.SetBool(ANI_ACTIVATE, false);
        // 게임 시작을 위해 선택했던 모든 값 초기화
        Game_Start_Checker.Instance.Start_Btn_Deactive();

        StartCoroutine(Open_Home());
    }

    public void Skill_Select_Click() {
        //btn 안보이게
        Home_Setter.Instance.Btn_Show_Toggle();
        Start_Btn.transform.Find("Gori").GetComponent<Start_Hole_Spinner>().enabled = false;

        // stamina 안보이게
        stamina_ani.SetBool(ANI_ACTIVATE, false);

        // skill open
        StartCoroutine(Open_Skill());
    }

    public void Close_Skill_Page() {
        SKill_Page.GetComponent<RectTransform>().DORotate(close, .25f);

        StartCoroutine(Open_Home());
    }

    IEnumerator Open_Stage() {
        yield return wfs_0_3;

        // stage select 화면을 보이게
        select_stage_ani.SetBool(ANI_ACTIVATE, true);
    }

    IEnumerator Open_Home() {
        yield return wfs_0_3;
        // stamina 보이게
        stamina_ani.SetBool(ANI_ACTIVATE, true);

        // 첫 시작 화면 보이기
        Home_Setter.Instance.Btn_Show_Toggle();
        Start_Btn.transform.Find("Gori").GetComponent<Start_Hole_Spinner>().enabled = true;
    }

    IEnumerator Open_Skill() {
        yield return wfs_0_3;

        SKill_Page.GetComponent<RectTransform>().DORotate(open , .25f);
    }
}
