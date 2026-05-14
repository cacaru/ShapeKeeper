using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CUSTOM_DATA;
using static CUSTOM_DATA.Home_UI_Controller;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Common_Data;
using DG.Tweening;


public class Shop_Seting_Intergrator : Scene_Singleton<Shop_Seting_Intergrator>
{
    // 에너지 구매 / 상자 구매 / 조각 구매 / 조각 판매 완료 => 로딩 패널을 닫고 결과창을 보여줌
    private Sequence loading_seq;
    private Sequence loading_chest_seq;
    private Sequence result_seq;
    private Sequence confirm_seq;
    private Sequence confirm_cancel_seq;
    private Sequence result_cancel_seq;

    [SerializeField] private GameObject confirm_panel;
    [SerializeField] private GameObject loading_panel;
    [SerializeField] private GameObject result_panel;

    private Vector3 close = new(0, 90, 0);

    private Vector2 ori_confirm_img_rect;
    private Vector2 ori_result_img_rect;


    public void Confirm_Field_On() {
        confirm_seq.Restart();
    }

    public void Confirm_Cancel_Click() {
        All_Confirm();
        confirm_cancel_seq.Restart();
    }

    public void Loading_On(int type) {
        switch (type) {
            case 0:  // energy
                loading_seq.Restart();
                break;
            case 1:  // chest
                loading_chest_seq.Restart();
                break;
        }
        //confirm panel deactivate
        confirm_panel.transform.DORotate(close, .25f);
    }

    public void Confirm_Check() {
        result_seq.Restart();
    }

    public void Result_Confirm_Click() {
        All_Confirm();
        result_cancel_seq.Restart();
    }

    private void Start() {
        ori_confirm_img_rect = confirm_panel.transform.Find("Border").Find("buy_item").GetComponent<RectTransform>().sizeDelta;
        ori_result_img_rect = result_panel.transform.Find("Simple_Border").Find("Item_Obj").Find("image").GetComponent<RectTransform>().sizeDelta;

        // result
        confirm_seq = DOTween.Sequence()
                             .Pause()
                             .SetAutoKill(false)
                             .Append(confirm_panel.transform.DORotate(open, .25f));

        // loading 
        loading_seq = DOTween.Sequence()
                             .Pause()
                             .SetAutoKill(false)
                             .Append(confirm_panel.transform.DORotate(close, 0.25f))
                             .Join(loading_panel.transform.DORotate(open, .25f))
                             .AppendCallback(() => Loading.Instance.Loading_Sequence_Start());

        loading_chest_seq = DOTween.Sequence()
                                   .Pause()
                                   .SetAutoKill(false)
                                   .Append(confirm_panel.transform.DORotate(close, 0.25f))
                                   .Join(loading_panel.transform.DORotate(open, .25f))
                                   .AppendCallback(() => Loading.Instance.Loading_Chest());

        // result
        result_seq = DOTween.Sequence()
                            .Pause()
                            .SetAutoKill(false)
                            .Append(loading_panel.transform.DORotate(close, .25f))
                            .Join(result_panel.transform.DORotate(open, .25f));

        confirm_cancel_seq = DOTween.Sequence()
                             .Pause()
                             .SetAutoKill(false)
                             .Append(confirm_panel.transform.DORotate(close, .25f));

        result_cancel_seq = DOTween.Sequence()
                             .Pause()
                             .SetAutoKill(false)
                             .Append(result_panel.transform.DORotate(close, .25f));
    }

    public void All_Confirm() {
        // init confirm_pannel && result pannel && loading pannel
        Buy_Energy.Instance.free_energy = false;
        buy_energy_value = 0;
        // confirm panel
        confirm_panel.transform.Find("Border").Find("buy_announce").GetComponent<TMP_Text>().text = "";
        confirm_panel.transform.Find("Border").Find("buy_announce_title").GetComponent<TMP_Text>().text = "구매 예정 내역";
        confirm_panel.transform.Find("Border").Find("buy_item").GetComponent<Image>().sprite = null;
        confirm_panel.transform.Find("Border").Find("price").GetComponent<TMP_Text>().text = "";
        confirm_panel.transform.Find("Border").transform.Find("price").GetComponent<TMP_Text>().color = Color.black;
        confirm_panel.transform.Find("Border").transform.Find("buy_item+border").GetComponent<Image>().color = Color.white;
        confirm_panel.transform.Find("Confirm").GetComponent<Button>().onClick.RemoveAllListeners();
        confirm_panel.transform.Find("Percent").gameObject.SetActive(false);

        confirm_panel.transform.Find("Border").Find("buy_item").GetComponent<RectTransform>().sizeDelta = ori_confirm_img_rect;

        // result panel
        result_panel.transform.Find("Simple_Border").Find("Item_Obj").Find("image").GetComponent<Image>().sprite = null;
        result_panel.transform.Find("Simple_Border").Find("Item_Obj").Find("name").GetComponent<TMP_Text>().text = "";
        result_panel.transform.Find("Simple_Border").Find("Item_Obj").Find("type").GetComponent<TMP_Text>().text = "";
        result_panel.transform.Find("Simple_Border").Find("piece_before").GetComponent<TMP_Text>().text = "0";
        result_panel.transform.Find("Simple_Border").Find("piece_after").GetComponent<TMP_Text>().text = "0";

        result_panel.transform.Find("Simple_Border").Find("Item_Obj").Find("image").GetComponent<RectTransform>().sizeDelta = ori_result_img_rect;

        var content = result_panel.transform.Find("Slider_Border").Find("Item_View").Find("Viewport").Find("Content").transform;
        foreach( Transform item in content) {
            if(item != content)
                Destroy(item.gameObject);
        }

        result_panel.transform.Find("Simple_Border").gameObject.SetActive(false);
        result_panel.transform.Find("Slider_Border").gameObject.SetActive(false);

        // loading panel
        Loading.Instance.Init_pos();
    }
}
