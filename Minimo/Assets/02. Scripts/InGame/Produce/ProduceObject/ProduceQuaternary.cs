using System;

public class ProduceQuaternary : ProduceElevated
{
    public event Action OnMinimoPlaced;
    
    public override void PlaceMinimo(Minimo minimo)
    {
        base.PlaceMinimo(minimo);
        
        OnMinimoPlaced?.Invoke();
    }
}
