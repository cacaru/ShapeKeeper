using TMPro;
using UnityEngine;
using static CUSTOM_DATA.Game_Data;
using static CUSTOM_DATA.Common_Data;
using CUSTOM_DATA;
using UnityEngine.UI;
using System.Collections.Generic;

public class Spell_Setter : Scene_Singleton<Spell_Setter>
{
    [SerializeField] GameObject ice_shower;
    [SerializeField] GameObject ice_upgrade;
    [SerializeField] Image generate_ice;

    [SerializeField] GameObject fire_shower;
    [SerializeField] GameObject fire_upgrade;
    [SerializeField] Image generate_fire;

    [SerializeField] GameObject poison_shower;
    [SerializeField] GameObject poison_upgrade;
    [SerializeField] Image generate_poison;

    [SerializeField] GameObject water_shower;
    [SerializeField] GameObject water_upgrade;
    [SerializeField] Image generate_water;

    private readonly string MAX = "MAX";

    private readonly Dictionary<string, TMP_Text> text_container = new();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text_container.Add("ice_grade", ice_shower.transform.Find("grade").GetComponent<TMP_Text>());
        text_container.Add("fire_grade", fire_shower.transform.Find("grade").GetComponent<TMP_Text>());
        text_container.Add("poison_grade", poison_shower.transform.Find("grade").GetComponent<TMP_Text>());
        text_container.Add("water_grade", water_shower.transform.Find("grade").GetComponent<TMP_Text>());

        text_container.Add("ice_value", ice_shower.transform.Find("value").GetComponent<TMP_Text>());
        text_container.Add("fire_value", fire_shower.transform.Find("value").GetComponent<TMP_Text>());
        text_container.Add("poison_value", poison_shower.transform.Find("value").GetComponent<TMP_Text>());
        text_container.Add("water_value", water_shower.transform.Find("value").GetComponent<TMP_Text>());

        text_container.Add("ice_counter", ice_shower.transform.Find("count").GetComponent<TMP_Text>());
        text_container.Add("fire_counter", fire_shower.transform.Find("count").GetComponent<TMP_Text>());
        text_container.Add("poison_counter", poison_shower.transform.Find("count").GetComponent<TMP_Text>());
        text_container.Add("water_counter", water_shower.transform.Find("count").GetComponent<TMP_Text>());

        text_container.Add("upgrade_ice_value", ice_upgrade.transform.Find("value").GetComponent<TMP_Text>());
        text_container.Add("upgrade_fire_value", fire_upgrade.transform.Find("value").GetComponent<TMP_Text>());
        text_container.Add("upgrade_poison_value", poison_upgrade.transform.Find("value").GetComponent<TMP_Text>());
        text_container.Add("upgrade_water_value", water_upgrade.transform.Find("value").GetComponent<TMP_Text>());

        text_container.Add("upgrade_ice_cost", ice_upgrade.transform.Find("cost").GetComponent<TMP_Text>());
        text_container.Add("upgrade_fire_cost", fire_upgrade.transform.Find("cost").GetComponent<TMP_Text>());
        text_container.Add("upgrade_poison_cost", poison_upgrade.transform.Find("cost").GetComponent<TMP_Text>());
        text_container.Add("upgrade_water_cost", water_upgrade.transform.Find("cost").GetComponent<TMP_Text>());

        text_container.Add("upgrade_next_ice_value", ice_upgrade.transform.Find("next_value").GetComponent<TMP_Text>());
        text_container.Add("upgrade_next_fire_value", fire_upgrade.transform.Find("next_value").GetComponent<TMP_Text>());
        text_container.Add("upgrade_next_poison_value", poison_upgrade.transform.Find("next_value").GetComponent<TMP_Text>());
        text_container.Add("upgrade_next_water_value", water_upgrade.transform.Find("next_value").GetComponent<TMP_Text>());

        Set_Spell_Area();
        Set_Upgrade_Spell_Area();
    }

    public void Set_Spell_Area() {
        var icon = stored_spell[OraType.Ice].level switch {
            1 => ice_1,
            2 => ice_2,
            3 => ice_3,
            4 => ice_4,
            _ => ice_1
        };
        ice_shower.transform.Find("icon").GetComponent<Image>().sprite = icon;
        ice_upgrade.transform.Find("icon").GetComponent<Image>().sprite = icon;
        generate_ice.sprite = icon;

        text_container["ice_grade"].text = stored_spell[OraType.Ice].Get_level();
        text_container["ice_value"].text = (stored_spell[OraType.Ice].level * 0.1f).ToString() + "초";
        text_container["ice_counter"].text = stored_spell[OraType.Ice].count.ToString();

        icon = stored_spell[OraType.Fire].level switch {
            1 => fire_1,
            2 => fire_2,
            3 => fire_3,
            4 => fire_4,
            _ => fire_1
        };
        fire_shower.transform.Find("icon").GetComponent<Image>().sprite = icon;
        fire_upgrade.transform.Find("icon").GetComponent<Image>().sprite = icon;
        generate_fire.sprite = icon;
        text_container["fire_grade"].text = stored_spell[OraType.Fire].Get_level();
        text_container["fire_value"].text = (stored_spell[OraType.Fire].level * 10f).ToString() + "%";
        text_container["fire_counter"].text = stored_spell[OraType.Fire].count.ToString();

        icon = stored_spell[OraType.Poison].level switch {
            1 => poison_1,
            2 => poison_2,
            3 => poison_3,
            4 => poison_4,
            _ => poison_1
        };
        poison_shower.transform.Find("icon").GetComponent<Image>().sprite = icon;
        poison_upgrade.transform.Find("icon").GetComponent<Image>().sprite = icon;
        generate_poison.sprite = icon;
        text_container["poison_grade"].text = stored_spell[OraType.Poison].Get_level();
        text_container["poison_value"].text = (stored_spell[OraType.Poison].level * 5).ToString() + "%";
        text_container["poison_counter"].text = stored_spell[OraType.Poison].count.ToString();

        icon = stored_spell[OraType.Water].level switch {
            1 => water_1,
            2 => water_2,
            3 => water_3,
            4 => water_4,
            _ => water_1
        };
        water_shower.transform.Find("icon").GetComponent<Image>().sprite = icon;
        water_upgrade.transform.Find("icon").GetComponent<Image>().sprite = icon;
        generate_water.sprite = icon;
        text_container["water_grade"].text = stored_spell[OraType.Water].Get_level();
        text_container["water_value"].text = (stored_spell[OraType.Water].level * 1f).ToString() + "초";
        text_container["water_counter"].text = stored_spell[OraType.Water].count.ToString();
    }

    public void Set_Upgrade_Spell_Area() {
        if (stored_spell[OraType.Ice].level == stored_spell[OraType.Ice].MAX_LEVEL) {
            text_container["upgrade_ice_value"].text = MAX;
            text_container["upgrade_ice_cost"].text = MAX;
            text_container["upgrade_next_ice_value"].text = MAX;
        }
        else {
            text_container["upgrade_ice_value"].text = (stored_spell[OraType.Ice].level * 0.1f).ToString() + "초";
            text_container["upgrade_ice_cost"].text = stored_spell[OraType.Ice].need_piece.ToString() + "개";
            text_container["upgrade_next_ice_value"].text = ((stored_spell[OraType.Ice].level + 1) * 0.1f).ToString() + "초";
        }
        //--------------------------------------------------------------------------------------------------------------
        if (stored_spell[OraType.Fire].level == stored_spell[OraType.Fire].MAX_LEVEL) {
            text_container["upgrade_fire_value"].text = MAX;
            text_container["upgrade_fire_cost"].text = MAX;
            text_container["upgrade_next_fire_value"].text = MAX;
        }
        else {
            text_container["upgrade_fire_cost"].text = stored_spell[OraType.Fire].need_piece.ToString() + "개";
            text_container["upgrade_fire_value"].text = (stored_spell[OraType.Fire].level * 10f).ToString() + "%";
            text_container["upgrade_next_fire_value"].text = ((stored_spell[OraType.Fire].level + 1) * 10f).ToString() + "%";
        }
        //--------------------------------------------------------------------------------------------------------------
        if (stored_spell[OraType.Poison].level == stored_spell[OraType.Poison].MAX_LEVEL) {
            text_container["upgrade_poison_value"].text = MAX;
            text_container["upgrade_poison_cost"].text = MAX;
            text_container["upgrade_next_poison_value"].text = MAX;
        }
        else {
            text_container["upgrade_poison_cost"].text = stored_spell[OraType.Poison].need_piece.ToString() + "개";
            text_container["upgrade_poison_value"].text = (stored_spell[OraType.Poison].level * 5f).ToString() + "%";
            text_container["upgrade_next_poison_value"].text = ((stored_spell[OraType.Poison].level + 1) * 5f).ToString() + "%";
        }
        //--------------------------------------------------------------------------------------------------------------
        if (stored_spell[OraType.Water].level == stored_spell[OraType.Water].MAX_LEVEL) {
            text_container["upgrade_water_value"].text = MAX;
            text_container["upgrade_water_cost"].text = MAX;
            text_container["upgrade_next_water_value"].text = MAX;
        }
        else {
            text_container["upgrade_water_value"].text = (stored_spell[OraType.Water].level * 1f).ToString() + "초";
            text_container["upgrade_water_cost"].text = (stored_spell[OraType.Water].need_piece).ToString() + "개";
            text_container["upgrade_next_water_value"].text = ((stored_spell[OraType.Water].level + 1) * 1f).ToString() + "초";
        }
    }

    public void Spell_Counter_Set() {
        text_container["ice_counter"].text = stored_spell[OraType.Ice].count.ToString();
        text_container["fire_counter"].text = stored_spell[OraType.Fire].count.ToString();
        text_container["poison_counter"].text = stored_spell[OraType.Poison].count.ToString();
        text_container["water_counter"].text = stored_spell[OraType.Water].count.ToString();
    }

}
