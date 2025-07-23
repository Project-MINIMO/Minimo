public class MinimoExpandResultWindow : ExpandResultWindow
{
    public override PopUpType Type() => PopUpType.MinimoExpandResult;

    protected override string GetDescription(TitleData title) => title.GetString("STR_MC_EXPANDSUCCEED_DESC");

    protected override int GetCapacity() => AccountInfo.Instance.MinimoCapacity;
}
