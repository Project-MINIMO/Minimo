using System;

public class ProduceQuaternary : ProduceElevated
{
    public event Action OnMinimoPlaced;
    
    public override void PlaceMinimo(MinimoObject minimo)
    {
        base.PlaceMinimo(minimo);
        
        OnMinimoPlaced?.Invoke();
    }
    
    public override void UnplaceMinimo()
    {
        base.UnplaceMinimo();
        
        OnMinimoPlaced?.Invoke();
    }
}
