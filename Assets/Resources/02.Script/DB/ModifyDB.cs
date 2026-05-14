using CUSTOM_DATA;
using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using static CUSTOM_DATA.Game_State_Data;
using UnityEngine;

using static CUSTOM_DATA.Common_Data;

public class ModifyDB : Singleton<ModifyDB>
{
    private IDbConnection connector;
    private IDbCommand command;

    void Start() {
        connector = new SqliteConnection(Game_Value_Data.DB_CONNECT_STRING);
        StartCoroutine(Modify_Active());
    }

    private bool modifying = false;
    private readonly Queue<string> query_queue = new();
    private readonly Queue<string> db_queue = new();

    // modify controll
    public void ModifySet(string query, string db_name) {
        query_queue.Enqueue(query);
        db_queue.Enqueue(db_name);
    }

    IEnumerator Modify_Active() {
        while (true) {
            if (query_queue.Count > 0 && !modifying) {
                Modify(query_queue.Dequeue(), db_queue.Dequeue());
            }
            else if(query_queue.Count <= 0 && !modifying && db_state != DB_STATE.Usable) {
                db_state = DB_STATE.Usable;
            }
            yield return wffu;
        }
    }

    private void Modify(string query, string db_name) {
        modifying = true;
        
        connector.Open();
        db_state = DB_STATE.Modifying;
        // query Àû¿ë
        command = connector.CreateCommand();
        command.CommandText = query;
        command.ExecuteNonQuery();

        switch(db_name) {
            case "unit":
                ConnectDB.Instance.Connect_UnitDB();
                break;
            case "user":
                ConnectDB.Instance.Connect_UserDB();
                break;
            case "achievement":
                ConnectDB.Instance.Connect_Achievement();
                break;
            case "daily":
                ConnectDB.Instance.Connect_Daily_Quest();
                break;
            case "weekly":
                ConnectDB.Instance.Connect_Weekly_Quest();
                break;
        }

        connector.Close();
        modifying = false;
    }
}
