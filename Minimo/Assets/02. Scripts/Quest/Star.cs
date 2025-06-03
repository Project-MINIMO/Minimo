using System.Linq;

using UnityEngine;

public class Star : InteractObject
{
    private QuestData _questData;
    private ScreenStateManager _screenStateManager;
    
    private SpriteRenderer _renderer;
    
    private bool _eventTriggered;
    private float _lastUpdateTime;

    private void Awake()
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

        if (_renderer.isVisible)
        {
            _eventTriggered = true;
            App.GetManager<QuestManager>().AddQuest(_questData);
        }
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
