using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
    private static T instance;

    public static T Instance {
        get {
            if (instance == null) {

                instance = (T)FindFirstObjectByType(typeof(T));

                if (instance == null) {
                    GameObject obj = new(typeof(T).Name, typeof(T));
                    instance = obj.AddComponent<T>();
                }

                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }

    void Awake() {
        if (instance != null) {

            if (instance != this) {
                Destroy(gameObject);
            }
            return;
        }

        instance = GetComponent<T>();
        DontDestroyOnLoad(gameObject);
    }

}
