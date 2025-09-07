using System;
using System.Threading.Tasks;

public class ProduceTertiary : ProduceElevated
{
    public override async Task Initialize(Building data)
    {
        await base.Initialize(data);
        
        _produceManager.RegisterTertiary(this);
    }

    public override bool Destroy()
    {
        if (AssignedMinimo == null && AllTasks.Count == 0)
        {
            _produceManager.UnregisterTertiary(this);
        }
        
        return base.Destroy();
    }
}
