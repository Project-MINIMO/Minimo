using System.Linq;

public class ProduceQuaternary : ProduceTertiary
{
    private WishPanel _wishPanel;
    
    protected override void Awake()
    {
        base.Awake();
        
        _wishPanel = App.GetManager<UIManager>().GetPanel<WishPanel>();
    }

    public void StartPlant(int food, int flower)
    {
        var item = ProduceData
            .Where(x => x.MaterialItems[0].ID == flower)
            .Where(x => x.MaterialItems[1].ID == food).ToList();

        if (item.Count == 0)  return;
        
        StartPlant(item[0]);
    }

    public override void OpenUI()
    {
        _wishPanel.OpenPanel();
    }
    
    public override void CloseUI()
    {
        _wishPanel.ClosePanel();
    }
}
