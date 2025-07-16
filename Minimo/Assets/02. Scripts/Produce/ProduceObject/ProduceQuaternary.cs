using System.Linq;

public class ProduceQuaternary : ProduceElevated
{
    public void StartPlant(int flower, int food)
    {
        var options = ProduceData
            .Where(x => x.MaterialItems[0].ID == flower)
            .Where(x => x.MaterialItems[1].ID == food).ToList();

        if (options.Count == 0)  return;
        
        StartPlant(options[0]);
    }
}
