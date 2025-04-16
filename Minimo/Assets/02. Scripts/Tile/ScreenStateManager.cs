using UnityEngine;
using UnityEngine.Tilemaps;

public enum ScreenState
{
    Town,
    Sky,
    Space,
    DeepSpace,
}

public class ScreenStateManager : ManagerBase
{
    public ScreenState CurrentState { get; private set; } = ScreenState.Sky;
    
    [SerializeField] private TileAlphaSystem _tileAlphaSystem;
    [SerializeField] private Tilemap _villageMap;

    protected override void Awake()
    {
        base.Awake();
        
        ChangeState(ScreenState.Sky);
    }
    
    public void ChangeState(int addValue)
    {
        if (addValue > 0)
        {
            ChangeState((ScreenState)Mathf.Min((int)CurrentState + addValue, (int)ScreenState.DeepSpace));
        }
        else
        {
            ChangeState((ScreenState)Mathf.Max((int)CurrentState + addValue, 0));
        }
        
    }

    private void ChangeState(ScreenState newState)
    {
        CurrentState = newState;
        Debug.Log(CurrentState);

        switch (newState)
        {
            case ScreenState.Town:
                _tileAlphaSystem.ActiveTileAlphaSystem(false);
                break;
            
            case ScreenState.Sky:
                _tileAlphaSystem.ActiveTileAlphaSystem(true);
                _villageMap.color = Color.white;
                Camera.main.orthographicSize = 3;
                break;
            
            case ScreenState.Space:
                _tileAlphaSystem.ActiveTileAlphaSystem(false);
                _villageMap.color = Color.clear;
                Camera.main.orthographicSize = 5;
                break;
            
            case ScreenState.DeepSpace:
                Camera.main.orthographicSize = 10;
                break;
        }
    }
}
