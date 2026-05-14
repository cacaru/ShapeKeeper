using CUSTOM_DATA;
using DG.Tweening;
using UnityEngine;
using static CUSTOM_DATA.Game_State_Data;
using static CUSTOM_DATA.Common_Data;

public class Achieve_Loading : Scene_Singleton<Achieve_Loading>
{
    [SerializeField] private GameObject loading_bar;
    [SerializeField] private GameObject Recive_Check_Panel;
    [SerializeField] private GameObject Loading_Panel;
    [SerializeField] private GameObject Result_Panel;

    private Vector3 ori_pos = new(0, 1200f, 0);

    private Vector3 close = new(90, 0, 0);
    
    Sequence open_recive_seq;
    Sequence close_recive_seq;
    Sequence to_loading_seq;
    Sequence loading_bar_seq;

    Sequence to_all_recive_loading_seq;
    Sequence to_all_recive_loading_bar_seq;

    Sequence end_loading_seq;
    Sequence back_seq;

    private int type;
    private int id;

    public void Start_Load(int id, int type) {
        this.id = id;
        this.type = type;
        to_loading_seq.Restart();
    }

    public void All_Recive_Load(int type) {
        this.type = type;
        id = -1;
        to_all_recive_loading_seq.Restart();
    }

    void Start()
    {
        float top = Screen.height + 100f;
        float bottom = 0;
        ori_pos.y = bottom;
        ori_pos.z = 0; ori_pos.x = 0;
        loading_bar.transform.position = ori_pos;
        
        // open recive check
        open_recive_seq = DOTween.Sequence().Pause().SetAutoKill(false)
                                 .Append(Recive_Check_Panel.transform.DORotate(open, 0.25f));

        close_recive_seq = DOTween.Sequence().Pause().SetAutoKill(false)
                                  .Append(Recive_Check_Panel.transform.DORotate(close, 0.25f));

        to_all_recive_loading_seq = DOTween.Sequence().Pause().SetAutoKill(false)
                                           .Append(Recive_Check_Panel.transform.DORotate(close, 0.25f))
                                           .Join(Loading_Panel.transform.DORotate(open, .25f))
                                           .AppendCallback(() => {
                                                loading_bar.transform.position = ori_pos;
                                                to_all_recive_loading_bar_seq.Restart();
                                            });

        to_all_recive_loading_bar_seq = DOTween.Sequence().SetAutoKill(false).Pause()
                                               .Append(loading_bar.transform.DOMoveY(top, 1.5f).SetEase(Ease.InOutExpo))
                                               .Append(loading_bar.transform.DOMoveY(bottom, 1.5f).SetEase(Ease.InOutExpo))
                                               .AppendCallback(() => {
                                                   if (db_state == DB_STATE.Usable) {
                                                        Achievement_Recive_Setter.Instance.Setting_Result();
                                                        Header_Setter.Instance.Set_Head();
                                                        switch (type) {
                                                            case 1:
                                                                 Achievement_Loader.Instance.Daily_Shower();
                                                                break;
                                                            case 2:
                                                                Achievement_Loader.Instance.Weekly_Shower();
                                                                break;
                                                            case 3:
                                                                 Achievement_Loader.Instance.Achieve_Shower();
                                                                break;
                                                        }
                                                    }
                                                    Check_Load_End();
                                                });

        // recive check -> loading
        to_loading_seq = DOTween.Sequence().Pause().SetAutoKill(false)
                                .Append(Recive_Check_Panel.transform.DORotate(close, 0.25f))
                                .Join(Loading_Panel.transform.DORotate(open, .25f))
                                .AppendCallback(() => {
                                    loading_bar.transform.position = ori_pos;
                                    loading_bar_seq.Restart(); 
                                });
        // loading bar 
        loading_bar_seq = DOTween.Sequence().SetAutoKill(false).Pause()
                                 .Append(loading_bar.transform.DOMoveY(top, 1.5f).SetEase(Ease.InOutExpo))
                                 .Append(loading_bar.transform.DOMoveY(bottom, 1.5f).SetEase(Ease.InOutExpo))
                                 .AppendCallback(() => {
                                     if(db_state == DB_STATE.Usable) {
                                         Achievement_Recive_Setter.Instance.Setting_Result(id, type);
                                         Header_Setter.Instance.Set_Head();
                                         switch (type) {
                                             case 1:
                                                 Achievement_Loader.Instance.Daily_Shower();
                                                 break;
                                             case 2:
                                                 Achievement_Loader.Instance.Weekly_Shower();
                                                 break;
                                             case 3:
                                                 Achievement_Loader.Instance.Achieve_Shower();
                                                 break;
                                         }
                                     }
                                     Check_Load_End();
                                });

        // loading -> result panel
        end_loading_seq = DOTween.Sequence().SetAutoKill(false).Pause()
                                 .Append(Loading_Panel.transform.DORotate(close, 0.25f))
                                 .Join(Result_Panel.transform.DORotate(open, .25f));

        back_seq = DOTween.Sequence().SetAutoKill(false).Pause()
                          .Append(Result_Panel.transform.DORotate(close, .25f))
                          ;
    }

    private void Check_Load_End() {

        // db end , header set end , result_page load 
        if (db_state == DB_STATE.Usable && Header_Setter.Instance.setting && Achievement_Loader.Instance.is_load && Achievement_Recive_Setter.Instance.loading_end ){
            // result announce
            end_loading_seq.Restart();
        }
        else {
            loading_bar_seq.Restart();
        }
    }
    
    public void Open_Recive_Check() {
        open_recive_seq.Restart();
    }

    public void Close_Recive_ChecK() {
        close_recive_seq.Restart();
    }

    public void All_Confirm_Recive() {
        back_seq.Restart();
    }
}
