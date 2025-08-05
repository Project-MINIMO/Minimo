public class MinimoCapacityHandler : CapacityHandler
{
    protected override int GetExpandCost(TitleData title) => title.Common["ResidenceExpandCost"];
    protected override int GetCurrentCapacity() => AccountInfo.Instance.MinimoCapacity;
    protected override int GetBaseCapacity() => _minimoManager.ActiveMinimos.Count;

    private MinimoManager _minimoManager;
    
    protected override void Awake()
    {
        base.Awake();

        _minimoManager = App.GetManager<MinimoManager>();
    }
    
    protected override void SuccessTransaction()
    {
        AccountInfo.Instance.AddMinimoCapacity(Quantity - CurrentCapacity);
    }
}