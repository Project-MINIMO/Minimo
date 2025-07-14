using System.Linq;

public class ProduceQuaternary : ProduceTertiary
{
    public void StartPlant(int food, int flower)
    {
        var item = ProduceData
            .Where(x => x.MaterialItems[0].ID == flower)
            .Where(x => x.MaterialItems[1].ID == food).ToList();

        if (item.Count == 0)  return;
        
        StartPlant(item[0]);
    }
}
