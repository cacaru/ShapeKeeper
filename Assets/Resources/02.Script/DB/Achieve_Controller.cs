using CUSTOM_DATA;

public class Achieve_Controller : Singleton<Achieve_Controller>
{
    /// <summary>
    /// 주간 / 일간 업적 초기화 부분
    /// </summary>
    public void Daily_Reset() {
        Utility.builder.Clear();
        string query = Utility.builder.Append("UPDATE dailyquest SET checker=0, counter=0")
                                      .ToString();
        ModifyDB.Instance.ModifySet(query, "daily");
        Utility.builder.Clear();
    }

    public void Weekly_Reset() {
        Utility.builder.Clear();
        string query = Utility.builder.Append("UPDATE weeklyquest SET checker=0, counter=0")
                                      .ToString();
        ModifyDB.Instance.ModifySet(query, "weekly");
        Utility.builder.Clear();
    }

}
