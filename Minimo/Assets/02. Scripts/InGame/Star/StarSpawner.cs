using UnityEngine;
using Random = UnityEngine.Random;

public class StarSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _starObj;

    private int _prevLevel;

    private void Awake()
    {
        AccountInfo.Instance.Level.OnLevelUp += OnLevelUp;
    }

    private void Start()
    {
        OnLevelUp(AccountInfo.Instance.Level.Count);
    }

    private void OnLevelUp(int level)
    {
        for (var i = 0; i < level - _prevLevel; i++)
        {
            Instantiate(_starObj, 
                new Vector3(Random.Range(-6f, 6f), Random.Range(-2.5f, 2.5f), 0),
                Quaternion.identity, 
                transform);
        }

        _prevLevel = level;
    }
}