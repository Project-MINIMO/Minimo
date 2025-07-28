using System;
using System.Threading.Tasks;

public class ProduceTertiary : ProduceElevated
{
    public override async Task Initialize(Building data)
    {
        base.Initialize(data);
        
        _produceManager.RegisterTertiary(this);
    }

    public override void Destroy()
    {
        _produceManager.UnregisterTertiary(this);
        base.Destroy();
    }
}
