using System;
using System.Text;
using CUSTOM_DATA;

public class User {

    private readonly int SKILL_MAX = 50;

    public int level;
    private int experience;
    private int max_experience;
    public int gold;
    public int skill_point;
    public int left_skill_point;

    private int[] invested_skill_point = new int[5] { 0, 0, 0, 0, 0 };
    /*
    - 공격력 증가 - 1%
    - 처치 시 골드 획득량 증가 10%
    - 게임 클리어시 골드 획득량 증가  5%
    - 미션 클리어시 골드 획득량 증가 5%
    - 소환시 확률로 e급 한 개 추가 획득 1%
*/

    public int MaxExperience => max_experience;

    // Skill 관련 메서드 추가
    public int GetSkillLevel(SkillType skill) {
        return invested_skill_point[(int)skill];
    }

    public int GetSkillValue(SkillType skill) {
        int value = skill switch {
            SkillType.AttackIncrease => 1,
            SkillType.GainGold => 10,
            SkillType.ClearGold => 5,
            SkillType.MissionGold => 5,
            SkillType.ExtraSummon => 1,
            _ => 1
        };

        return invested_skill_point[(int)skill] * value;
    }

    public int GetSkillValueNext(SkillType skill) {

        int value = skill switch {
            SkillType.AttackIncrease => 1,
            SkillType.GainGold => 10,
            SkillType.ClearGold => 5,
            SkillType.MissionGold => 5,
            SkillType.ExtraSummon => 1,
            _ => 1
        };

        return invested_skill_point[(int)skill] >= SKILL_MAX ? SKILL_MAX * value : (invested_skill_point[(int)skill] + 1) * value;
    }


    public int Experience {
        get => experience;
        set {
            experience = value;
            if (experience >= max_experience) {
                level++;
                skill_point++;
                left_skill_point++;
                experience -= max_experience;
                UpdateUser($"experience={experience}, level={level}, skill_point={skill_point}, left_skill_point={left_skill_point}");
            }
            else {
                UpdateUser($"experience={experience}");
            }
        }
    }

    public int Gold {
        get => gold;
        set {
            gold = value;
            UpdateUser($"gold={gold}");
        }
    }

    public bool AddSkillPoint(SkillType skill) {
        if (left_skill_point <= 0)
            return false; // 스킬 포인트가 없으면 실패

        int skill_index = (int)skill;

        // 만약 스킬이 최대치(50)라면 찍을 수 없음
        if (invested_skill_point[skill_index] >= SKILL_MAX)
            return false;

        invested_skill_point[skill_index] += 1;
        left_skill_point -= 1;

        UpdateUser($"invested_skill_point='{string.Join(",", invested_skill_point)}'");
        UpdateUser($"skill_point={skill_point}, left_skill_point={left_skill_point}");

        return true; // 성공
    }

    public void Init(int _level, int _experience, int _gold, int _skill_point, int _left_skill_point, int[] _invested) {
        level = _level;
        experience = _experience;
        gold = _gold;
        skill_point = _skill_point;
        left_skill_point = _left_skill_point;
        invested_skill_point = _invested;

        max_experience = level * 20 + 100;
    }

    public void SkillPointChecker() {

        // checking
        var now_using_point = 0;
        foreach (int points in invested_skill_point) {
            now_using_point += points;
        }

        bool is_skill_point_valid = (now_using_point + left_skill_point) == skill_point;

        if (!is_skill_point_valid) {
            // 스킬들에 찍혀있는 값을 모두 초기화하여 left로 저장할 것
            for (int i = 0; i < invested_skill_point.Length; i++) {
                invested_skill_point[i] = 0;
            }

            left_skill_point = skill_point;

            UpdateUser($"invested_skill_point='{string.Join(",", invested_skill_point)}'");
            UpdateUser($"skill_point={skill_point}, left_skill_point={left_skill_point}");
        }

    }

    private void UpdateUser(string where_str) {
        StringBuilder builder = Utility.builder;
        builder.Clear();
        builder.Append("UPDATE user SET ").Append(where_str);
        ModifyDB.Instance.ModifySet(builder.ToString(), "user");
        builder.Clear();
    }
}