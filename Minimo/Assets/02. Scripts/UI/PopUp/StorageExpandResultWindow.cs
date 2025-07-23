public class StorageExpandResultWindow : ExpandResultWindow
{
    public override PopUpType Type() => PopUpType.StorageExpandResult;

    protected override string GetDescription(TitleData title) => title.GetString("STR_STORTAGE_UI_EXPAND_COMPLETE");

    protected override int GetCapacity() => AccountInfo.Instance.StorageCapacity;
}
