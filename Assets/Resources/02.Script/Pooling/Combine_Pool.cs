using System.Collections.Generic;
using UnityEngine;
using static CUSTOM_DATA.Game_Value_Data;

public class Combine_Pool : Singleton<Combine_Pool> {

    private readonly Queue<GameObject> function_pool = new();
    private readonly Queue<GameObject> function_title_pool = new();
    private readonly string title_name = "combine_title";
    private readonly string function_name = "combine_prefab";
    private readonly string compare_title = "title";

    void Start() {
        Initialize();
    }

    private void Initialize() {
        // title 5
        for (int i = 0; i < 5; i++) {
            Create_Title();
        }
        // combine_fuction 20
        for (int i = 0; i < 20; i++) {
            Create_Function();
        }
    }

    private void Create_Title() {
        var title = Instantiate(combine_function_title_prefab, transform);
        title.name = title_name;
        title.SetActive(false);
        function_title_pool.Enqueue(title);
    }

    private void Create_Function() {
        var function = Instantiate(combine_function_prefab, transform);
        function.name = function_name;
        function.SetActive(false);
        function_pool.Enqueue(function);
    }


    public GameObject Get_Function(int type) {
        if (function_pool.Count <= 0) Create_Function();
        if (function_title_pool.Count <= 0) Create_Title();

        var result = type switch {
            1 => function_title_pool.Dequeue(),
            2 => function_pool.Dequeue(),
            _ => null
        };

        result.SetActive(true);
        return result;
    }

    public void Return_Function(GameObject return_obj) {
        return_obj.transform.SetParent(transform);
        
        return_obj.SetActive(false);

        if (return_obj.name.Contains(compare_title)){
            function_title_pool.Enqueue(return_obj);
        }
        else {
            // img init 
            return_obj.transform.Find("Material_1").gameObject.SetActive(false);
            return_obj.transform.Find("MaterialBorder_1").gameObject.SetActive(false);
            return_obj.transform.Find("MaterialBack_1").gameObject.SetActive(false);
            return_obj.transform.Find("plus_1").gameObject.SetActive(false);

            return_obj.transform.Find("Material_2").gameObject.SetActive(false);
            return_obj.transform.Find("MaterialBorder_2").gameObject.SetActive(false);
            return_obj.transform.Find("MaterialBack_2").gameObject.SetActive(false);
            return_obj.transform.Find("plus_2").gameObject.SetActive(false);
            return_obj.transform.Find("piece_2").gameObject.SetActive(false);

            return_obj.transform.Find("Material_3").gameObject.SetActive(false);
            return_obj.transform.Find("MaterialBorder_3").gameObject.SetActive(false);
            return_obj.transform.Find("MaterialBack_3").gameObject.SetActive(false);
            return_obj.transform.Find("plus_3").gameObject.SetActive(false);
            return_obj.transform.Find("piece_3").gameObject.SetActive(false);

            return_obj.transform.Find("Material_4").gameObject.SetActive(false);
            return_obj.transform.Find("MaterialBorder_4").gameObject.SetActive(false);
            return_obj.transform.Find("MaterialBack_4").gameObject.SetActive(false);
            return_obj.transform.Find("plus_4").gameObject.SetActive(false);
            return_obj.transform.Find("piece_4").gameObject.SetActive(false);

            function_pool.Enqueue(return_obj);
        }
    }
}
