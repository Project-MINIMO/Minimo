using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;
    private static bool _isDestroyed = false;

    public static T Instance
    {
        get
        {
            if (_isDestroyed)
            {
                return null;
            }

            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    GameObject singletonObject = new(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                }
            }

            return _instance;
        }
    }

    public static bool IsInstanceCreated()
    {
        return _instance != null;
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ForceDestroy()
    {
        _instance = null;
        Destroy(gameObject);
    }

    public void ForgetInstance()
    {
        _instance = null;   
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            DestroySingleton();
            _isDestroyed = true;
            _instance = null;
        }
    }

    /// <summary>
    /// Clean up Singleton
    /// </summary>
    public virtual void DestroySingleton() { }
}