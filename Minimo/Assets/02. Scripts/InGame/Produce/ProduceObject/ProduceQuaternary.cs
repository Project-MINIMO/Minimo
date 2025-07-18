using System.Linq;

public class ProduceQuaternary : ProduceElevated
{
    public void StartPlant(Item[] materials)
    {
        var options = ProduceData
            .Where(x => x.MaterialItems[0].ID == materials[1].ID)
            .Where(x => x.MaterialItems[1].ID == materials[0].ID).ToList();

        if (options.Count == 0)  return;
        
        StartPlant(options[0]);
    }
}
