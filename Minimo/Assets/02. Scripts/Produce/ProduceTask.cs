using UnityEngine;
using Random = System.Random;

public class ProduceTask
{
    public ProduceData Data { get; }
    public float OriginalTime { get; }
    public float RemainTime => Mathf.Max(0, _modifiedTime - _elapsedTime);
    public ITaskState CurrentState { get; private set; }
    
    private readonly Random _random = new();

    private float _reducedTime;
    private float _modifiedTime;
    private float _elapsedTime;
    
    private float _reductionRatio;
    private float _harvestRatio;
    
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

    public void ApplyHarvestRatio(float harvestRatio)
    {
        _harvestRatio = harvestRatio;
    }
    
    public void ReduceRemainTime(float amount)
    {
        _elapsedTime += amount;
    }

    public void Exit()
    {
        CurrentState.OnExit(this);
    }

    public void ChangeState(ITaskState newState)
    {
        CurrentState = newState;
    }

    public void Harvest(TitleData titleData)
    {
        Debug.Log($"Harvested: {Data.ResultItems[0].ID}");
        
        var result = Data.ResultItems[0];
        var item = titleData.Item[result.ID];
        var amount = result.Amount;

        var bonus = 0;
        for (var i = 1; i <= amount; i++)
        {
            if (_random.NextDouble() < _harvestRatio)
            {
                bonus++;
            }
        }
        
        var finalAmount = amount + bonus;
        
        if (AccountInfo.Instance.items.ContainsKey(item))
        {
            AccountInfo.Instance.items[item] += finalAmount;
        }
        else
        {
            AccountInfo.Instance.items.Add(item, finalAmount);
        }
    }
}

public interface ITaskState
{
    void OnUpdate(ProduceTask task);
    void OnExit(ProduceTask task);
}

public class PendingState : ITaskState
{
    public static readonly PendingState Instance = new();
    private PendingState() { }
    
    public void OnUpdate(ProduceTask task) { }

    public void OnExit(ProduceTask task) { }
}

public class ActiveState : ITaskState
{
    public static readonly ActiveState Instance = new();
    private ActiveState() { }
    
    public void OnUpdate(ProduceTask task)
    {
        task.ReduceRemainTime(1);
    }

    public void OnExit(ProduceTask task)
    {
        task.ReduceRemainTime(task.RemainTime);
        task.ChangeState(CompletedState.Instance);
    }
}

public class CompletedState : ITaskState
{
    public static readonly CompletedState Instance = new();
    private readonly TitleData _titleData;

    private CompletedState()
    {
        _titleData = App.GetData<TitleData>();
    }
    
    public void OnUpdate(ProduceTask task) { }

    public void OnExit(ProduceTask task)
    {
        task.Harvest(_titleData);
        task.ChangeState(EndState.Instance);
    }
}

public class EndState : ITaskState
{
    public static readonly EndState Instance = new();
    private EndState() { }
    
    public void OnUpdate(ProduceTask task) { }

    public void OnExit(ProduceTask task) { }
}