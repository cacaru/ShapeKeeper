using System;
using System.Collections;
using UnityEngine;
using CUSTOM_DATA;
using static CUSTOM_DATA.Common_Data;
using static CUSTOM_DATA.Game_State_Data;

public class Stamina_Observer : Singleton<Stamina_Observer>
{
    private readonly int RATE = 3;
    private bool is_active = false;
    
    public void Observing() {
        if (is_active) return;

        is_active = true;
        StartCoroutine(CheckRecoveryRoutine());
    }

    IEnumerator CheckRecoveryRoutine() {
        while (true) {
            RecoverStaminaIfNeeded();
            yield return wfs_60;
        }
    }

    void RecoverStaminaIfNeeded() {

        if (stamina_storage.Stamina >= stamina_storage.Max_Stamina) return;

        DateTime now = DateTime.Now;
        DateTime lastTime;
        if (!DateTime.TryParse(stamina_storage.last_recover_time, out lastTime)) {
            // 실패 시 현재 시간으로 대체
            lastTime = now;
            stamina_storage.last_recover_time = now.ToString("o");
            Utility.SaveStaminaStorage();
        }
        double passing_time = (now - lastTime).TotalMinutes;

        int recoverCount = Mathf.FloorToInt((float)(passing_time / RATE));
        if (recoverCount > 0) {
            int cal = stamina_storage.Max_Stamina - stamina_storage.Stamina;
            if (cal < recoverCount) recoverCount = cal;

            stamina_storage.Add_Stamina(recoverCount);
            stamina_storage.last_recover_time = now.AddMinutes(-passing_time % RATE).ToString("o"); // 나머지 유지
            Utility.SaveStaminaStorage();
            // home 화면이면 보이기 upload
            if (now_scene == SceneType.Home) {
                Home_Stamina_Setter.Instance.Setting();
            }
        }
    }

}
