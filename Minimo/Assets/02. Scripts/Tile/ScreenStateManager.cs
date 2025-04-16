using UnityEngine;

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
            ChangeState((ScreenState)Mathf.Max((int)CurrentState - addValue, 0));
        }
        
    }

    private void ChangeState(ScreenState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case ScreenState.Town:
                _tileAlphaSystem.ActiveTileAlphaSystem(false);
                break;
            
            case ScreenState.Sky:
                _tileAlphaSystem.ActiveTileAlphaSystem(true);
                break;
            
            case ScreenState.Space:
                break;
            
            case ScreenState.DeepSpace:
                break;
        }
    }
}
