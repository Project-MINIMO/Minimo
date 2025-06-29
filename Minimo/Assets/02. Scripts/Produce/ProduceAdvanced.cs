using UnityEngine;
using UniRx;

public abstract class ProduceAdvanced : ProduceObject
{
    public Transform MinimoWorkingPosition;
    public bool IsMinimoWorking => MinimoWorkingPosition != null && MinimoWorkingPosition.childCount > 0;
    public string AnimTrigger;

    private Minimo _placedMinimo;
    
    protected override void Awake()
    {
        base.Awake();

        MinimoWorkingPosition = transform.GetChild(2);
        
        App.GetManager<MinimoManager>()
            .GlobalTimeRatio
            .Subscribe(value =>
            {
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
        foreach (var task in AllTasks)
        {
            task.ApplyTimeReduction(reduction); 
        }
    }
}
