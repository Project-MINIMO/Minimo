using System.Linq;

using UnityEngine;

public class Star : InteractObject
{
    private QuestData _questData;
    private ScreenStateManager _screenStateManager;
    
    private SpriteRenderer _renderer;
    
    private bool _eventTriggered;
    private float _lastUpdateTime;

    private void Start()
    {
        SetQuest();
        
        _renderer = GetComponent<SpriteRenderer>();
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
            _eventTriggered = true;
            App.GetManager<QuestManager>().AddQuest(_questData);
        }
    }
    
    bool IsInUpperHalfOfScreen(Transform target)
    {
        var cam = Camera.main;

        // 스프라이트의 월드 좌표 → 뷰포트 좌표로 변환
        Vector3 viewportPos = cam.WorldToViewportPoint(target.position);

        // 화면 안에 있어야 하며
        bool isOnScreen = viewportPos.z > 0 &&
                          viewportPos.x >= 0 && viewportPos.x <= 1 &&
                          viewportPos.y >= 0 && viewportPos.y <= 1;

        // 화면 상단 절반에 있는가?
        bool isInUpperHalf = viewportPos.y > 0.5f;

        return isOnScreen && isInUpperHalf;
    }

    private void SetQuest() //temp
    {
        var filtered = App.GetData<TitleData>().Quest.Values.Where(q => q.Type == 2).ToList();

        if (filtered.Count == 0)
        {
            Debug.LogError("No quest data found");
            return;
        }
        
        _questData = filtered[Random.Range(0, filtered.Count)];
    }
    
    public override void OnLongPress() { }

    public override void OnClickUp() { }
}
