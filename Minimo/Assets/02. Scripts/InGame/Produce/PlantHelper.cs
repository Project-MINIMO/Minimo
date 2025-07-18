using System;
using System.Collections.Generic;

public class PlantHelper
{
    private readonly UseCashPanel _useCashPanel = App.GetManager<UIManager>().GetPanel<UseCashPanel>();

    public void TryPlant(ProduceData option, Action<ProduceTask> onTaskCreated)
    {
        var lackItems = GetLackItems(option.MaterialItems);

        if (lackItems.Count > 0)
        {
            _useCashPanel.OpenPanel(lackItems, () =>
            {
                foreach (var item in lackItems)
                {
                    AccountInfo.Instance.AddItem(item.Item1, item.Item2);
                }
                CreateTask(option, onTaskCreated);
            });

            return;
        }

        CreateTask(option, onTaskCreated);
    }
    
    private List<(Item, int)> GetLackItems(ProduceMaterial[] materials)
    {
        var lackItems = new List<(Item, int)>();

        foreach (var material in materials)
        {
            var item = AccountInfo.Instance.Items[material.ID];
            if (item.Count < material.Amount)
            {
                lackItems.Add((item, material.Amount - item.Count));
            }
        }

        return lackItems;
    }
    
    private void CreateTask(ProduceData option, Action<ProduceTask> onTaskCreated)
    {
        ConsumeMaterials(option.MaterialItems);

        var newTask = new ProduceTask(option);
        onTaskCreated?.Invoke(newTask);
    }

    private void ConsumeMaterials(ProduceMaterial[] materials)
    {
        foreach (var material in materials)
        {
            AccountInfo.Instance.RemoveItem(material.ID, material.Amount);
        }
    }
}
