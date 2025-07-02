using System.Linq;

using UnityEngine;

public abstract class QuestGiver : InteractObject
{
    protected DetailQuestData _questData;
    private ScreenStateManager _screenStateManager;

    private bool _eventTriggered;
    private float _lastUpdateTime;

    private void Start()
    {
        SetQuest();

        _screenStateManager = App.GetManager<ScreenStateManager>();
        
        _lastUpdateTime = Time.time;
    }
    
    private void Update()
    {
        if (Time.time - _lastUpdateTime < 0.1f) return;

        _lastUpdateTime = Time.time;
        
        if (_eventTriggered) return;
        if (_screenStateManager.CurrentState.Value != ScreenState.Sky) return;

        if (IsInUpperHalfOfScreen(transform))
        {
            Debug.Log("_eventTriggered");
            _eventTriggered = true;
            App.GetManager<QuestManager>().AddQuest(_questData);
        }
    }
    
    private bool IsInUpperHalfOfScreen(Transform target)
    {
        var cam = Camera.main;
        
        var viewportPos = cam.WorldToViewportPoint(target.position);
        
        var isOnScreen = viewportPos.z > 0 &&
                         viewportPos.x >= 0 && viewportPos.x <= 1 &&
                         viewportPos.y >= 0 && viewportPos.y <= 1;
        
        var isInUpperHalf = viewportPos.y > 0.5f;

        return isOnScreen && isInUpperHalf;
    }

    protected abstract void SetQuest(); //temp
}
