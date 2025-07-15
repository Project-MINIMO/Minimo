using System;
using System.Collections.Generic;

public class PlantHelper
{
    private readonly TitleData _titleData = App.GetData<TitleData>();
    private readonly UseCashPanel _useCashPanel = App.GetManager<UIManager>().GetPanel<UseCashPanel>();

    public void TryPlant(
        ProduceData option,
        int optionIndex,
        Action<ProduceTask, int> onTaskCreated)
    {
        var lackItems = GetLackItems(option.MaterialItems);

        if (lackItems.Count > 0)
        {
            _useCashPanel.OpenPanel(lackItems, async () =>
            {
                foreach (var item in lackItems)
                {
                    AccountInfo.Instance.AddItem(item.Item1.ID, item.Item2);
                }
                CreateTaskAsync(option, optionIndex, onTaskCreated);
            });

            return;
        }

        CreateTaskAsync(option, optionIndex, onTaskCreated);
    }
    
    private List<(ItemData, int)> GetLackItems(ProduceMaterial[] materials)
    {
        var lackItems = new List<(ItemData, int)>();

        foreach (var material in materials)
        {
            var item = _titleData.Item[material.ID];
            if (AccountInfo.Instance.Items.TryGetValue(material.ID, out var value))
            {
                if (value.Count < material.Amount)
                {
                    lackItems.Add((item, material.Amount - value.Count));
                }
            }
            else
            {
                lackItems.Add((item, material.Amount));
            }
        }

        return lackItems;
    }
    
    private void CreateTaskAsync(
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
