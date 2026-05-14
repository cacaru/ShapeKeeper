using UnityEngine;
using UnityEngine.EventSystems;

public class Combine_Book : MonoBehaviour, IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventData) {
        int item = int.Parse(eventData.pointerCurrentRaycast.gameObject.name);
        //Debug.Log(item);

        // item 이 유닛의 키에 등록이 되어있으면
        // 조합 보기 펴기
        Combine_Observer.Instance.Set_Combine_Book_Click(item);
        Pannel_Controller.Instance.Activate_for_Combine();
    }

}
