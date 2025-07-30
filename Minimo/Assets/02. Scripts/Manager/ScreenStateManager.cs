using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

public enum ScreenState
{
    Sky,
    Space,
    DeepSpace,
}

public class ScreenStateManager : ManagerBase
{
    public ReactiveProperty<ScreenState> CurrentState { get; } = new(ScreenState.Sky);
    
    [SerializeField] private Tilemap _villageMap;
    [SerializeField] private GameObject _planetObj;

    private void Start()
    {
        ChangeState(ScreenState.Sky);
        
        _planetObj.SetActive(false);
        Camera.main.orthographicSize = 4f;
        _villageMap.color = Color.white;
    }
    
    public void ChangeState(int addValue)
    {
        if (addValue > 0)
        {
            ChangeState((ScreenState)Mathf.Min((int)CurrentState.Value + addValue, (int)ScreenState.DeepSpace));
        }
        else
        {
            ChangeState((ScreenState)Mathf.Max((int)CurrentState.Value + addValue, 0));
        }
        
    }

    private void ChangeState(ScreenState newState)
    {
        if (CurrentState.Value == newState) return;
        
        switch (newState)
        {
            case ScreenState.Sky:
                if (CurrentState.Value == ScreenState.Space)
                {
                    Camera.main.DOOrthoSize(3f, 1f);
                    App.FadeInOut(0.5f, 
                        () =>
                        {
                            
                            _villageMap.color = Color.white;
                            _planetObj.SetActive(false);
                        });
                }
                break;
            
            case ScreenState.Space:
                Camera.main.DOKill();
                Camera.main.DOOrthoSize(10f, 1f).SetEase(Ease.InQuad);
                App.FadeInOut(0.5f, 
                    () =>
                    {
                        _planetObj.SetActive(true);
                        _villageMap.color = Color.clear;
                    });
                break;
            
            case ScreenState.DeepSpace:
                Camera.main.DOOrthoSize(100f, 1f).SetEase(Ease.InQuad);
                App.FadeInOut(0.5f, 
                    () => _planetObj.SetActive(false));
                break;
        }
        
        CurrentState.Value = newState;
    }
}
