using static CUSTOM_DATA.Game_Data;
using UnityEngine;

public class Shift : Scene_Singleton<Shift>
{
    public void Shift_tower(int new_id, Vector3 pos) {

        GameObject target = null;
        foreach (var tower in installed_tower) {
            if(tower.Value.pos == pos) {
                target = tower.Value.me;
            }
        }

        if (target == null) {
            //Debug.Log("err  with non object in pos >>>> " + pos);
            return;
        }

        int target_id = target.GetComponent<Combine_Field>().id;
        // target_id를 card로
        Card_Observer.Instance.Card_Hider(target_id, true);
        Destroy(target);
        installed_tower.Remove(target.GetComponent<Special_Unit_Type_Changer>().tower_id);
        // new_id 새로운 타워로 install
        TowerInstaller.Instance.Install_Tower(new_id, pos);
        Card_Observer.Instance.Card_Hider(new_id, false);
    }
}
