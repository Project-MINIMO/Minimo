using UnityEngine;

public class StarSpawner : MonoBehaviour
{
    [SerializeField] private CameraBoundsUpdater _mapBounds;
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
                _mapBounds.GetRandomOutsideMapPoint(),
                Quaternion.identity, 
                transform);
        }

        _prevLevel = level;
    }
}