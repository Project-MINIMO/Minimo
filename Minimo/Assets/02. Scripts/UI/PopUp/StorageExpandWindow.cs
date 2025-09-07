public class StorageExpandWindow : ExpandWindow
{
    public override PopUpType Type() => PopUpType.StorageExpand;

    protected override string GetTitle(TitleData title) => title.GetString("STR_STORAGE_UI_NAME");
    protected override string GetDescription(TitleData title) => title.GetString("STR_STORTAGE_UI_EXPAND_DESC");

    protected override PopUpType GetPopUpType() => PopUpType.StorageExpandResult;
}
