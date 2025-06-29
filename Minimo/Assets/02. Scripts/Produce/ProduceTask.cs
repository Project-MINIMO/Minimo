using UnityEngine;

public class ProduceTask
{
    public ProduceData Data { get; }
    public float OriginalTime { get; }
    public float RemainTime => Mathf.Max(0, _modifiedTime - _elapsedTime);
    public ITaskState CurrentState { get; private set; }

    private float _reducedTime;
    private float _modifiedTime;
    private float _elapsedTime;
    private float _reductionRatio;
    
    public ProduceTask(ProduceData produceOption)
    {
        Data = produceOption;
        
        OriginalTime = produceOption.Time;
        _reducedTime = produceOption.Time;
        _modifiedTime = OriginalTime;
        
        CurrentState = PendingState.Instance;
    }

    public void Update()
    {
        CurrentState.OnUpdate(this);
    }
    
    public void ApplyTimeRatio(float reductionRatio)
    {
        _reductionRatio = reductionRatio;
        _modifiedTime = _reducedTime * reductionRatio;
    }

    public void ApplyTimeReduction(float reductionAmount)
    {
        _reducedTime = Mathf.Max(0, OriginalTime - reductionAmount);
        _modifiedTime = _reducedTime * _reductionRatio;
    }
    
    public void ReduceRemainTime(float amount)
    {
        _elapsedTime += amount;
    }

    public void Harvest()
    {
        CurrentState.OnHarvest(this);
    }

    public void ChangeState(ITaskState newState)
    {
        CurrentState = newState;
    }
}

public interface ITaskState
{
    void OnUpdate(ProduceTask task);
    void OnHarvest(ProduceTask task);
}

public class PendingState : ITaskState
{
    public static readonly PendingState Instance = new();
    private PendingState() { }
    
    public void OnUpdate(ProduceTask task) { }

    public void OnHarvest(ProduceTask task) { }
}

public class ActiveState : ITaskState
{
    public static readonly ActiveState Instance = new();
    private ActiveState() { }
    
    public void OnUpdate(ProduceTask task)
    {
        task.ReduceRemainTime(1);
    }

    public void OnHarvest(ProduceTask task)
    {
        task.ReduceRemainTime(task.RemainTime);
        task.ChangeState(CompletedState.Instance);
    }
}

public class CompletedState : ITaskState
{
    public static readonly CompletedState Instance = new();
    private TitleData _titleData;

    private CompletedState()
    {
        _titleData = App.GetData<TitleData>();
    }
    
    public void OnUpdate(ProduceTask task) { }

    public void OnHarvest(ProduceTask task)
    {
        Debug.Log($"Harvested: {task.Data.ResultItems[0].ID}");
        
        var item = _titleData.Item[task.Data.ResultItems[0].ID];
        if (AccountInfo.Instance.items.ContainsKey(item))
        {
            AccountInfo.Instance.items[item] += task.Data.ResultItems[0].Amount;
        }
        else
        {
            AccountInfo.Instance.items.Add(item, task.Data.ResultItems[0].Amount);
        }
        
        task.ChangeState(EndState.Instance);
    }
}

public class EndState : ITaskState
{
    public static readonly EndState Instance = new();
    private EndState() { }
    
    public void OnUpdate(ProduceTask task) { }

    public void OnHarvest(ProduceTask task) { }
}