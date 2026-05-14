//using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace CUSTOM_DATA {

    public static class Game_State_Data {
        public static DB_STATE db_state = DB_STATE.Before;
        public static SceneType now_scene = SceneType.Home;

        public static bool is_click_unit = false;
        public static bool is_menu = true;
        public static bool is_gaming = false;
        public static int difficulty = 1;
        public static int game_speed = 1;
        public static bool same_day_checker = false;
        public static StaminaClass stamina_storage;

        private static int round_counter = 0;
        private static int round_max = 100;
        public static int Round_counter { 
            set { 
                round_counter = value;
                if(is_gaming)
                    Round_Checker.Instance.Observing();
            }
            get { return round_counter; }
        }
        public static int ROUND_MAX { get { return round_max; } }

        public static int round_mob_counter = 40;

        private static Plannet now_plannet = Plannet.Gray;
        public static Plannet Now_plannet {
            get { return now_plannet; }
            set {
                now_plannet = value;
                round_max = value switch {
                    Plannet.Green => 100,
                    Plannet.Blue => 120,
                    Plannet.Gray => 140,
                    Plannet.Ancient => 160,
                    _ => 100
                };
            }
        }
        public static bool data_setting_complete = false;

        public static void State_Init() {
            is_gaming = false;
            difficulty = 1;
            game_speed = 1;
            round_counter = 0;
            now_plannet = Plannet.non;
        }
    }

    public static class Home_UI_Controller {
        public static bool page_loading = false;
        public static int buy_energy_value = 0;
        public static int now_info_unit_id = 0;
    }

    public enum Chest_Type { D, C, B, A, S, None }
    public enum Shop_Result_Type { Energy_Buy, Chest_Buy, Piece_Buy, Piece_Sell }


    public static class Game_Data {

        public static Dictionary<int, Unit> unit;
        public static int piece_count = 0;
        public static int stone_count = 0;
        public static User user;
        // enemy
        public static Dictionary<int, Enemy> enemy;
        public static Dictionary<int, Installed_Tower> installed_tower = new();
        // spell
        public static Dictionary<OraType, Spell> stored_spell = new() {
            { OraType.Ice , new() },
            { OraType.Fire , new() },
            { OraType.Poison , new() },
            { OraType.Water , new() },
        };
        // achievement
        public static Dictionary<int, Achievement> achievement;
        public static Dictionary<int, Quest> daily_quest;
        public static Dictionary<int, Quest> weekly_quest;
        // goods
        public static float dot = 200;
        public static int upgrade_value_d = 0;
        public static int upgrade_value_c = 0;
        public static int upgrade_value_b = 0;
        public static int upgrade_value_a = 0;
        public static int upgrade_value_s = 0;
        public static int upgrade_value_ex = 0;


        /// <summary>
        /// key : unit_id 
        /// value : counter
        /// </summary>
        public static Dictionary<int, Unit_Count_Checker> unit_counter;

        // 게임 시작과 종료시 초기화해야함
        public static void Data_Init() {
            piece_count = 0;
            stone_count = 0;
            dot = 200;

            upgrade_value_d = 0;
            upgrade_value_c = 0;
            upgrade_value_b = 0;
            upgrade_value_a = 0;
            upgrade_value_s = 0;
            upgrade_value_ex = 0;

            Init_Counter();

            installed_tower.Clear();

            stored_spell[OraType.Ice] = new();
            stored_spell[OraType.Fire] = new();
            stored_spell[OraType.Poison] = new();
            stored_spell[OraType.Water] = new();
        }

        public static void Init_Counter() {
            // unit_counter 초기화
            foreach (var item in unit_counter) {
                item.Value.count = 0;
                item.Value.is_field = false;
            }
        }

        public static void Set_dot(float _dot) {
            dot = _dot;
            if (dot >= 1000) Achievement_Observer.Instance.Possess_Dot_Achievement();
            if (Game_State_Data.is_gaming || (Game_State_Data.game_speed == 0 && !Game_State_Data.is_gaming)) {
                In_Game_Setter.Instance.Set_Goods();
            }
        }
    }
    public enum OraType {
        None = 0,
        Ice = 1,
        Fire = 2,
        Poison = 3,
        Water = 4
    }

    public class Spell {
        public int count = 0;
        public int all_count = 0;
        public int level = 1;
        public readonly int MAX_LEVEL = 4;
        public int need_piece = 10;

        public void Level_Up() {
            level += 1;
            need_piece = (level + 1) * 5;
            if (level >= 4) {
                level = 4;
                need_piece = 99999999;
            }
        }

        public void Init() {
            count = 0;
            level = 1;
            need_piece = 5;
        }

        public string Get_level() {
            return level switch {
                1 => "4등급",
                2 => "3등급",
                3 => "2등급",
                4 => "1등급",
                _ => "등급외"
            };
        }
    }

    public class Unit_Count_Checker {
        public int count = 0;
        public bool is_field = false;
        public Unit_Count_Checker(int count, bool is_field) {
            this.count = count;
            this.is_field = is_field;
        }
    }

    public class Installed_Tower {
        public int tower_id;
        public float attack;
        public int material;
        public int ethereal;
        public GameObject me;
        public Vector3 pos;

        public Installed_Tower(int _id, float _attack, int _material, int _ethereal, GameObject _me, Vector3 pos) {
            tower_id = _id;
            attack = _attack;
            material = _material;
            ethereal = _ethereal;
            me = _me;
            this.pos = pos;
        }
    }

    public class Damage {
        public float material;
        public float ethereal;
        public OraType type;
    }

    public enum SkillType {
        AttackIncrease,    // 공격력 증가
        GainGold,          // 처치 시 골드 획득량 증가
        ClearGold,         // 게임 클리어 시 골드 획득량 증가
        MissionGold,       // 미션 클리어 시 골드 획득량 증가
        ExtraSummon        // 소환 시 추가 획득 확률 증가
    }

    public enum Enemy_Type {
        Original,
        Shield,
        Bust,
        Reinforced_Original,
        Reinforced_Shield,
        Reinforced_Bust,
        Boss_Original,
        Boss_Shield,
        Boss_Bust,
    }

    public enum DB_STATE {
        Before,
        Connecting,
        Connect,
        Usable,
        Loading,
        Modifying,
    }

    public enum UIPanelID {
        None,
        SummonButtonArea,
        InstallArea,
        ButtonArea,
        CombineArea,
        UnitInfoArea,
        UpgradeArea,
        FieldUnitClickArea,
        AnnounceArea,
        RoundCheckArea,
        CombineBookArea,
        MissionArea,
        SpellArea,
    }

    public enum Plannet {
        non,
        Green,
        Blue,
        Gray,
        Ancient,
    }
   
    public enum SceneType {
        home,
        Home,
        Achievement,
        Plannet,
        Shop,
        Unit,
    }

    public static class Game_Value_Data {
#if UNITY_ANDROID
        public static readonly string DB_CONNECT_STRING = "Data Source=" + Application.persistentDataPath + "/ShapeKeeperDB.db" + ";Version=3;";
#else
        public static readonly string DB_CONNECT_STRING = "URI=file:" + Application.persistentDataPath + "/ShapeKeeperDB.db";  
#endif
        public static readonly string ANI_ACTIVATE = "Activate";
        public static readonly string ANI_Passively_ACTIVATE = "Passively_Activate";

        public static readonly float Bullet_Speed = 0.03f;

        // prefab
        public static readonly GameObject d_prefab = Resources.Load<GameObject>("03.Prefab/Tower/Origin/D_Tower_prefab");
        public static readonly GameObject c_prefab = Resources.Load<GameObject>("03.Prefab/Tower/Origin/C_Tower_prefab");
        public static readonly GameObject b_prefab = Resources.Load<GameObject>("03.Prefab/Tower/Origin/B_Tower_prefab");
        public static readonly GameObject a_prefab = Resources.Load<GameObject>("03.Prefab/Tower/Origin/A_Tower_prefab");
        public static readonly GameObject s_prefab = Resources.Load<GameObject>("03.Prefab/Tower/Origin/S_Tower_prefab");
        public static readonly GameObject ex_prefab = Resources.Load<GameObject>("03.Prefab/Tower/Origin/EX_Tower_prefab");

        public static readonly GameObject d_translucent_prefab = Resources.Load<GameObject>("03.Prefab/Tower/Translucent/D_Tower_prefab");
        public static readonly GameObject c_translucent_prefab = Resources.Load<GameObject>("03.Prefab/Tower/Translucent/C_Tower_prefab");
        public static readonly GameObject b_translucent_prefab = Resources.Load<GameObject>("03.Prefab/Tower/Translucent/B_Tower_prefab");
        public static readonly GameObject a_translucent_prefab = Resources.Load<GameObject>("03.Prefab/Tower/Translucent/A_Tower_prefab");
        public static readonly GameObject s_translucent_prefab = Resources.Load<GameObject>("03.Prefab/Tower/Translucent/S_Tower_prefab");
        public static readonly GameObject ex_translucent_prefab = Resources.Load<GameObject>("03.Prefab/Tower/Translucent/EX_Tower_prefab");

        public static readonly Dictionary<string, GameObject> prefab_container = new() {
            { "d", d_prefab },
            { "c", c_prefab },
            { "b", b_prefab },
            { "a", a_prefab },
            { "s", s_prefab },
            { "ex", ex_prefab }
        };

        public static readonly Dictionary<string, GameObject> translucent_container = new() {
            { "d", d_translucent_prefab },
            { "c", c_translucent_prefab },
            { "b", b_translucent_prefab },
            { "a", a_translucent_prefab },
            { "s", s_translucent_prefab },
            { "ex", ex_translucent_prefab }
        };

        public static readonly GameObject bullet_prefab = Resources.Load<GameObject>("03.Prefab/Tower/Bullet/Bullet_prefab");

        // UI prefab
        public static readonly GameObject attack_area_prefab = Resources.Load<GameObject>("03.Prefab/Tower/Attack Area/Attack_Area");
        public static readonly GameObject lock_on_prefab = Resources.Load<GameObject>("03.Prefab/Tower/Attack Area/Lock_On_Area");
        public static readonly GameObject ora_prefab = Resources.Load<GameObject>("03.Prefab/Tower/Gift/Ora");

        //effect prefab
        public static readonly GameObject ice_effect = Resources.Load<GameObject>("03.Prefab/Tower/Gift/Ice");
        public static readonly GameObject fire_effect = Resources.Load<GameObject>("03.Prefab/Tower/Gift/Fire");
        public static readonly GameObject poison_effect = Resources.Load<GameObject>("03.Prefab/Tower/Gift/Poison");
        public static readonly GameObject water_effect = Resources.Load<GameObject>("03.Prefab/Tower/Gift/Water");

        // combine prefab
        public static readonly GameObject combine_function_title_prefab = Resources.Load<GameObject>("03.Prefab/Combine/Combine_Title");
        public static readonly GameObject combine_function_prefab = Resources.Load<GameObject>("03.Prefab/Combine/Combine_Function");
        public static readonly GameObject combine_function_prefab_2 = Resources.Load<GameObject>("03.Prefab/Combine/Combine_Function_2");
        public static readonly GameObject combine_function_prefab_3 = Resources.Load<GameObject>("03.Prefab/Combine/Combine_Function_3");
        public static readonly GameObject combine_function_prefab_4 = Resources.Load<GameObject>("03.Prefab/Combine/Combine_Function_4");
        public static readonly GameObject combine_function_prefab_5 = Resources.Load<GameObject>("03.Prefab/Combine/Combine_Function_5");

        // UI prefab
        public static readonly GameObject unit_card_prefab = Resources.Load<GameObject>("03.Prefab/UI/Unit");
        public static readonly GameObject unit_result_prefab = Resources.Load<GameObject>("03.Prefab/UI/Result_Unit");
        public static readonly GameObject goal_prefab = Resources.Load<GameObject>("03.Prefab/UI/Goal");
        public static readonly GameObject skill_card_prefab = Resources.Load<GameObject>("03.Prefab/UI/Skill_UI");

        public static GameObject load_map_prefab;
        public static GameObject show_map_prefab;
        public static GameObject back_map_prefab;

        public static GameObject green_resistance_show_prefab = Resources.Load<GameObject>("03.Prefab/UI/Green_Resis");

        //Enemy

        // slime
        public static GameObject slime_prefab_ori = Resources.Load<GameObject>("03.Prefab/Slime Enemy/Green Idle");
        public static GameObject slime_prefab_shield = Resources.Load<GameObject>("03.Prefab/Slime Enemy/Blue Idle");
        public static GameObject slime_prefab_bust = Resources.Load<GameObject>("03.Prefab/Slime Enemy/Red Idle");

        public static GameObject slime_prefab_boss_ori = Resources.Load<GameObject>("03.Prefab/Slime Enemy/Green_Boss");
        public static GameObject slime_prefab_boss_shield = Resources.Load<GameObject>("03.Prefab/Slime Enemy/Blue_Boss");
        public static GameObject slime_prefab_boss_bust = Resources.Load<GameObject>("03.Prefab/Slime Enemy/Red_Boss");

        // wisp
        public static GameObject wisp_prefab_ori = Resources.Load<GameObject>("03.Prefab/Wisp/Normal_Wisp");
        public static GameObject wisp_prefab_shield = Resources.Load<GameObject>("03.Prefab/Wisp/Shield_Wisp");
        public static GameObject wisp_prefab_bust = Resources.Load<GameObject>("03.Prefab/Wisp/Bust_Wisp");

        public static GameObject wisp_prefab_boss_ori = Resources.Load<GameObject>("03.Prefab/Wisp/Normal_Wisp_Boss");
        public static GameObject wisp_prefab_boss_shield = Resources.Load<GameObject>("03.Prefab/Wisp/Shield_Wisp_Boss");
        public static GameObject wisp_prefab_boss_bust = Resources.Load<GameObject>("03.Prefab/Wisp/Bust_Wisp_Boss");

        // core
        public static GameObject core_prefab_ori = Resources.Load<GameObject>("03.Prefab/Core/Normal_Core");
        public static GameObject core_prefab_shield = Resources.Load<GameObject>("03.Prefab/Core/Shield_Core");
        public static GameObject core_prefab_bust = Resources.Load<GameObject>("03.Prefab/Core/Bust_Core");

        public static GameObject core_prefab_boss_ori = Resources.Load<GameObject>("03.Prefab/Core/Normal_Core_Boss");
        public static GameObject core_prefab_boss_shield = Resources.Load<GameObject>("03.Prefab/Core/Shield_Core_Boss");
        public static GameObject core_prefab_boss_bust = Resources.Load<GameObject>("03.Prefab/Core/Bust_Core_Boss");

        // ancient core
        public static GameObject ancient_core_prefab_ori = Resources.Load<GameObject>("03.Prefab/Ancient_Core/Normal_Core");
        public static GameObject ancient_core_prefab_shield = Resources.Load<GameObject>("03.Prefab/Ancient_Core/Shield_Core");
        public static GameObject ancient_core_prefab_bust = Resources.Load<GameObject>("03.Prefab/Ancient_Core/Bust_Core");

        public static GameObject ancient_core_prefab_boss_ori = Resources.Load<GameObject>("03.Prefab/Ancient_Core/Normal_Core_Boss");
        public static GameObject ancient_core_prefab_boss_shield = Resources.Load<GameObject>("03.Prefab/Ancient_Core/Shield_Core_Boss");
        public static GameObject ancient_core_prefab_boss_bust = Resources.Load<GameObject>("03.Prefab/Ancient_Core/Bust_Core_Boss");

        // mission
        public static GameObject mission_boss_1 = Resources.Load<GameObject>("03.Prefab/Golem/Mission_1");
        public static GameObject mission_boss_2 = Resources.Load<GameObject>("03.Prefab/Golem/Mission_2");
        public static GameObject mission_boss_3 = Resources.Load<GameObject>("03.Prefab/Golem/Mission_3");
        public static GameObject mission_boss_4 = Resources.Load<GameObject>("03.Prefab/Golem/Mission_4");
        public static GameObject mission_boss_5 = Resources.Load<GameObject>("03.Prefab/Golem/Mission_5");

        public static readonly int UPGRADE_MAX_VALUE = 15;

        public static readonly int D_UPGRADE_FIGURE = 10;
        public static readonly int C_UPGRADE_FIGURE = 20;
        public static readonly int B_UPGRADE_FIGURE = 50;
        public static readonly int A_UPGRADE_FIGURE = 120;
        public static readonly int S_UPGRADE_FIGURE = 400;
        public static readonly int EX_UPGRADE_FIGURE = 1200;

        public static readonly int D_UPGRADE_PIECE_COEFFICENT = 10;
        public static readonly int C_UPGRADE_PIECE_COEFFICENT = 15;
        public static readonly int B_UPGRADE_PIECE_COEFFICENT = 20;
        public static readonly int A_UPGRADE_PIECE_COEFFICENT = 30;
        public static readonly int S_UPGRADE_PIECE_COEFFICENT = 40;
        public static readonly int EX_UPGRADE_PIECE_COEFFICENT = 50;

        public static readonly int D_UPGRADE_GOLD_COEFFICENT = 100;
        public static readonly int C_UPGRADE_GOLD_COEFFICENT = 200;
        public static readonly int B_UPGRADE_GOLD_COEFFICENT = 500;
        public static readonly int A_UPGRADE_GOLD_COEFFICENT = 1000;
        public static readonly int S_UPGRADE_GOLD_COEFFICENT = 1500;
        public static readonly int EX_UPGRADE_GOLD_COEFFICENT = 3000;

        public static readonly int SKILL_ATTACK_INCREASE_VALUE = 1;
        public static readonly int SKILL_GAIN_ENEMY_DOT = 10;
        public static readonly int SKILL_GAIN_CLEAR_DOT = 5;
        public static readonly int SKILL_GAIN_MISSION_DOT = 5;
        public static readonly int SKILL_SUMMON_ADDITIONAL_E = 1;

        public static readonly int SKILL_MAX_POINT = 50;
    }


    public static class Common_Data{
        public static WaitForSeconds wfs_0_1 = new(.1f);
        public static WaitForSeconds wfs_0_12 = new(.12f);
        public static WaitForSeconds wfs_0_18 = new(.18f);
        public static WaitForSeconds wfs_0_2 = new(.2f);
        public static WaitForSeconds wfs_0_25 = new(.25f);
        public static WaitForSeconds wfs_0_3 = new(.3f);
        public static WaitForSeconds wfs_0_4 = new(.4f);
        public static WaitForSeconds wfs_0_5 = new(.5f);
        public static WaitForSeconds wfs_1 = new(1);
        public static WaitForSeconds wfs_1_5 = new(1.5f);
        public static WaitForSeconds wfs_2 = new(2);
        public static WaitForSeconds wfs_5 = new(5);
        public static WaitForSeconds wfs_10 = new(10);
        public static WaitForSeconds wfs_30 = new(30);
        public static WaitForSeconds wfs_60 = new(60);

        public static WaitForFixedUpdate wffu = new();
        public static WaitForEndOfFrame wfef = new ();

        public static Color material_color = new(1f, 185 / 255f, 185 / 255f, 1f);
        public static Color hybrid_color = new(233/ 255f, 192 / 255f, 1f, 1f);
        public static Color ethereal_color = new(185 / 255f, 245 / 255f, 1f, 1f);

        public static Color special_material_color = new(1f, 1f, 180 / 255f, 1f);
        public static Color special_hybrid_color = new(1f, 198 / 255f, 180 / 255f, 1f);
        public static Color special_ethereal_color = new(110 / 255f, 1f, 200 / 255f, 1f);

        public static Color background_off_color = new(1f, 1f, 1f, 0f);

        public static Color GREEN_THEME_COLOR = new(186 / 255f / 247 / 255f, 1f, 1f);

        public static readonly Color color_r = new(85 / 255f, 184 / 255f, 1f, 1);
        public static readonly Color color_e = new(1f, 1f, 1f, 1);
        public static readonly Color color_d = new(204 / 255f, 209 / 255f, 48 / 255f, 1);
        public static readonly Color color_c = new(28 / 255f, 195 / 255f, 0, 1);
        public static readonly Color color_b = new(50 / 255f, 117 / 255f, 215 / 255f, 1);
        public static readonly Color color_a = new(151 / 255f, 120 / 255f, 238 / 255f, 1);
        public static readonly Color color_s = new(255 / 255f, 170 / 255f, 83 / 255f, 1);
        public static readonly Color color_ex = new(224 / 255f, 83 / 255f, 90 / 255f, 1);

        // spell color
        public static readonly Color ice_color = new(0, 1f, 234 / 255f, 1f);
        public static readonly Color fire_color = new(255 / 255f, 67 / 255f, 7 / 255f, 1);
        public static readonly Color poison_color = new(78 / 255f, 193 / 255f, 113 / 255f, 1);
        public static readonly Color water_color = new(34 / 255f, 185 / 255f, 1f, 1f);

        // spell string
        public static readonly string ICE_TEXT = "ice";
        public static readonly string FIRE_TEXT = "fire";
        public static readonly string POISON_TEXT = "poison";
        public static readonly string WATER_TEXT = "water";

        // spell sprite
        public static readonly Sprite ice_1 = Resources.Load<Sprite>("04.Sprite/Icon/Spell/ice_1");
        public static readonly Sprite ice_2 = Resources.Load<Sprite>("04.Sprite/Icon/Spell/ice_2");
        public static readonly Sprite ice_3 = Resources.Load<Sprite>("04.Sprite/Icon/Spell/ice_3");
        public static readonly Sprite ice_4 = Resources.Load<Sprite>("04.Sprite/Icon/Spell/ice_4");

        public static readonly Sprite fire_1 = Resources.Load<Sprite>("04.Sprite/Icon/Spell/fire_1");
        public static readonly Sprite fire_2 = Resources.Load<Sprite>("04.Sprite/Icon/Spell/fire_2");
        public static readonly Sprite fire_3 = Resources.Load<Sprite>("04.Sprite/Icon/Spell/fire_3");
        public static readonly Sprite fire_4 = Resources.Load<Sprite>("04.Sprite/Icon/Spell/fire_4");

        public static readonly Sprite poison_1 = Resources.Load<Sprite>("04.Sprite/Icon/Spell/poison_1");
        public static readonly Sprite poison_2 = Resources.Load<Sprite>("04.Sprite/Icon/Spell/poison_2");
        public static readonly Sprite poison_3 = Resources.Load<Sprite>("04.Sprite/Icon/Spell/poison_3");
        public static readonly Sprite poison_4 = Resources.Load<Sprite>("04.Sprite/Icon/Spell/poison_4");

        public static readonly Sprite water_1 = Resources.Load<Sprite>("04.Sprite/Icon/Spell/water_1");
        public static readonly Sprite water_2 = Resources.Load<Sprite>("04.Sprite/Icon/Spell/water_2");
        public static readonly Sprite water_3 = Resources.Load<Sprite>("04.Sprite/Icon/Spell/water_3");
        public static readonly Sprite water_4 = Resources.Load<Sprite>("04.Sprite/Icon/Spell/water_4");

        public static readonly Sprite ice_ora = Resources.Load<Sprite>("04.Sprite/Icon/Spell/ice_ora");
        public static readonly Sprite fire_ora = Resources.Load<Sprite>("04.Sprite/Icon/Spell/fire_ora");
        public static readonly Sprite poison_ora = Resources.Load<Sprite>("04.Sprite/Icon/Spell/poison_ora");
        public static readonly Sprite water_ora = Resources.Load<Sprite>("04.Sprite/Icon/Spell/water_ora");

        // home ui color
        public static readonly Color enter_select = new(190 / 255f, 255 / 255f, 168 / 255f, 1);
        public static readonly Color enter_await = new(255 / 255f, 203 / 255f, 162 / 255f, 1);

        // shop ui
        public static readonly Sprite battery_sprite = Resources.Load<Sprite>("04.Sprite/Icon/battery");
        public static readonly Sprite chest_sprite = Resources.Load<Sprite>("04.Sprite/Icon/Chest/chest_icon");
        public static readonly GameObject chest_prefab = Resources.Load<GameObject>("03.Prefab/UI/Chest_Buy_Result");

        public static readonly Sprite d_circle = Resources.Load<Sprite>("04.Sprite/Tower/d_circle");
        public static readonly Sprite d_triangle = Resources.Load<Sprite>("04.Sprite/Tower/d_triangle");
        public static readonly Sprite d_square = Resources.Load<Sprite>("04.Sprite/Tower/d_square");
        public static readonly Sprite d_star = Resources.Load<Sprite>("04.Sprite/Tower/d_star");
        public static readonly Sprite d_moon = Resources.Load<Sprite>("04.Sprite/Tower/d_moon");

        public static readonly Sprite b_circle = Resources.Load<Sprite>("04.Sprite/Tower/b_circle");
        public static readonly Sprite b_triangle = Resources.Load<Sprite>("04.Sprite/Tower/b_triangle");
        public static readonly Sprite b_square = Resources.Load<Sprite>("04.Sprite/Tower/b_square");
        public static readonly Sprite b_star = Resources.Load<Sprite>("04.Sprite/Tower/b_star");
        public static readonly Sprite b_moon = Resources.Load<Sprite>("04.Sprite/Tower/b_moon");

        public static readonly Sprite piece_sprite = Resources.Load<Sprite>("04.Sprite/Tower/piece");
        public static readonly Sprite stone_sprite = Resources.Load<Sprite>("04.Sprite/Tower/stone");

        // achieve ui
        public static GameObject achieve_prefab = Resources.Load<GameObject>("03.Prefab/UI/Achieve");
        public static Sprite achieve_can_recive_icon = Resources.Load<Sprite>("04.Sprite/Icon/Achivement_Icon_2");
        public static Sprite achieve_normal_icon = Resources.Load<Sprite>("04.Sprite/Icon/Achivement_Icon");

        // icon
        public static readonly Sprite[] bright_back_icon = Resources.LoadAll<Sprite>("04.Sprite/Icon/BrightBackground");
        public static readonly Sprite[] white_icon = Resources.LoadAll<Sprite>("04.Sprite/Icon/BrightIcons");

        // Speed icon
        public static readonly Sprite speed_normal = bright_back_icon[28];
        public static readonly Sprite speed_stop = bright_back_icon[27];
        public static readonly Sprite speed_bust = bright_back_icon[40];

        // mission icon
        public static readonly Sprite mission_carry_out_possible_icon = bright_back_icon[46];
        public static readonly Sprite mission_carry_out_impossible_icon = bright_back_icon[45];

        // sell unit icon
        public static readonly Sprite sell_default_icon = white_icon[13];

        //unit field icon
        public static readonly Sprite unit_field_icon_0 = Resources.Load<Sprite>("04.Sprite/Icon/field_icon_1");
        public static readonly Sprite unit_field_icon_2v = Resources.Load<Sprite>("04.Sprite/Icon/field_icon_2v");
        public static readonly Sprite unit_field_icon_2h = Resources.Load<Sprite>("04.Sprite/Icon/field_icon_2h");
        public static readonly Sprite unit_field_icon_4 = Resources.Load<Sprite>("04.Sprite/Icon/field_icon_4");
        public static readonly Sprite unit_field_icon_6v = Resources.Load<Sprite>("04.Sprite/Icon/field_icon_6v");
        public static readonly Sprite unit_field_icon_6h = Resources.Load<Sprite>("04.Sprite/Icon/field_icon_6h");

        public static Tilemap load_map;
        public static Tilemap show_map;

        public static TileBase tower_base_left_up = Resources.Load<RuleTile>("05.Tile/Tower Base/Tower_left_up");
        public static TileBase tower_base_center_up = Resources.Load<RuleTile>("05.Tile/Tower Base/Tower_center_up");
        public static TileBase tower_base_right_up = Resources.Load<RuleTile>("05.Tile/Tower Base/Tower_right_up");

        public static TileBase tower_base_left_center = Resources.Load<RuleTile>("05.Tile/Tower Base/Tower_left_center");
        public static TileBase tower_base_right_center = Resources.Load<RuleTile>("05.Tile/Tower Base/Tower_right_center");

        public static TileBase tower_base_left_down = Resources.Load<RuleTile>("05.Tile/Tower Base/Tower_left_down");
        public static TileBase tower_base_center_down = Resources.Load<RuleTile>("05.Tile/Tower Base/Tower_center_down");
        public static TileBase tower_base_right_down = Resources.Load<RuleTile>("05.Tile/Tower Base/Tower_right_down");

        public static TileBase tower_base_vertical_up = Resources.Load<RuleTile>("05.Tile/Tower Base/Tower_vertical_up");
        public static TileBase tower_base_vertical_center = Resources.Load<RuleTile>("05.Tile/Tower Base/Tower_vertical_center");
        public static TileBase tower_base_vertical_down = Resources.Load<RuleTile>("05.Tile/Tower Base/Tower_vertical_down");

        public static TileBase tower_base_horizontal_left = Resources.Load<RuleTile>("05.Tile/Tower Base/Tower_horizontal_left");
        public static TileBase tower_base_horizontal_center = Resources.Load<RuleTile>("05.Tile/Tower Base/Tower_horizontal_center");
        public static TileBase tower_base_horizontal_right = Resources.Load<RuleTile>("05.Tile/Tower Base/Tower_horizontal_right");

        public static TileBase base_camp_tile = Resources.Load<RuleTile>("05.Tile/Map/79");

        public static TileBase map_base_green_tile = Resources.Load<RuleTile>("05.Tile/Map/54-0");
        public static TileBase map_base_blue_tile = Resources.Load<RuleTile>("05.Tile/Map/71");
        public static TileBase map_base_gray_tile = Resources.Load<RuleTile>("05.Tile/Map/801");
        public static TileBase map_base_ancient_tile = Resources.Load<RuleTile>("05.Tile/Map/72");

        public static TileBase load_map_back_tile = Resources.Load<RuleTile>("05.Tile/Map/74");

        public static TileBase green_show_map_back_tile = Resources.Load<RuleTile>("05.Tile/Map/55-0");
        public static TileBase blue_show_map_back_tile = Resources.Load<RuleTile>("05.Tile/Map/69");
        public static TileBase gray_show_map_back_tile = Resources.Load<RuleTile>("05.Tile/Map/75");
        public static TileBase ancient_show_map_back_tile = Resources.Load<RuleTile>("05.Tile/Map/70");

        public static readonly int D_start = 2001;
        public static readonly int D_end = 2009;
        public static readonly int D_end_1 = 2010;

        public static readonly int C_start = 3001;
        public static readonly int C_end = 3011;
        public static readonly int C_end_1 = 3012;

        public static readonly int B_start = 4001;
        public static readonly int B_end = 4011;
        public static readonly int B_end_1 = 4012;

        public static readonly int A_start = 5001;
        public static readonly int A_end = 5011;
        public static readonly int A_end_1 = 5012;

        public static readonly int S_start = 6001;
        public static readonly int S_end = 6011;
        public static readonly int S_end_1 = 6012;

        public static readonly int EX_start = 7001;
        public static readonly int EX_end = 7005;
        public static readonly int EX_end_1 = 7006;

        public static readonly Vector3 open = new(0, 0, 0);
    }

    public enum MapID {
        load_map = 1,
        show_map,
    }
    
    public class EnemyPathChecker {

        int pos_min_x = 999;
        int pos_max_x = -999;
        int pos_min_y = 999;
        int pos_max_y = -999;

        LoadNode[,] Monster_Load;

        LoadNode start_point, end_point, cnode;
        readonly List<LoadNode> Next = new();
        readonly List<LoadNode> Visited = new();
        public readonly List<LoadNode> Final_way = new();

        public Vector3 start_position;
        private bool has_start_point = false;

        public void Set_start_position(Vector3 start_position) {
            this.start_position = start_position;
            has_start_point = true;
        }

        public void Init() {
            Next.Clear();
            Visited.Clear();
            Final_way.Clear();
            
            // 맵 생성
            foreach (Vector3Int pos in Common_Data.load_map.cellBounds.allPositionsWithin) {
                TileBase tile = Common_Data.load_map.GetTile(pos);
                if (tile != null) {
                    if (pos.x > pos_max_x) pos_max_x = pos.x;
                    if (pos.x < pos_min_x) pos_min_x = pos.x;
                    if (pos.y > pos_max_y) pos_max_y = pos.y;
                    if (pos.y < pos_min_y) pos_min_y = pos.y;
                }
            }

            int x, y;
            if (pos_min_x < 0) x = (pos_min_x * -1) + pos_max_x + 1;
            else x = pos_min_y + pos_max_y + 1;
            if (pos_min_y < 0) y = (pos_min_y * -1) + pos_max_y + 1;
            else y = pos_min_y + pos_max_y + 1;

            //Debug.Log(pos_min_x + "," + pos_max_y + " // " + pos_max_x + "," + pos_min_y + " //// " + x + "," + y);

            Monster_Load = new LoadNode[y, x];

            // 길 노드로 채우기
            Vector3Int check_pos = new(0, 0, 0);
            for (int Y = 0; Y < Monster_Load.GetLength(0); Y++) {
                for (int X = 0; X < Monster_Load.GetLength(1); X++) {
                    int grid_x = pos_min_x + X;
                    int grid_y = pos_max_y - Y;
                    check_pos.x = pos_min_x + X;
                    check_pos.y = pos_max_y - Y;
                    TileBase tile = Common_Data.load_map.GetTile(check_pos);
                    //Debug.Log(tile.name + " > " + grid_x + " / " + grid_y + " == (" + X + ","+ Y + ") /// tile : " + check_pos.x + "," + check_pos.y);
                    bool pass = Is_Passible(tile.name);
                    Monster_Load[Y, X] = new LoadNode(pass , new(grid_x, grid_y), X, Y);
                    //Monster_Load[Y, X] = new LoadNode(true, new(grid_x, grid_y), X, Y);

                    // 마지막 지점 확인 하는 부분
                    if (tile.name.Contains("79")) {
                        end_point = Monster_Load[Y, X];
                    }
                }
            }
        }

        private bool Is_Passible(string name) {
            return Game_State_Data.Now_plannet switch {
                Plannet.Green => name.Contains("54") || name.Contains("55") || name.Contains("79"),
                Plannet.Blue => name.Contains("69") || name.Contains("71") || name.Contains("79"),
                Plannet.Gray => name.Contains("801") || name.Contains("75") || name.Contains("79"),
                Plannet.Ancient => name.Contains("70") || name.Contains("72") || name.Contains("79"),
                _ => name.Contains("54") || name.Contains("55") || name.Contains("79"),
            };
        }

        public bool Check_End(Vector3 obj_transform) {
            // obj transform을 grid pos로 변경해야함
            var obj_pos = Common_Data.load_map.WorldToCell(obj_transform);
            //Debug.Log(end_point.grid_pos.x + "," + end_point.y + " but obj is >> " + obj_transform);
            return (end_point.grid_pos.x == obj_pos.x && end_point.grid_pos.y == obj_pos.y);
        }
        
        // 타워 설치시 경로 재설정
        public void Re_Path_Finding(Vector3 currentPos) {
            Vector3Int currentCell = Common_Data.load_map.WorldToCell(currentPos);
            LoadNode currentNode = null;

            // 현재 위치에 해당하는 노드를 찾음
            for (int y = 0; y < Monster_Load.GetLength(0); y++) {
                for (int x = 0; x < Monster_Load.GetLength(1); x++) {
                    if (Monster_Load[y, x].grid_pos.x == currentCell.x && Monster_Load[y, x].grid_pos.y == currentCell.y) {
                        currentNode = Monster_Load[y, x];
                        break;
                    }
                }
            }

            // 현재 위치가 벽이라면 위로 피해서 경로 설정
            if (currentNode != null && !currentNode.can_go) {
                Vector3Int upCell = new (currentCell.x, currentCell.y + 1, 0);
                TileBase upTile = Common_Data.load_map.GetTile(upCell);

                if (upTile != null) {
                    // 위쪽 노드를 찾아서 갈 수 있다면 이동
                    for (int y = 0; y < Monster_Load.GetLength(0); y++) {
                        for (int x = 0; x < Monster_Load.GetLength(1); x++) {
                            if (Monster_Load[y, x].grid_pos.x == upCell.x && Monster_Load[y, x].grid_pos.y == upCell.y) {
                                if (Monster_Load[y, x].can_go) {
                                    Set_start_position(Common_Data.load_map.CellToWorld(upCell));
                                    Path_Finding();
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            // 현재 위치에서 가장 가까운 Final_way 지점을 찾아서 경로 설정
            if (Final_way.Count > 0) {
                float minDist = float.MaxValue;
                LoadNode closest = null;
                foreach (var node in Final_way) {
                    float dist = Vector2.Distance(currentPos, Common_Data.load_map.CellToWorld(new Vector3Int((int)node.grid_pos.x, (int)node.grid_pos.y, 0)));
                    if (dist < minDist) {
                        minDist = dist;
                        closest = node;
                    }
                }

                if (closest != null) {
                    Set_start_position(Common_Data.load_map.CellToWorld(new Vector3Int((int)closest.grid_pos.x, (int)closest.grid_pos.y, 0)));
                }
                else {
                    Set_start_position(currentPos);
                }
            }
            else {
                Set_start_position(currentPos);
            }

            Path_Finding();
        }

        // 길만들기
        public bool Path_Finding() {
            Init();
            if (has_start_point) {
                // 시작 위치를 몬스터의 위치에 맞게 설정하게 변수화
                // 현재 몬스터의 위치에 있는 tile을 찾기
                // tile의 x y 좌표를 가진 MonsterLoad를 찾아 start_point에 넣어두기
                Vector2 start_pos = new(Common_Data.load_map.WorldToCell(start_position).x, Common_Data.load_map.WorldToCell(start_position).y);
                //Debug.Log(start_pos);
                for (int Y = 0; Y < Monster_Load.GetLength(0); Y++) {
                    bool checker = false;
                    for (int X = 0; X < Monster_Load.GetLength(1); X++) {
                        if (Monster_Load[Y, X].grid_pos.x == start_pos.x && Monster_Load[Y, X].grid_pos.y == start_pos.y) {
                            start_point = Monster_Load[Y, X];
                            checker = true;
                            break;
                        }
                    }
                    if (checker) break;
                }

            }
            else {
                start_point = Monster_Load[0, 0];
            }

            Next.Add(start_point);

            int counter = 0;
            // 길찾기
            while (Next.Count > 0) {
                counter++;
                // 다음으로 진행해볼 node 중 F가 가장작고 / 남은거리(H)가 가장 작은 것을 골라 보기 ( 우선순위를 강제함 )
                cnode = Next[0];
                for (int i = 1; i < Next.Count; i++)
                    if (Next[i].F <= cnode.F && Next[i].H < cnode.H) cnode = Next[i];
                Next.Remove(cnode);
                Visited.Add(cnode);

                // 목적지에 도착했을 경우
                if (cnode == end_point) {

                    Final_way.Clear();
                    // final_way에 삽입 공정
                    LoadNode way_node = end_point;
                    while (way_node != start_point) {
                        Final_way.Add(way_node);
                        way_node = way_node.parent;
                    }
                    Final_way.Add(start_point);
                    Final_way.Reverse();
                    break;
                }

                // 도착하지 않은 경우 현 노드에서 갈 수 있는 모든 길을 갈 수 있는 길 검사하기
                // 8방향 ( 대각선으로도 갈 수 있게 만들예정
                // 상
                Move_Checker(cnode.x, cnode.y - 1, (int)cnode.grid_pos.x, (int)cnode.grid_pos.y + 1);
                // 우상
                Move_Checker(cnode.x + 1, cnode.y - 1, (int)cnode.grid_pos.x + 1, (int)cnode.grid_pos.y + 1);
                // 우
                Move_Checker(cnode.x + 1, cnode.y, (int)cnode.grid_pos.x + 1, (int)cnode.grid_pos.y);
                // 우하
                Move_Checker(cnode.x + 1, cnode.y + 1, (int)cnode.grid_pos.x + 1, (int)cnode.grid_pos.y - 1);
                // 하
                Move_Checker(cnode.x, cnode.y + 1, (int)cnode.grid_pos.x, (int)cnode.grid_pos.y - 1);
                // 좌하
                Move_Checker(cnode.x - 1, cnode.y + 1, (int)cnode.grid_pos.x - 1, (int)cnode.grid_pos.y - 1);
                // 좌
                Move_Checker(cnode.x - 1, cnode.y, (int)cnode.grid_pos.x - 1, (int)cnode.grid_pos.y);
                // 좌상
                Move_Checker(cnode.x - 1, cnode.y - 1, (int)cnode.grid_pos.x - 1, (int)cnode.grid_pos.y + 1);
            }

            //Debug.Log("get output >> " + Final_way.Count);
            return Final_way.Count > 0;
        }

        /// <summary>
        /// 갈 수 있는 길 검사
        /// </summary>
        /// <param name="load_x">Monster load 의 x 좌표</param>
        /// <param name="load_y">Monster load 의 y 좌표</param>
        /// <param name="grid_x">TileMap 의 x 좌표</param>
        /// <param name="grid_y">Tilemap 의 y 좌표</param>
        private void Move_Checker(int load_x, int load_y, int grid_x, int grid_y) {
            if (load_x < 0 || load_x >= Monster_Load.GetLength(1) || load_y < 0 || load_y >= Monster_Load.GetLength(0)) return;

            // 다음에 갈 좌표가 이동 가능한 좌표인지 탐색
            // 확인해야할것 : tilemap 좌표 내, Visted에 없다는 사실, 갈 수 있는 곳인지
            if ((pos_min_x > grid_x || pos_max_x < grid_x || pos_min_y > grid_y || pos_max_y < grid_y) ||
                Visited.Contains(Monster_Load[load_y, load_x]) ||
                !Monster_Load[load_y, load_x].can_go
                ) {
                return;
            }

            // 대각 이동시 이동방향의 좌/우 길이 막혀있으면 지나갈 수 없음
            if (!Monster_Load[load_y, cnode.x].can_go || !Monster_Load[cnode.y, load_x].can_go) return;

            // 현재 위치가 타워 안이면 뒤로 물러나야함

            //Debug.Log("check here");
            // 이동 비용 산정해보기
            // 직선은 10 대각은 14
            LoadNode n_node = Monster_Load[load_y, load_x];
            int cost = cnode.G + (cnode.x - grid_x == 0 || cnode.y - grid_y == 0 ? 10 : 14);

            // 이동하게 될 노드와 비교해 갈 수 있을지 확인해보기
            if (n_node.G > cost || !Next.Contains(n_node)) {
                n_node.G = cost;
                n_node.H = Mathf.Abs(n_node.x - end_point.x) + Mathf.Abs(n_node.y - end_point.y) * 10;
                n_node.parent = cnode;

                Next.Add(n_node);
            }
        }

    }

    public enum Tower_Space {
        _2v,
        _2h,
        _4,
        _6v,
        _6h
    }

    public class TOWER_BASE {
        
        public Tower_Space space;

        public Vector3Int left_up = new();
        public Vector3Int left_down = new ();
        public Vector3Int right_up = new();
        public Vector3Int right_down = new();
        public Vector3Int up = new();
        public Vector3Int down = new();

        public void Set_All_Color(MapID map_type, Color color) {
            switch (map_type) {
                // monster load_map
                case MapID.load_map:
                    Common_Data.load_map.SetColor(left_up, color);
                    Common_Data.load_map.SetColor(left_down, color);
                    Common_Data.load_map.SetColor(right_up, color);
                    Common_Data.load_map.SetColor(right_down, color);
                    Common_Data.load_map.SetColor(up, color);
                    Common_Data.load_map.SetColor(down, color);
                    break;

                // player show map
                case MapID.show_map:
                    Common_Data.show_map.SetColor(left_up, color);
                    Common_Data.show_map.SetColor(left_down, color);
                    Common_Data.show_map.SetColor(right_up, color);
                    Common_Data.show_map.SetColor(right_down, color);
                    Common_Data.show_map.SetColor(up, color);
                    Common_Data.show_map.SetColor(down, color);
                    break;
            }
        }
    }


    public static class Utility {
        public static StringBuilder builder = new();

        public static void SaveStaminaStorage() {
            PlayerPrefs.SetString("Stamina_Json", JsonUtility.ToJson(Game_State_Data.stamina_storage));
            PlayerPrefs.Save();
        }

        public static string FloatFormatter(float num) {
            if (num % 1 == 0) return ((int)num).ToString();
            else return num.ToString("0.#");
        }
        static List<string> parts = new();
        public static string FormatLeftTime(int sec) {

            int hours = sec / 3600;
            int minutes = (sec % 3600) / 60;
            int seconds = sec % 60;

            parts.Clear();

            if (hours > 0)
                parts.Add($"{hours}시간");
            if (minutes > 0)
                parts.Add($"{minutes}분");
            if (seconds > 0 || parts.Count == 0) // 모든 값이 0인 경우 최소한 "0초"라도 표시
                parts.Add($"{seconds}초");

            return string.Join(" ", parts) + " 남음";
        }

        public static Dictionary<int, Unit_Count_Checker> All_Mission_Recive(int type) {
            Game_Data.Init_Counter();
            var result = Game_Data.unit_counter;
            if(!result.ContainsKey(0)) result.Add(0, new(0, false));
            else result[0] = new(0, false);
            if (!result.ContainsKey(1)) result.Add(1, new(0, false));
            else result[1] = new(0, false);

            int size;
            string query;

            builder.Clear();
            switch (type) {
                // daily all recive
                case 1:
                    size = Game_Data.daily_quest.Count;
                    for (int i = 1; i <= size; i++) {
                        if (Game_Data.daily_quest[i].can_recive) {
                            Reciver(i, 1, result);
                            query = builder.Append("UPDATE dailyquest SET checker=1 WHERE id=")
                                           .Append(i).ToString();
                            ModifyDB.Instance.ModifySet(query, "daily");
                            builder.Clear();
                            Game_Data.daily_quest[i].checker = 1;
                            Game_Data.daily_quest[size].counter += 1;
                        }
                    }

                    // 모든 퀘스트 클리어 재점검
                    if (Game_Data.daily_quest[size].counter >= Game_Data.daily_quest[size].request_counter) {
                        Achievement_Observer.Instance.Daily_Complete_Check();
                        Achievement_Observer.Instance.Daily_Quest_All_Complete();
                        // 받기 가능하니까 받기
                        Game_Data.daily_quest[size].can_recive = true;
                        Reciver(size, 1, result);
                        query = builder.Append("UPDATE dailyquest SET checker=1 WHERE id=")
                                           .Append(size).ToString();
                        ModifyDB.Instance.ModifySet(query, "daily");

                        builder.Clear();
                    }

                    break;

                // weekly all recive
                case 2:
                    
                    size = Game_Data.weekly_quest.Count;
                    for(int i = 1; i <= size; i++) {
                        if (Game_Data.weekly_quest[i].can_recive) {
                            Reciver(i, 2, result);
                            query = builder.Append("UPDATE weeklyquest SET checker=1 WHERE id=")
                                           .Append(i).ToString();
                            ModifyDB.Instance.ModifySet(query, "weekly");
                            builder.Clear();
                            Game_Data.weekly_quest[i].checker = 1;
                            Game_Data.weekly_quest[size].counter += 1;
                        }

                        // 모든 퀘스트 클리어 재점검
                        if (Game_Data.weekly_quest[size].counter >= Game_Data.weekly_quest[size].request_counter) {
                            Achievement_Observer.Instance.Weekly_Complete_Check();
                            // 받기 가능하니까 받기
                            Game_Data.weekly_quest[size].can_recive = true;
                            Reciver(size, 2, result);
                            query = builder.Append("UPDATE weeklyquest SET checker=1 WHERE id=")
                                           .Append(size).ToString();
                            ModifyDB.Instance.ModifySet(query, "weekly");
                            builder.Clear();
                        }

                    }
                    break;

                // achievement all recive
                case 3:
                    // 로딩 띄우기
                    size = Game_Data.achievement.Count;
                    for (int i = 1; i <= size; i++) {
                        if (Game_Data.achievement[i].can_recive) {
                            Reciver(i, 3, result);
                            Game_Data.achievement[i].checker += 1;

                            query = builder.Append("UPDATE achievement SET checker=")
                                               .Append(Game_Data.achievement[i].checker)
                                               .Append(" WHERE id=")
                                               .Append(i).ToString();
                            ModifyDB.Instance.ModifySet(query, "achievement");
                            builder.Clear();

                            // 무한 반복 퀘스트 보상 반복을 위한 변수 제자리걸음

                            // can_recieve 수정
                            if (Game_Data.achievement[i].repeat) {
                                if (Game_Data.achievement[i].counter < (Game_Data.achievement[i].checker + 1) * Game_Data.achievement[i].endless_value) {
                                    Game_Data.achievement[i].can_recive = false;
                                }
                                i -= 1;
                            }
                        }
                    }
                    break;
            }

            // 업적 업데이트
            builder.Clear();
            if (result[0].count > 0) {
                Game_Data.user.Gold += result[0].count;
            }

            if (result[1].count > 0) {
                Game_Data.user.Experience += result[1].count;
            }


            // unit_counter의 숫자들을 체크해보고 0이상이면 업데이트
            foreach (var unit in Game_Data.unit_counter) {
                if (unit.Value.count > 0 && (unit.Key != 0 && unit.Key != 1)) {
                    builder.Clear();
                    query = builder.Append("UPDATE unit SET piece=")
                                   .Append(unit.Value.count + Game_Data.unit[unit.Key].piece)
                                   .Append(" WHERE id=")
                                   .Append(unit.Key)
                                   .ToString();
                    ModifyDB.Instance.ModifySet(query, "unit");
                }
            }

            return result;
        }
        private static void Reciver(int id, int type, Dictionary<int, Unit_Count_Checker> counter) {
            Mission quest = type switch {
                1 => Game_Data.daily_quest[id],
                2 => Game_Data.weekly_quest[id],
                3 => Game_Data.achievement[id],
                _ => null
            };
            if (quest == null || !quest.can_recive) { return; }
            // reward accept

            int reward_size = quest.reward_list.Count;
            int piece_size;
            for (int i = 0; i < reward_size; i++) {
                switch (quest.reward_list[i]) {
                    case "gold":
                        counter[0].count += int.Parse(quest.reward_val[i].ToString());
                        break;
                    case "d":
                        piece_size = int.Parse(quest.reward_val[i].ToString());
                        Select_Unit_Piece(piece_size, 2, counter);
                        break;
                    case "c":
                        piece_size = int.Parse(quest.reward_val[i].ToString());
                        Select_Unit_Piece(piece_size, 3, counter);
                        break;
                    case "b":
                        piece_size = int.Parse(quest.reward_val[i].ToString());
                        Select_Unit_Piece(piece_size, 4, counter);
                        break;
                    case "a":
                        piece_size = int.Parse(quest.reward_val[i].ToString());
                        Select_Unit_Piece(piece_size, 5, counter);
                        break;
                    case "s":
                        piece_size = int.Parse(quest.reward_val[i].ToString());
                        Select_Unit_Piece(piece_size, 6, counter);
                        break;
                    case "ex":
                        piece_size = int.Parse(quest.reward_val[i].ToString());
                        Select_Unit_Piece(piece_size, 7, counter);
                        break;
                    case "exp":
                        counter[1].count += int.Parse(quest.reward_val[i].ToString());
                        break;
                }
            }
        }

        public static Dictionary<int, Unit_Count_Checker> Mission_Recive(int id, int type) {
            Mission quest = type switch {
                1 => Game_Data.daily_quest[id],
                2 => Game_Data.weekly_quest[id],
                3 => Game_Data.achievement[id],
                _ => null
            };
            if (quest == null || !quest.can_recive) { return null; }
            // reward accept
            Game_Data.Init_Counter();
            int reward_size = quest.reward_list.Count;
            int piece_size ;
            for (int i = 0; i < reward_size; i++) {
                switch (quest.reward_list[i]) {
                    case "gold":
                        Game_Data.user.Gold += int.Parse(quest.reward_val[i].ToString()) ;
                        break;
                    case "d":
                        piece_size = int.Parse(quest.reward_val[i].ToString());
                        Select_Unit_Piece(piece_size, 2, Game_Data.unit_counter);
                        break;
                    case "c":
                        piece_size = int.Parse(quest.reward_val[i].ToString());
                        Select_Unit_Piece(piece_size, 3, Game_Data.unit_counter);
                        break;
                    case "b":
                        piece_size = int.Parse(quest.reward_val[i].ToString());
                        Select_Unit_Piece(piece_size, 4, Game_Data.unit_counter);
                        break;
                    case "a":
                        piece_size = int.Parse(quest.reward_val[i].ToString());
                        Select_Unit_Piece(piece_size, 5, Game_Data.unit_counter);
                        break;
                    case "s":
                        piece_size = int.Parse(quest.reward_val[i].ToString());
                        Select_Unit_Piece(piece_size, 6, Game_Data.unit_counter);
                        break;
                    case "ex":
                        piece_size = int.Parse(quest.reward_val[i].ToString());
                        Select_Unit_Piece(piece_size, 7, Game_Data.unit_counter);
                        break;
                    case "exp":
                        Game_Data.user.Experience += int.Parse(quest.reward_val[i].ToString());
                        break;
                }
            }
            string query;
            // unit_counter의 숫자들을 체크해보고 0이상이면 업데이트
            foreach(var unit in Game_Data.unit_counter) {
                if(unit.Value.count > 0 && (unit.Key != 0 || unit.Key != 1)) {
                    builder.Clear();
                    query = builder.Append("UPDATE unit SET piece=")
                                   .Append(unit.Value.count + Game_Data.unit[unit.Key].piece)
                                   .Append(" WHERE id=")
                                   .Append(unit.Key)
                                   .ToString();
                    ModifyDB.Instance.ModifySet(query, "unit");
                }
            }

            // 업적 업데이트
            builder.Clear();
            switch (type) {
                case 1:
                    query = builder.Append("UPDATE dailyquest SET checker=1 WHERE id=")
                                   .Append(id).ToString();
                    ModifyDB.Instance.ModifySet(query, "daily");

                    if(id == 10) {
                        Achievement_Observer.Instance.Daily_Quest_All_Complete();
                    }
                    break;
                case 2:
                    query = builder.Append("UPDATE weeklyquest SET checker=1 WHERE id=")
                                   .Append(id).ToString();
                    ModifyDB.Instance.ModifySet(query, "weekly");

                    break;
                case 3:
                    query = builder.Append("UPDATE achievement SET checker=1 WHERE id=")
                                   .Append(id).ToString();
                    ModifyDB.Instance.ModifySet(query, "achievement");

                    break;
            }
            builder.Clear();
            return Game_Data.unit_counter;
        }

        public static void Select_Unit_Piece(int size, int grade, Dictionary<int, Unit_Count_Checker> counter) {
            int min = grade switch {
                2 => 2001,
                3 => 3001,
                4 => 4001,
                5 => 5001,
                6 => 6001,
                7 => 7001,
                _ => 0
            };
            int max = grade switch {
                2 => 2010,
                3 => 3012,
                4 => 4012,
                5 => 5012,
                6 => 6012,
                7 => 7006,
                _ => 0
            };
            if (min == 0 || max == 0) return;

            for(int i = 0; i < size; i++) {
                int unit_number = UnityEngine.Random.Range(min, max);
                counter[unit_number].count++;
            }
        }
        public static string Get_Type_String(string type) {
            return type.Split("_")[1] switch {
                "1" => "일반",
                "2" => "하이브리드",
                "3" => "특수",
                "4" => "선택형",
                "5" => "선택형",
                _ => "불명"
            };
        }

        public static Enemy_Type Get_Enemy_Type(string type) {
            return type switch {
                "original" => Enemy_Type.Original,
                "shield" => Enemy_Type.Shield,
                "bust" => Enemy_Type.Bust,
                "reinforced_original" => Enemy_Type.Reinforced_Original,
                "reinforced_shield" => Enemy_Type.Reinforced_Shield,
                "reinforced_bust" => Enemy_Type.Reinforced_Bust,
                "boss_original" => Enemy_Type.Boss_Original,
                "boss_shield" => Enemy_Type.Boss_Shield,
                "boss_bust" => Enemy_Type.Boss_Bust,
                _ => Enemy_Type.Original
            };
        }

        public static Sprite Get_sprite(string name) {
            return name switch {
                "d_circle" or "c_circle" => Common_Data.d_circle,
                "d_triangle" or "c_triangle" => Common_Data.d_triangle,
                "d_square" or "c_square" => Common_Data.d_square,

                "c_star" => Common_Data.d_star,
                "c_moon" => Common_Data.d_moon,

                "b_circle" or "a_circle" or "s_circle" or "ex_circle" => Common_Data.b_circle,
                "b_triangle" or "a_triangle" or "s_triangle" or "ex_triangle" => Common_Data.b_triangle,
                "b_square" or "a_square" or "s_square" or "ex_square" => Common_Data.b_square,

                "b_star" or "a_star" or "s_star" or "ex_star" => Common_Data.b_star,
                "b_moon" or "a_moon" or "s_moon" or "ex_moon" => Common_Data.b_moon,
                

                _ => Common_Data.d_circle
            };
        }

        public static Sprite Get_sprite(int unit_id) {
            if(unit_id > 1000 && unit_id < 4000) {
                return Game_Data.unit[unit_id].type.Split("_")[0] switch {
                    "ci" => Common_Data.d_circle,
                    "tr" => Common_Data.d_triangle,
                    "sq" => Common_Data.d_square,
                    "st" => Common_Data.d_star,
                    "mo" => Common_Data.d_moon,
                    _ => Common_Data.d_circle
                };
            }
            else if (unit_id > 4000) {
                return Game_Data.unit[unit_id].type.Split("_")[0] switch {
                    "ci" => Common_Data.b_circle,
                    "tr" => Common_Data.b_triangle,
                    "sq" => Common_Data.b_square,
                    "st" => Common_Data.b_star,
                    "mo" => Common_Data.b_moon,
                    _ => Common_Data.b_circle
                };
            }
            else {
                return unit_id switch {
                    101 => Common_Data.piece_sprite,
                    102 => Common_Data.stone_sprite,
                    _ => Common_Data.piece_sprite
                };
            }
        }

        public static Sprite Get_Satellite_sprite(int unit_id) {
            if (unit_id < 5000) return null;
            // 5000 ~ 6000번대
            else if (unit_id < 7000) {
                return Game_Data.unit[unit_id].type.Split("_")[0] switch {
                    "ci" => Common_Data.d_circle,
                    "tr" => Common_Data.d_triangle,
                    "sq" => Common_Data.d_square,
                    "st" => Common_Data.d_star,
                    "mo" => Common_Data.d_moon,
                    _ => Common_Data.d_circle
                };
            }
            // 7000
            else {
                return Game_Data.unit[unit_id].type.Split("_")[0] switch {
                    "ci" => Common_Data.b_circle,
                    "tr" => Common_Data.b_triangle,
                    "sq" => Common_Data.b_square,
                    "st" => Common_Data.b_star,
                    "mo" => Common_Data.b_moon,
                    _ => Common_Data.b_circle
                };
            }
        }

        public static Color Get_Grade_Color(int unit_id) {
            if(unit_id > 1000) {
                return Game_Data.unit[unit_id].grade switch {
                    "e" => Common_Data.color_e,
                    "d" => Common_Data.color_d,
                    "c" => Common_Data.color_c,
                    "b" => Common_Data.color_b,
                    "a" => Common_Data.color_a,
                    "s" => Common_Data.color_s,
                    "ex" => Common_Data.color_ex,
                    _ => Common_Data.color_e
                };
            }
            else {
                return Common_Data.color_e;
            }
        }
        public static Color Get_Grade_Color(string grade) {
            return grade switch {
                "e" => Common_Data.color_e,
                "d" => Common_Data.color_d,
                "c" => Common_Data.color_c,
                "b" => Common_Data.color_b,
                "a" => Common_Data.color_a,
                "s" => Common_Data.color_s,
                "ex" => Common_Data.color_ex,
                "r" => Common_Data.color_r,
                _ => Common_Data.color_e
            };
        }

        public static Color Get_Type_Color(int unit_id) {
            if (unit_id > 2000) {
                return Game_Data.unit[unit_id].type.Split("_")[1] switch {
                    "1" => Common_Data.material_color,
                    "2" => Common_Data.hybrid_color,
                    "3" => Common_Data.ethereal_color,
                    "4" => Common_Data.special_material_color,
                    "5" => Common_Data.special_ethereal_color,
                    _ => Common_Data.material_color
                };
            }
            else {
                return Common_Data.color_e;
            }
        }

        public static bool Is_Special(int unit_id) {
            if (unit_id < 1000) return false;
            return Game_Data.unit[unit_id].type.Contains("st") || Game_Data.unit[unit_id].type.Contains("mo");
        }

        public static UIPanelID Get_Pannel_ID(string name) {
            return name switch {
                "summon_btn_area" => UIPanelID.SummonButtonArea,
                "btn_area" => UIPanelID.ButtonArea,
                "combine_area" => UIPanelID.CombineArea,
                "install_area" => UIPanelID.InstallArea,
                "unit_info_area" => UIPanelID.UnitInfoArea,
                "upgrade_area" => UIPanelID.UpgradeArea,
                "field_unit_click_area" => UIPanelID.FieldUnitClickArea,
                "announce_area" => UIPanelID.AnnounceArea,
                "round_check_area" => UIPanelID.RoundCheckArea,
                "combine_book_area" => UIPanelID.CombineBookArea,
                "mission_area" => UIPanelID.MissionArea,
                "spell_area" => UIPanelID.SpellArea,
                _ => UIPanelID.None
            };
        }

        public static Sprite Get_Field_Type(int unit_id) {
            return Game_Data.unit[unit_id].type.Split("_")[1] switch {
                "1" => Common_Data.unit_field_icon_4,
                "2" => Common_Data.unit_field_icon_2v,
                "3" => Common_Data.unit_field_icon_2h,
                "4" => Common_Data.unit_field_icon_6h,
                "5" => Common_Data.unit_field_icon_6v,
                _ => Common_Data.unit_field_icon_4
            };
        }

        public static void MergeSort(int[] arr, int start, int end) {

            if (start >= end)
                return;

            int mid = (start + end) / 2;
            MergeSort(arr, start, mid);
            MergeSort(arr, mid + 1, end);
            Merge(arr, start, mid, end);
        }

        private static void Merge(int[] arr, int start, int mid, int end) {
            int a = start;
            int b = mid + 1;
            int c = start;

            int[] tmp = new int[arr.Length];
            // 두지점부터 시작해 마지막까지 비교하며 작은것부터 정렬
            while (a <= mid && b <= end) {
                tmp[c++] = arr[a] <= arr[b] ? arr[a++] : arr[b++];
            }

            // 남아있는 경우 순서대로 다시 넣기
            // 후열부터 확인 -> 전열이 더 작은 경우를 먼저 넣었으므로 
            // 전열이 남은것은 후열보다 큰것 -> 반드시 남은 후열이 남은 전열보다 작음
            while (b <= end) {
                tmp[c++] = arr[b++];
            }
            while (a <= mid) {
                tmp[c++] = arr[a++];
            }

            // a를 정렬상태로 옮기기
            // 시작점은 start와 end까지만
            for (int i = start; i <= end; i++) {
                arr[i] = tmp[i];
            }

        }

    }

}