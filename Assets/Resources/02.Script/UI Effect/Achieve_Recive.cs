using UnityEngine;
using UnityEngine.EventSystems;

public class Achieve_Recive : MonoBehaviour, IPointerClickHandler
{

    public int Id = 0;
    public int type = 0; // 1 : daily . 2 : weekly . 3 : achieve

    public void OnPointerClick(PointerEventData eventData) {
        if (Id <= 0) return;

        // 해당 번호의 id 의 업적을 확인해서 확인 모달을 띄우기
        Achievement_Recive_Setter.Instance.Setting(Id, type);
    }

}
