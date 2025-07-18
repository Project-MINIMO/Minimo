using UnityEngine;
using UniRx;

public abstract class ProduceAdvanced : ProduceObject
{
    public Transform MinimoWorkingPosition;
    public bool IsMinimoWorking => MinimoWorkingPosition != null && MinimoWorkingPosition.childCount > 0;
    public string AnimTrigger;

    private Minimo _placedMinimo;
    private float _timeReduction;
    private float _globalTimeReduction;
    
    protected override void Awake()
    {
        base.Awake();

        MinimoWorkingPosition = transform.GetChild(2);
        
        App.GetManager<MinimoManager>()
            .GlobalTimeReduction
            .Subscribe(value =>
            {
                _globalTimeReduction = value;
                
                foreach (var task in AllTasks)
                {
                    task.ApplyTimeReduction(_timeReduction + _globalTimeReduction); 
                }
            })
            .AddTo(this);
    }

    public void PlaceMinimo(Minimo minimo)
    {
        _placedMinimo?.SetChillState();
        _placedMinimo = minimo;
    }

    public void ApplyTimeReduction(float reduction)
    { 
        _timeReduction = reduction;
        
        foreach (var task in AllTasks)
        {
            task.ApplyTimeReduction(_timeReduction + _globalTimeReduction); 
        }
    }

    internal override void StartPlant(ProduceData option)
    {
        if (_placedMinimo == null) return;
        
        base.StartPlant(option);
    }
    
    internal override void OnPlant(ProduceTask task)
    {
        task.ApplyTimeReduction(_timeReduction + _globalTimeReduction);
        
        base.OnPlant(task);
    }
}
