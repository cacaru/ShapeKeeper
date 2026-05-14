using UnityEngine;

using DG.Tweening;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{
    [SerializeField] private GameObject white_back;
    [SerializeField] private GameObject black_back;

    private Sequence load;
    private Color target_color = new(0f,0f,0f,0);
    private Color ori_color = new(0f,0f,0f,1);

    public Ease ease;
    //private Vector3 scale_vector = new(2, 2, 2);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ease = Ease.InOutExpo;

        load = DOTween.Sequence().SetAutoKill(false)
            .Append(black_back.transform.DOScale(30f, 2f)).SetEase(ease)
            .Append(black_back.GetComponent<Image>().DOColor(target_color, 1))
            ;

        /*
        load = DOTween.Sequence().SetAutoKill(false)
            .Append(white_back.transform.DOScale(3f, 2f)).SetEase(ease)
            .Append(black_back.transform.DOScale(0, 1))
            .Append(white_back.GetComponent<Image>().DOColor(target_color, 1))
            .AppendCallback(() => {
                gameObject.SetActive(false);
                white_back.SetActive(false);
                black_back.SetActive(false);
                white_back.transform.localScale = Vector3.zero;
                white_back.GetComponent<Image>().color = ori_color;
            })
            ;
        */
        load.Play();
    }

}
