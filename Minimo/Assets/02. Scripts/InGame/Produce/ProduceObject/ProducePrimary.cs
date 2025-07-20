using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ProducePrimary : ProduceObject
{
    private SpriteRenderer _cropRenderer;
    private Dictionary<int, Sprite[]> _spritesMap;
    
    private int _currentID;
    private int _currentIndex;

    protected override void Awake()
    {
        base.Awake();
      
        _cropRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
    }
    
    public override async Task Initialize(Building data)
    {
        await base.Initialize(data);

        var config = Resources.Load<ProduceVisualConfig>($"Building/Config/{data.Code}");
        if (config == null)
        {
            Debug.LogError($"[ProducePrimary] Config not found for '{data.Name}'");
            _spritesMap = new Dictionary<int, Sprite[]>();
        }
        else
        {
            _spritesMap = new Dictionary<int, Sprite[]>(config.Sets.Length);
            foreach (var set in config.Sets)
            {
                _spritesMap[set.ID] = set.Sprites;
            }
        }
        
        if (ActiveTask != null)
        {
            BindTask(ActiveTask);
        }
    }
    
    public override ProduceTask CreateTask(ProduceData option)
    {
        var task = base.CreateTask(option);
        BindTask(task);
        return task;
    }

    private void OnDisable()
    {
        if (ActiveTask == null) return;
        
        ActiveTask.OnRemainTimeChanged -= OnRemainTimeChanged;
        ActiveTask.OnStateChanged -= OnStateChanged;
    }

    private void BindTask(ProduceTask task)
    {
        task.OnRemainTimeChanged += OnRemainTimeChanged;
        task.OnStateChanged += OnStateChanged;

        SetupSprites(task);
    }
    
    private void SetupSprites(ProduceTask task)
    {
        _currentID = task.Data.ResultItems[0].ID;
        _currentIndex = -1;
        SetCropSprite(ActiveTask.RemainTime, ActiveTask.ModifiedTime);
    }

    private void SetCropSprite(float remain, float full)
    {
        var ratio = full > 0 ? remain / full : 0f;
        var idx = ratio >= 0.5f ? 0
            : ratio > 0 ? 1
            : 2;

        if (idx == _currentIndex) return;
        
        _currentIndex = idx;
        _cropRenderer.sprite = _spritesMap[_currentID][_currentIndex];
    }
    
    private void OnRemainTimeChanged(float remain)
    {
        SetCropSprite(remain, ActiveTask.ModifiedTime);
    }
    
    private void OnStateChanged(ITaskState state)
    {
        if (state == CompletedState.Instance)
        {
            SetCropSprite(AllTasks[0].RemainTime, AllTasks[0].ModifiedTime);
            AllTasks[0].OnRemainTimeChanged -= OnRemainTimeChanged;
        }
        else if (state == EndState.Instance)
        {
            _cropRenderer.sprite = null;
            AllTasks[0].OnStateChanged -= OnStateChanged;
        }
    }
}
