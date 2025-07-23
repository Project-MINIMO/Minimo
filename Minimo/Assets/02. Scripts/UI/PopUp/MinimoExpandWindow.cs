public class MinimoExpandWindow : ExpandWindow
{
    public override PopUpType Type() => PopUpType.MinimoExpand;

    protected override string GetTitle(TitleData title) => title.GetString("STR_MINIMOCENTER_NAME");
    protected override string GetDescription(TitleData title) => title.GetString("STR_MC_RESIDENCEEXPAND_DESC");

    protected override PopUpType GetPopUpType() => PopUpType.MinimoExpandResult;
}
