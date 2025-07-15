using System;
using System.Collections.Generic;

public class PlantHelper
{
    private readonly UseCashPanel _useCashPanel = App.GetManager<UIManager>().GetPanel<UseCashPanel>();

    public void TryPlant(
        ProduceData option,
        int optionIndex,
        Action<ProduceTask, int> onTaskCreated)
    {
        var lackItems = GetLackItems(option.MaterialItems);

        if (lackItems.Count > 0)
        {
            _useCashPanel.OpenPanel(lackItems, () =>
            {
                foreach (var item in lackItems)
                {
                    AccountInfo.Instance.AddItem(item.Item1.Data.ID, item.Item2);
                }
                CreateTask(option, optionIndex, onTaskCreated);
            });

            return;
        }

        CreateTask(option, optionIndex, onTaskCreated);
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
    
    private void CreateTask(
        ProduceData option, 
        int optionIndex, 
        Action<ProduceTask, int> onTaskCreated)
    {
        ConsumeMaterials(option.MaterialItems);

        var newTask = new ProduceTask(option);
        onTaskCreated?.Invoke(newTask, optionIndex);
    }

    private void ConsumeMaterials(ProduceMaterial[] materials)
    {
        foreach (var material in materials)
        {
            AccountInfo.Instance.RemoveItem(material.ID, material.Amount);
        }
    }
}
