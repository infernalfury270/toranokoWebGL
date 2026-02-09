using UnityEngine;
public class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    private T _instance;
    public T Instance
    {
        get {
            if (_instance == null)
                return Load();
            return _instance; 
        }
    }
    private T Load()
    {
        if (_instance != null) return _instance;
        var search = Resources.LoadAll<T>("");
        if (search == null || search.Length == 0)
        {
#if UNITY_EDITOR
            Debug.LogError("Could not find " + nameof(T) + " asset in Resources.");
#endif
            return null;
        }
#if UNITY_EDITOR
        if (search.Length > 1)
            Debug.LogWarning("Found more than one instance of " + nameof(T) + " in Resources.");
#endif
        _instance = search[0];
        return _instance;
    }
}
