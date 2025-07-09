using UnityEngine;

using Random = System.Random;

public class ProduceTask
{
    public ProduceData Data { get; }
    public ITaskState CurrentState { get; private set; }
    
    public float RemainTime => Mathf.Max(0, _modifiedTime - ElapsedTime);
    public float ModifiedTime => _isModifiedDirty ? RecalculateModifiedTime() : _modifiedTime;
    private float ElapsedTime => CurrentState is ActiveState activeState ? activeState.ElapsedTime : 0;
    
    public float HarvestRatio { get; private set; }
    
    private readonly float _maxReducedTime;
    private readonly float _baseTime;
    
    private float _reducedTime;
    private float _modifiedTime;
    private float _timeRatio;
    
    private bool _isModifiedDirty = true;
    
    public ProduceTask(ProduceData produceOption)
    {
        Data = produceOption;
        
        _baseTime = produceOption.Time;
        _reducedTime = produceOption.Time;
        _modifiedTime = _baseTime;

        _maxReducedTime = _baseTime * (1 - App.GetData<TitleData>().Common["ProdTimeReduceCap"] / 100f);

        ChangeState(PendingState.Instance);
    }
    
    public void ChangeState(ITaskState newState)
    {
        CurrentState?.OnExit(this);
        CurrentState = newState;
        CurrentState.OnEnter(this);
    }
    
    public void Update() => CurrentState.OnUpdate(this);
    
    private float RecalculateModifiedTime()
    {
        _modifiedTime = Mathf.Max(_maxReducedTime, _reducedTime * _timeRatio);
        _isModifiedDirty = false;
        
        return _modifiedTime;
    }

    #region Apply Minimo Abilities
    public void ApplyTimeRatio(float reductionRatio)
    {
        _timeRatio = reductionRatio;
        _isModifiedDirty = true;
        
        App.LogBox("red", "생산 시간 로그", new()
        {
            { "원본 생산 시간", _baseTime.ToString() },
            { "수정된 생산 시간", _reducedTime.ToString() },
            { "생산 시간 감소 비율", _timeRatio.ToString() },
            { "최종 생산 시간 (비율 적용)", (_reducedTime * _timeRatio).ToString() },
            { "최종 생산 시간 (캡값 적용)", _modifiedTime.ToString() }
        });
    }

    public void ApplyTimeReduction(float reductionAmount)
    {
        _reducedTime = Mathf.Max(0, _baseTime - reductionAmount);
        _isModifiedDirty = true;
    }

    public void ApplyHarvestRatio(float harvestRatio)
    {
        HarvestRatio = 1f - harvestRatio;
    }
    #endregion
}

public interface ITaskState
{
    void OnEnter(ProduceTask produceTask);
    void OnUpdate(ProduceTask task);
    void OnExit(ProduceTask task);
}

public class PendingState : ITaskState
{
    public static readonly PendingState Instance = new();
    private PendingState() { }
    
    public void OnEnter(ProduceTask task) { }
    public void OnUpdate(ProduceTask task) { }
    public void OnExit(ProduceTask task) { }
}

public class ActiveState : ITaskState
{
    public float ElapsedTime { get; private set; }

    public static readonly ActiveState Instance = new();
    private ActiveState() { }
    
    public void OnEnter(ProduceTask task)
    {
        ElapsedTime = 0f;
    }
    
    public void OnUpdate(ProduceTask task)
    {
        ElapsedTime += 0.1f;
    }

    public void OnExit(ProduceTask task)
    {
        ElapsedTime = task.ModifiedTime;
    }
}

public class CompletedState : ITaskState
{
    public static readonly CompletedState Instance = new();

    private CompletedState() { }
    
    public void OnEnter(ProduceTask task) { }
    public void OnUpdate(ProduceTask task) { }
    public void OnExit(ProduceTask task)
    {
        TryHarvest(task);
    }
    
    private void TryHarvest(ProduceTask task)
    {
        var result = task.Data.ResultItems[0];
        var bonus = CalculateBonus(result.Amount, task.HarvestRatio);

        AccountInfo.Instance.AddItem(result.ID, result.Amount + bonus);
        
        App.LogBox("yellow", "생산물 추가 수확 로그", new()
        {
            { "기존 수확량", result.Amount.ToString() },
            { "추가 생산 확률", task.HarvestRatio.ToString() },
            { "추가 수확량", bonus.ToString() },
            { "최종 수확량", (result.Amount + bonus).ToString() },
        });
    }

    private int CalculateBonus(int amount, float chance)
    {
        var random = new Random();
        var bonus = 0;
        
        for (var i = 0; i < amount; i++)
        {
            if (random.NextDouble() < chance) bonus++;
        }
        
        return bonus;
    }
}

public class EndState : ITaskState
{
    public static readonly EndState Instance = new();
    private EndState() { }
    
    public void OnEnter(ProduceTask task) { }
    public void OnUpdate(ProduceTask task) { }
    public void OnExit(ProduceTask task) { }
}