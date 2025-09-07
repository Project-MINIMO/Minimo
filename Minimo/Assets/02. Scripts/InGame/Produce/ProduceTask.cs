using System;
using UnityEditor;
using UnityEngine;

using Random = System.Random;

public class ProduceTask
{
    public ProduceData Data { get; }

    public ITaskState CurrentState { get; private set; }
    
    public event Action<ITaskState> OnStateChanged;
    public event Action<float> OnRemainTimeChanged;
    
    public float RemainTime => Mathf.Max(0, ModifiedTime - ElapsedTime);
    public float ModifiedTime => _isModifiedDirty ? RecalculateModifiedTime() : _modifiedTime;
    public float ElapsedTime;
    public Transform ProduceTransform;
    
    private readonly float _maxReducedTime;
    private readonly float _baseTime;
    
    private float _reducedTime;
    private float _modifiedTime;
    private float _timeRatio;
    
    public float HarvestRatio { get; private set; }
    public float ExpRatio { get; private set; }
    
    private bool _isModifiedDirty = true;
    
    public ProduceTask(ProduceData produceOption, Transform trans)
    {
        Data = produceOption;
        ProduceTransform = trans;
        
        _baseTime = produceOption.Time;
        _reducedTime = produceOption.Time;
        _modifiedTime = _baseTime;

        _maxReducedTime = _baseTime * (1 - App.GetData<TitleData>().Common["ProdTimeReduceCap"] / 100f);

        ChangeState(PendingState.Instance);
    }

    public void Update()
    {
        CurrentState.OnUpdate(this);
        
        OnRemainTimeChanged?.Invoke(RemainTime);

        if (CurrentState is ActiveState && RemainTime <= 0f)
        {
            ChangeState(CompletedState.Instance);
        }
    }
    
    public void ChangeState(ITaskState newState)
    {
        CurrentState?.OnExit(this);
        CurrentState = newState;
        CurrentState.OnEnter(this);

        OnStateChanged?.Invoke(newState);
    }

    public void ChangeStateWithoutNotify(ITaskState newState)
    {
        CurrentState = newState;
        CurrentState.OnEnter(this);

        OnStateChanged?.Invoke(newState);
    }
    
    private float RecalculateModifiedTime()
    {
        _modifiedTime = Mathf.Max(_maxReducedTime, _reducedTime * _timeRatio);
        _isModifiedDirty = false;
        
        App.LogBox("red", "생산 시간 로그", new()
        {
            { "원본 생산 시간", _baseTime.ToString() },
            { "수정된 생산 시간", _reducedTime.ToString() },
            { "생산 시간 감소 비율", _timeRatio.ToString() },
            { "최종 생산 시간 (비율 적용)", (_reducedTime * _timeRatio).ToString() },
            { "최종 생산 시간 (캡값 적용)", _modifiedTime.ToString() }
        });
        
        return _modifiedTime;
    }

    #region Apply Minimo Abilities
    public void ApplyTimeRatio(float reductionRatio)
    {
        if (CurrentState is CompletedState or EndState) return;
        
        _timeRatio = reductionRatio;
        _isModifiedDirty = true;
    }

    public void ApplyTimeReduction(float reductionAmount)
    {
        if (CurrentState is CompletedState or EndState) return;
        
        _reducedTime = Mathf.Max(0, _baseTime - reductionAmount);
        _isModifiedDirty = true;
    }

    public void ApplyHarvestRatio(float harvestRatio)
    {
        HarvestRatio = 1f - harvestRatio;
    }

    public void ApplyExpRatio(float expRatio)
    {
        ExpRatio = expRatio;
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
    public static readonly ActiveState Instance = new();
    private ActiveState() { }

    public void OnEnter(ProduceTask task)
    {
        task.ElapsedTime = 0;
    }
    public void OnUpdate(ProduceTask task)
    {
        task.ElapsedTime += 0.1f;
    }
    public void OnExit(ProduceTask task)
    {
        task.ElapsedTime = task.ModifiedTime;
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
        AccountInfo.Instance.AddItem(result.ID, result.Amount + bonus, task.ProduceTransform);
        App.LogBox("yellow", "생산물 추가 수확 로그", new()
        {
            { "기존 수확량", result.Amount.ToString() },
            { "추가 생산 확률", task.HarvestRatio.ToString() },
            { "추가 수확량", bonus.ToString() },
            { "최종 수확량", (result.Amount + bonus).ToString() },
        });

        var expAmount = task.Data.EXP;
        var modifiedExp = expAmount * task.ExpRatio;
        var roundedExp = Mathf.RoundToInt(modifiedExp);
        AccountInfo.Instance.Level.AddCount(roundedExp);
        App.LogBox("purple", "획득 경험치 비율 증가 로그", new()
        {
            { "기존 경험치", expAmount.ToString() },
            { "획득 경험치 증가 비율", task.ExpRatio.ToString() },
            { "재계산된 경험치", modifiedExp.ToString() },
            { "반올림된 최종 경험치", roundedExp.ToString() },
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