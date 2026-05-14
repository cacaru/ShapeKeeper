using System.Collections;

public class Mission
{
    // cover
    public int id;                          // 번호
    public string name;                     // 이름
    public int checker;                     // 보상 받았는지 여부
    public int counter;                     // 보상 달성 여부
    public ArrayList reward_list;           // 보상받을 아이템 리스트
    public ArrayList reward_val;            // 보상 아이템당 갯수
    public int request_counter;             // 보상받을때 까지 필요한 counter
    public bool can_recive = false;         // 지금 받을 수 있는지 여부
}
