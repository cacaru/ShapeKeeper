using UnityEngine;
using UnityEngine.EventSystems;

public class Combine_Board : MonoBehaviour, IPointerClickHandler 
{
    public int Id = 0;
    public int function_id = -1;
    public bool is_material = true;
    public bool is_field = false;

    public void OnPointerClick(PointerEventData eventData) {
        
        // 클릭한 유닛을 조합할 수 있는 식들을 보여줘야함
        if (is_material) {
            if(Id < 1000) {
                return;
            }
            // combine table reset
            if(Id > 2000) {
                Unit_Info_Pannel_Setter.Instance.Set_Info(Id);
            }
            if(Id > 1000) {
                Combine_Observer.Instance.Set_Combine_Field_from_Combine_Material(Id);
            }
            // 상세정보의 유닛을 수정함
            Unit_Info_Pannel_Setter.Instance.Set_Info(Id);
        }

        // try combine
        else {
            Combine_Observer.Instance.Combine_Checker(function_id, is_field);
        }
    }
    
}
