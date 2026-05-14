using DG.Tweening;
using UnityEngine;

public class Spell_Pannel_Toggler : MonoBehaviour
{

    [SerializeField] GameObject Generate_pannel;
    [SerializeField] GameObject Upgrade_pannel;


    private bool generate_pannel_toggle = false;
    private bool upgrade_pannel_toggle = false;

    private Vector3 ON = Vector3.zero;
    private Vector3 OFF = new(90, 0, 0);

    public void Generate_Spell_Toggle() {
        generate_pannel_toggle = !generate_pannel_toggle;
        Generate_pannel.transform.DORotate( generate_pannel_toggle ? ON : OFF, 0.1f);

        if (upgrade_pannel_toggle) {
            upgrade_pannel_toggle = false;
            Upgrade_pannel.transform.DORotate(upgrade_pannel_toggle ? ON : OFF, 0.1f);
        }
    }

    public void Upgrade_Spell_Toggle() {
        upgrade_pannel_toggle = !upgrade_pannel_toggle;
        Upgrade_pannel.transform.DORotate(upgrade_pannel_toggle ? ON : OFF, 0.1f);

        if (generate_pannel_toggle) {
            generate_pannel_toggle = false;
            Generate_pannel.transform.DORotate(generate_pannel_toggle ? ON : OFF, 0.1f);
        }
    }

}
