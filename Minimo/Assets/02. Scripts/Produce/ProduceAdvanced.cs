using UnityEngine;
using UniRx;

public abstract class ProduceAdvanced : ProduceObject
{
    public Transform MinimoWorkingPosition;
    public bool IsMinimoWorking => MinimoWorkingPosition != null && MinimoWorkingPosition.childCount > 0;
    public string AnimTrigger;

    private Minimo _placedMinimo;
    private float _timeReduction;
    
    protected override void Awake()
    {
        base.Awake();

        MinimoWorkingPosition = transform.GetChild(2);
        
        App.GetManager<MinimoManager>()
            .GlobalTimeReduction
            .Subscribe(value =>
            {
                _timeReduction = value;
                foreach (var task in AllTasks)
                {
                    task.ApplyTimeReduction(value); 
                }
            })
            .AddTo(this);
    }

    public void PlaceMinimo(Minimo minimo)
    {
        if (_placedMinimo != null)
        {
            _placedMinimo.SetChillState();
        }
        
        _placedMinimo = minimo;
    }

    public void ApplyTimeReduction(float reduction)
    { 
        _timeReduction = reduction;
        
        foreach (var task in AllTasks)
        {
            task.ApplyTimeReduction(reduction); 
        }
    }
    
    internal override void OnPlant(ProduceTask task)
    {
        task.ApplyTimeReduction(_timeReduction);
        
        base.OnPlant(task);
    }
}
