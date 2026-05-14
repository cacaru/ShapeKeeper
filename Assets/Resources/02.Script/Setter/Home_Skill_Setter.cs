using CUSTOM_DATA;
using System;
using System.Collections;
using UnityEngine;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Common_Data;
using static CUSTOM_DATA.Game_State_Data;
using static CUSTOM_DATA.Game_Value_Data;
using TMPro;
using UnityEngine.UI;

public class Home_Skill_Setter : Scene_Singleton<Home_Skill_Setter> {
    // user의 스킬 값들을 가져와 각 오브젝트별로 뿌려주기
    // prefab으로 생성 / 풀링 안함
    // 5개 밖에 없어

    // 스킬 레벨/ 효과 / 스킬업 
    // 초기화 버튼 (골드 소모) 

    /*
        - 공격력 증가 - 1%
        - 처치 시 골드 획득량 증가 10%
        - 게임 클리어시 골드 획득량 증가  5%
        - 미션 클리어시 골드 획득량 증가 5%
        - 소환시 확률로 e급 한 개 추가 획득 1%
    */
    // name - 스킬 이름
    // now_value - 현재 효과
    // next_value - 다음 효과
    // level - 스킬 레벨
    // button - 강화 가능하면 점멸
    // 비용은 업그레이드 버튼 누른 이후 표기

    [SerializeField] GameObject skill_ui_field;
    [SerializeField] TMP_Text left_skill_text;

    private Color btn_off_color = new(1f,1f,1f, 30/255f);
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Init_Checker());
    }

    IEnumerator Init_Checker() {
        while(true) {
            if(db_state == DB_STATE.Usable) {
                break;
            }
            yield return wffu;
        }
        user.SkillPointChecker();
        Skill_Init();
    }

    public void Content_Init() {
        foreach(Transform t in skill_ui_field.transform) {
            Destroy(t.gameObject);
        }
    }

    public void Skill_Init() {
        // 스킬 5개 뿐이므로 항상 전부 재시도
        Content_Init();

        foreach(SkillType skill in Enum.GetValues(typeof(SkillType))) {

            string name = skill switch {
                SkillType.AttackIncrease => "공격력 증가",
                SkillType.GainGold => "몬스터 처치 시 골드 획득량 증가",
                SkillType.ClearGold => "게임 클리어 시 골드 획득량 증가",
                SkillType.MissionGold => "미션 클리어 시 골드 획득량 증가",
                SkillType.ExtraSummon => "소환 시 확률로 E급 추가 획득",
                _ => "__"
            };

            int now_value = user.GetSkillValue(skill);
            int next_value = user.GetSkillValueNext(skill);
            int level = user.GetSkillLevel(skill);

            //Debug.Log($"{name} =>{level} >>> {now_value}  / {next_value} ");

            // generate prefab 
            GameObject instance = Instantiate(skill_card_prefab);
            instance.transform.SetParent(skill_ui_field.transform, false);

            instance.transform.Find("name").GetComponent<TMP_Text>().text = name;
            instance.transform.Find("now_value").GetComponent<TMP_Text>().text = now_value.ToString();
            instance.transform.Find("next_value").GetComponent<TMP_Text>().text = next_value.ToString();
            instance.transform.Find("level").GetComponent<TMP_Text>().text = level.ToString();

            instance.transform.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();

            if(user.left_skill_point > 0) {
                instance.transform.Find("Button").GetComponent<Image>().color = Color.white;
                // btn에 이벤트 삽입
                instance.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => {
                    Home_Skill_Upgrade_Setter.Instance.Setting(skill);
                });
            }
            else {
                instance.transform.Find("Button").GetComponent<Image>().color = btn_off_color;
            }
        }

        // 남은 스킬 포인트 보여주기
        left_skill_text.text = user.left_skill_point.ToString();
    }

}
