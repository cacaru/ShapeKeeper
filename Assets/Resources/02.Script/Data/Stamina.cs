using System;
using UnityEngine;

[Serializable]
public class StaminaClass
{
    [SerializeField] int stamina = 0;
    [SerializeField] int max_stamina = 0;
    [SerializeField] public string last_recover_time = "";

    public int Stamina { get { return stamina; } }
    public int Max_Stamina { get { return max_stamina; } }
    
    public void Add_Stamina(int val) {
        stamina += val;
    }

    public void Max_Increase() {
        max_stamina += 4;
    }

    public void Consume_Stamina(int used) {
        stamina -= used;
    }
}
