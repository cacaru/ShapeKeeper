using UnityEngine;
using CUSTOM_DATA;
using static CUSTOM_DATA.Game_Data;

public class Spell_Upgrade : MonoBehaviour
{
    public void Upgrade(string _type) {
        var type = _type switch {
            "ice" => OraType.Ice,
            "fire" => OraType.Fire,
            "poison" => OraType.Poison,
            "water" => OraType.Water,
            _ => OraType.None
        };

        if (type == OraType.None) return;

        int need_piece = stored_spell[type].need_piece;
        if (need_piece <= piece_count) {
            piece_count -= need_piece;
            stored_spell[type].Level_Up();
            // 표기 업데이트
            In_Game_Setter.Instance.Set_Goods();
            Spell_Setter.Instance.Set_Spell_Area();
            Spell_Setter.Instance.Set_Upgrade_Spell_Area();
        }
    }
    
}
