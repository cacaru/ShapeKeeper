public class Achievement : Mission
{
    public bool repeat = false;             // n에 의한 반복 보상 판별 여부
    public int end_time;                    // 반복 보상의 마지막 단계
    public int endless_value;               // 무한 반복 단위
}
