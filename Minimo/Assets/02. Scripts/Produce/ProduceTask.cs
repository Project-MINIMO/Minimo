using UnityEngine;

public class ProduceTask
{
    public ProduceData Data { get; }
    public int RemainTime { get; private set; }
    public ITaskState CurrentState { get; private set; }
    
    public ProduceTask(ProduceData produceOption)
    {
        Data = produceOption;
        RemainTime = produceOption.Time;
        
        CurrentState = PendingState.Instance;
    }

    public void Update()
    {
        CurrentState.OnUpdate(this);
    }

    public void Harvest()
    {
        CurrentState.OnHarvest(this);
    }

    public void ChangeState(ITaskState newState)
    {
        CurrentState = newState;
    }

    public void ReduceRemainTime(int amount)
    {
        RemainTime = Mathf.Max(0, RemainTime - amount);
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