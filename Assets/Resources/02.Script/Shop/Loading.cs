using CUSTOM_DATA;
using DG.Tweening;
using UnityEngine;
using static CUSTOM_DATA.Game_State_Data;

public class Loading : Scene_Singleton<Loading>
{
    private Sequence sq;
    private Sequence chest_seq;
    [SerializeField] private GameObject loading_bar;
    [SerializeField] private GameObject retry_btn;
    private Vector3 ori_pos = new(0, 1200f, 0);

    public void Loading_Sequence_Start() {
        loading_counter = 0;
        sq.Restart();
    }

    public void Loading_Chest() {
        chest_seq.Restart();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float top = Screen.height;
        float bottom = 0;
        ori_pos.y = bottom;
        loading_bar.transform.position = ori_pos;

        sq = DOTween.Sequence().Pause().SetAutoKill(false)
                    .Append(loading_bar.transform.DOMoveY(top, 1.5f).SetEase(Ease.InOutExpo))
                    .Append(loading_bar.transform.DOMoveY(bottom, 1.5f).SetEase(Ease.InOutExpo))
                    .AppendCallback(Check_Load_End);

        chest_seq = DOTween.Sequence().Pause().SetAutoKill(false)
                           .Append(loading_bar.transform.DOMoveY(top, 1.5f).SetEase(Ease.InOutExpo))
                           .Append(loading_bar.transform.DOMoveY(bottom, 1.5f).SetEase(Ease.InOutExpo))
                           .AppendCallback(Check_Chest_End);
    }

    // 로딩이 5초 이상 지속되면 원인에 대한 재시도를 진행해야함
    private int loading_counter = 0;
    private void Check_Load_End() {
        loading_counter++;
        if(loading_counter == 5) {
            // retry btn open
            retry_btn.transform.DORotate(Vector3.zero, 0.25f);
        }
        // 확인 검증을 어떻게 하지?
        // db state 확인
        // header state 설정
        if(db_state == DB_STATE.Usable && Header_Setter.Instance.setting) {
            // result announce
            Shop_Seting_Intergrator.Instance.Confirm_Check();
        }
        else {
            sq.Restart();
        }
    }

    private void Check_Chest_End() {
        if (db_state == DB_STATE.Usable && Buy_Chest.Instance.Chest_Setting) {
            // result announce
            Shop_Seting_Intergrator.Instance.Confirm_Check();
        }
        else {
            chest_seq.Restart();
        }
    }

    public void Init_pos() {
        loading_bar.transform.position = ori_pos;
    }

}
