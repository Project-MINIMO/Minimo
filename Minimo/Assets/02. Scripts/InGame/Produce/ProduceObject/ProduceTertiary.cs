using System.Threading.Tasks;

public class ProduceTertiary : ProduceElevated
{
    protected override Task<bool> CreateBuilding()
    {
        _produceManager.RegisterTertiary(this);
        return base.CreateBuilding();
    }
}
