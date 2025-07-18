public class ProduceTertiary : ProduceElevated
{
    protected override bool CreateBuilding()
    {
        _produceManager.RegisterTertiary(this);
        return base.CreateBuilding();
    }
}
