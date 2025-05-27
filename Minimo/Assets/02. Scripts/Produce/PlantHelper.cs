using System;
using System.Collections.Generic;

public class PlantHelper 
{
    private readonly TitleData _titleData = App.GetData<TitleData>();
    private readonly UseCashPanel _useCashPanel = App.GetManager<UIManager>().GetPanel<UseCashPanel>();

    public void TryPlant(
        ProduceData option,
        int optionIndex,
        int slotIndex,
        Action<ProduceTask, int> onTaskCreated)
    {
        var lackItems = GetLackItems(option.MaterialItems);

        if (lackItems.Count > 0)
        {
            _useCashPanel.OpenPanel(lackItems, async () =>
            {
                foreach (var item in lackItems)
                {
                    if (AccountInfo.Instance.items.ContainsKey(item.Item1))
                    {
                        AccountInfo.Instance.items[item.Item1] += item.Item2;
                    }
                    else
                    {
                        AccountInfo.Instance.items.Add(item.Item1, item.Item2);
                    }
                }
                CreateTaskAsync(option, optionIndex, slotIndex, onTaskCreated);
            });

            return;
        }

        CreateTaskAsync(option, optionIndex, slotIndex, onTaskCreated);
    }
    
    private List<(ItemData, int)> GetLackItems(ProduceMaterial[] materials)
    {
        var lackItems = new List<(ItemData, int)>();

        foreach (var material in materials)
        {
            var item = _titleData.Item[material.ID];
            if (AccountInfo.Instance.items.TryGetValue(item, out var value))
            {
                if (value < material.Amount)
                {
                    lackItems.Add((item, material.Amount - value));
                }
            }
        }

        return lackItems;
    }
    
    private void CreateTaskAsync(
        ProduceData option, 
        int optionIndex, 
        int slotIndex,
        Action<ProduceTask, int> onTaskCreated)
    {
        ConsumeMaterials(option.MaterialItems);

        var newTask = new ProduceTask(option, slotIndex);
        onTaskCreated?.Invoke(newTask, optionIndex);
    }

    private void ConsumeMaterials(ProduceMaterial[] materials)
    {
        foreach (var material in materials)
        {
            //_accountInfo.AddItemCount(material.ID, -material.Amount);
        }
    }
}
