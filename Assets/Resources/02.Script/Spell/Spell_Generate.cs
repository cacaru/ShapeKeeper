using UnityEngine;
using static CUSTOM_DATA.Game_Data;
using CUSTOM_DATA;


public class Spell_Generate : Scene_Singleton<Spell_Generate>
{
    public void Generate(string _type) {
        if (piece_count < 5) return;

        var type = _type switch {
            "ice" => OraType.Ice,
            "fire" => OraType.Fire,
            "poison" => OraType.Poison,
            "water" => OraType.Water,
            _ => OraType.None
        };
        
        // type이 ice fire water poison 중 하나가 아니면 return
        if (!(type.Equals(OraType.Ice) || type.Equals(OraType.Fire) || type.Equals(OraType.Poison) || type.Equals(OraType.Water)) || type == OraType.None ) return;
        
        stored_spell[type].count++;
        stored_spell[type].all_count++;
        piece_count -= 5;
        In_Game_Setter.Instance.Set_Goods();
        // counter 리셋
        Spell_Setter.Instance.Spell_Counter_Set();
    }

    public void Recall(OraType type) {
        stored_spell[type].count++;
        Spell_Setter.Instance.Spell_Counter_Set();
    }
}
