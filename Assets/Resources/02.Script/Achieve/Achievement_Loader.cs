using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Common_Data;
using TMPro;
using System.Collections;
public class Achievement_Loader : Scene_Singleton<Achievement_Loader>
{
    /// 업적 화면 설정
    /// 받기 가능 / 불가능 - checker
    /// 진행률 - rate
    /// 업적 이름 - title
    /// prefab => achieve_prefab
    /// 업적 터치시 상세 + 확인 버튼 나오게 
    /// 받기 가능하면 외관에는 모두받기 버튼 활성화 
    [SerializeField] Button all_recive_btn;
    [SerializeField] GameObject daily_header;
    [SerializeField] GameObject weekly_header;
    [SerializeField] GameObject achieve_header;

    [SerializeField] GameObject shower;
    [SerializeField] GameObject content;

    private Color On = new(134/255f, 1f, 1f, 1f);
    private Color Off = new(134/255f, 1f, 1f, 0f);

    private Vector2 CONTENT_TOP = new Vector2(0, 1);

    private readonly Color Can_Recive_Color = Color.green;
    private readonly Color Normal_Color = Color.red;
    private readonly Color Already_Recive_Color = Color.black;

    public bool is_load = false;
    void Start()
    {
        Content_Clear();

        // 첫 화면은 항상 일일퀘
        daily_header.GetComponent<Image>().color = On;
        weekly_header.GetComponent<Image>().color = Off;
        achieve_header.GetComponent<Image>().color = Off;

        // 일일퀘 목록 업데이트
        Achievement_Observer.Instance.Daily_Complete_Check();

        Open_Panel("daily");
    }

    public void Open_Panel(string type) {
        switch (type) {
            case "daily":
                daily_header.GetComponent<Image>().color = On;
                weekly_header.GetComponent<Image>().color = Off;
                achieve_header.GetComponent<Image>().color = Off;
                all_recive_btn.onClick.RemoveAllListeners();
                all_recive_btn.onClick.AddListener(() => {
                    Achievement_Recive_Setter.Instance.All_Recive_Mission(1);
                });
                Daily_Shower();
                break;
            case "weekly":
                daily_header.GetComponent<Image>().color = Off;
                weekly_header.GetComponent<Image>().color = On;
                achieve_header.GetComponent<Image>().color = Off;
                all_recive_btn.onClick.RemoveAllListeners();
                all_recive_btn.onClick.AddListener(() => {
                    Achievement_Recive_Setter.Instance.All_Recive_Mission(2);
                });
                Weekly_Shower();
                break;
            case "achieve":
                daily_header.GetComponent<Image>().color = Off;
                weekly_header.GetComponent<Image>().color = Off;
                achieve_header.GetComponent<Image>().color = On;
                all_recive_btn.onClick.RemoveAllListeners();
                all_recive_btn.onClick.AddListener(() => {
                    Achievement_Recive_Setter.Instance.All_Recive_Mission(3);
                });
                Achieve_Shower();
                break;
        }
    }

    public void Daily_Shower() {
        is_load = false;
        Content_Clear();
        // daily 업적 내용을 토대로 받기 가능한것부터 위로 올려서 
        // 오브젝트 생성하기
        // 받기 가능 
        List<Quest> can_recive_quest = new();
        List<Quest> normal_quest = new();
        List<Quest> end_quest = new();
        // end 부터 확인
        // 기왕이면 번호순대로
        // 일일 업적은 10개이므로 10개 순서대로 확인하기
        for(int i = 1; i < 11; i++) {
            if (daily_quest[i].ended) {
                end_quest.Add(daily_quest[i]);
            }
            // 받기 가능
            else {
                if (daily_quest[i].can_recive) {
                    can_recive_quest.Add(daily_quest[i]);
                }
                else {
                    normal_quest.Add(daily_quest[i]);
                }
            }
        }

        // 받기 가능부터 출력
        int size = can_recive_quest.Count;
        for(int i =0; i < size; i++) {
            GameObject quest = Instantiate(achieve_prefab, content.transform);
            quest.transform.SetParent(content.transform, false);
            quest.transform.localScale = Vector3.one;
            // 이름
            quest.transform.Find("title").GetComponent<TMP_Text>().text = can_recive_quest[i].name;
            // 받기 가능
            quest.transform.Find("checker").GetComponent<Image>().color = Can_Recive_Color;
            quest.transform.Find("icon").GetComponent<Image>().sprite = achieve_can_recive_icon;
            quest.transform.Find("rate").GetComponent<Slider>().value = 1;
            quest.GetComponent<Achieve_Recive>().Id = can_recive_quest[i].id;
            quest.GetComponent<Achieve_Recive>().type = 1;
        }

        size = normal_quest.Count;
        for(int i = 0; i < size; i++) {
            GameObject quest = Instantiate(achieve_prefab, content.transform);
            quest.transform.SetParent(content.transform, false);
            quest.transform.localScale = Vector3.one;
            // 이름
            quest.transform.Find("title").GetComponent<TMP_Text>().text = normal_quest[i].name;
            // 받기 가능
            quest.transform.Find("checker").GetComponent<Image>().color = Normal_Color;
            quest.transform.Find("icon").GetComponent<Image>().sprite = achieve_normal_icon;
            float value = 0;
            if (normal_quest[i].request_counter > 0 && normal_quest[i].counter > 0) {
                value = (float)normal_quest[i].counter / normal_quest[i].request_counter ;
            }
            quest.transform.Find("rate").GetComponent<Slider>().value = value;
            quest.GetComponent<Achieve_Recive>().Id = normal_quest[i].id;
            quest.GetComponent<Achieve_Recive>().type = 1;
        }
        
        size = end_quest.Count;
        for(int i = 0; i < size; i++) {
            GameObject quest = Instantiate(achieve_prefab, content.transform);
            // 전체적으로 어둡게

            quest.transform.SetParent(content.transform, false);
            quest.transform.localScale = Vector3.one;
            // 이름
            quest.transform.Find("title").GetComponent<TMP_Text>().text = end_quest[i].name;
            // 받기 가능
            quest.transform.Find("checker").GetComponent<Image>().color = Already_Recive_Color;
            quest.transform.Find("icon").GetComponent<Image>().sprite = achieve_can_recive_icon;
            quest.transform.Find("rate").GetComponent<Slider>().value = 1;

            quest.GetComponent<Achieve_Recive>().Id = end_quest[i].id;
            quest.GetComponent<Achieve_Recive>().type = 1;
        }
        is_load = true;
    }

    public void Weekly_Shower() {
        is_load = false;
        Content_Clear();
        // 받기 가능 
        List<Quest> can_recive_quest = new();
        List<Quest> normal_quest = new();
        List<Quest> end_quest = new();
        // end 부터 확인
        // 기왕이면 번호순대로
        // 일일 업적은 10개이므로 10개 순서대로 확인하기
        for (int i = 1; i < 11; i++) {
            if (weekly_quest[i].ended) {
                end_quest.Add(weekly_quest[i]);
            }
            // 받기 가능
            else {
                if (weekly_quest[i].can_recive) {
                    can_recive_quest.Add(weekly_quest[i]);
                }
                else {
                    normal_quest.Add(weekly_quest[i]);
                }
            }
        }

        // 받기 가능부터 출력
        int size = can_recive_quest.Count;
        for (int i = 0; i < size; i++) {
            GameObject quest = Instantiate(achieve_prefab, content.transform);
            quest.transform.SetParent(content.transform, false);
            quest.transform.localScale = Vector3.one;
            // 이름
            quest.transform.Find("title").GetComponent<TMP_Text>().text = can_recive_quest[i].name;
            // 받기 가능
            quest.transform.Find("checker").GetComponent<Image>().color = Can_Recive_Color;
            quest.transform.Find("icon").GetComponent<Image>().sprite = achieve_can_recive_icon;
            quest.transform.Find("rate").GetComponent<Slider>().value = 1;

            quest.GetComponent<Achieve_Recive>().Id = can_recive_quest[i].id;
            quest.GetComponent<Achieve_Recive>().type = 2;
        }

        size = normal_quest.Count;
        for (int i = 0; i < size; i++) {
            GameObject quest = Instantiate(achieve_prefab, content.transform);
            quest.transform.SetParent(content.transform, false);
            quest.transform.localScale = Vector3.one;
            // 이름
            quest.transform.Find("title").GetComponent<TMP_Text>().text = normal_quest[i].name;
            // 받기 가능
            quest.transform.Find("checker").GetComponent<Image>().color = Normal_Color;
            quest.transform.Find("icon").GetComponent<Image>().sprite = achieve_normal_icon;
            float value = 0;
            if (normal_quest[i].request_counter > 0 && normal_quest[i].counter > 0) {
                value = (float)normal_quest[i].counter / normal_quest[i].request_counter;
            }
            quest.transform.Find("rate").GetComponent<Slider>().value = value;

            quest.GetComponent<Achieve_Recive>().Id = normal_quest[i].id;
            quest.GetComponent<Achieve_Recive>().type = 2;
        }

        size = end_quest.Count;
        for (int i = 0; i < size; i++) {
            GameObject quest = Instantiate(achieve_prefab, content.transform);
            // 전체적으로 어둡게

            quest.transform.SetParent(content.transform, false);
            quest.transform.localScale = Vector3.one;
            // 이름
            quest.transform.Find("title").GetComponent<TMP_Text>().text = end_quest[i].name;
            // 받기 가능
            quest.transform.Find("checker").GetComponent<Image>().color = Already_Recive_Color;
            quest.transform.Find("icon").GetComponent<Image>().sprite = achieve_can_recive_icon;
            quest.transform.Find("rate").GetComponent<Slider>().value = 1;

            quest.GetComponent<Achieve_Recive>().Id = end_quest[i].id;
            quest.GetComponent<Achieve_Recive>().type = 2;
        }
        is_load = true;
    }

    public void Achieve_Shower() {
        is_load = false;
        Content_Clear();

        // 받기 가능 
        List<Achievement> can_recive_quest = new();
        List<Achievement> normal_quest = new();
        List<Achievement> end_quest = new();
        // end 부터 확인
        // 기왕이면 번호순대로
        // 일일 업적은 10개이므로 10개 순서대로 확인하기
        int size = achievement.Count;
        for (int i = 1; i <= size; i++) {
            if (!achievement[i].repeat && achievement[i].checker == 1) {
                end_quest.Add(achievement[i]);
            }
            // 받기 가능
            else {
                if (achievement[i].can_recive) {
                    can_recive_quest.Add(achievement[i]);
                }
                else {
                    normal_quest.Add(achievement[i]);
                }
            }
        }

        // 받기 가능부터 출력
        size = can_recive_quest.Count;
        for (int i = 0; i < size; i++) {
            GameObject quest = Instantiate(achieve_prefab, content.transform);
            quest.transform.SetParent(content.transform, false);
            quest.transform.localScale = Vector3.one;
            // 이름
            quest.transform.Find("title").GetComponent<TMP_Text>().text = can_recive_quest[i].name;
            // 받기 가능
            quest.transform.Find("checker").GetComponent<Image>().color = Can_Recive_Color;
            quest.transform.Find("icon").GetComponent<Image>().sprite = achieve_can_recive_icon;
            quest.transform.Find("rate").GetComponent<Slider>().value = 1;

            quest.GetComponent<Achieve_Recive>().Id = can_recive_quest[i].id;
            quest.GetComponent<Achieve_Recive>().type = 3;
        }

        size = normal_quest.Count;
        for (int i = 0; i < size; i++) {
            GameObject quest = Instantiate(achieve_prefab, content.transform);
            quest.transform.SetParent(content.transform, false);
            quest.transform.localScale = Vector3.one;
            // 이름
            quest.transform.Find("title").GetComponent<TMP_Text>().text = normal_quest[i].name;
            // 받기 가능
            quest.transform.Find("checker").GetComponent<Image>().color = Normal_Color;
            quest.transform.Find("icon").GetComponent<Image>().sprite = achieve_normal_icon;
            float value = 0;
            if (normal_quest[i].repeat) {
                // 반복값 구하기
                value = (float)normal_quest[i].counter / normal_quest[i].endless_value * (normal_quest[i].checker + 1);
            }
            else {
                // else는 단일 한정 업적이므로 value는 반드시 0;
                value = 0;
            }

            quest.transform.Find("rate").GetComponent<Slider>().value = value;

            quest.GetComponent<Achieve_Recive>().Id = normal_quest[i].id;
            quest.GetComponent<Achieve_Recive>().type = 3;
        }

        size = end_quest.Count;
        for (int i = 0; i < size; i++) {
            GameObject quest = Instantiate(achieve_prefab, content.transform);
            // 전체적으로 어둡게

            quest.transform.SetParent(content.transform, false);
            quest.transform.localScale = Vector3.one;
            // 이름
            quest.transform.Find("title").GetComponent<TMP_Text>().text = end_quest[i].name;
            // 받기 가능
            quest.transform.Find("checker").GetComponent<Image>().color = Already_Recive_Color;
            quest.transform.Find("icon").GetComponent<Image>().sprite = achieve_can_recive_icon;
            quest.transform.Find("rate").GetComponent<Slider>().value = 1;

            quest.GetComponent<Achieve_Recive>().Id = end_quest[i].id;
            quest.GetComponent<Achieve_Recive>().type = 3;
        }


        is_load = true;
    }


    public void Content_Clear() {
        foreach(Transform item in content.transform) {
            if(item != content.transform) {
                Destroy(item.gameObject);
            }
        }

        shower.GetComponent<ScrollRect>().normalizedPosition = CONTENT_TOP;
    }

}
