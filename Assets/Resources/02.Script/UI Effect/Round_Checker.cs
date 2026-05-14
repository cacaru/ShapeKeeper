using TMPro;
using UnityEngine;
using static CUSTOM_DATA.Game_State_Data;
public class Round_Checker : Scene_Singleton<Round_Checker>
{
    private TMP_Text round_text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        round_text = transform.Find("Round").GetComponent<TMP_Text>();
    }

    public void Observing() {
        round_text.text = Round_counter.ToString();
    }
}
