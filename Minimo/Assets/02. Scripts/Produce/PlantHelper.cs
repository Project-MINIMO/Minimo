using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MinimoShared;

public class PlantHelper 
{
    private readonly ItemSO _itemSO = App.GetData<TitleData>().ItemSO;
    private readonly UseCashPanel _useCashPanel = App.GetManager<UIManager>().GetPanel<UseCashPanel>();
    private readonly AccountInfoManager _accountInfo = App.GetManager<AccountInfoManager>();

    public void TryPlant(
        ProduceData option,
        int optionIndex,
        int slotIndex,
        Func<ProduceTask, int, UniTask> onTaskCreated)
    {
        var lackItems = GetLackItems(option.MaterialItems);

        if (lackItems.Count > 0)
        {
            _useCashPanel.OpenPanel(lackItems, async () =>
            {
                foreach (var item in lackItems)
                {
                    _accountInfo.AddItemCount(item.Item1.Code, item.Item2);
                }
                await CreateTaskAsync(option, optionIndex, slotIndex, onTaskCreated);
            });

            return;
        }

        CreateTaskAsync(option, optionIndex, slotIndex, onTaskCreated).Forget();
    }
    
    private List<(Item, int)> GetLackItems(ProduceMaterial[] materials)
    {
        var lackItems = new List<(Item, int)>();

        foreach (var material in materials)
        {
            var item = _itemSO.GetItem(material.ID);
            var itemDTO = _accountInfo.GetItem(item.Code);
            if (itemDTO.Count < material.Amount)
            {
                lackItems.Add((item, material.Amount - itemDTO.Count));
            }
        }

        return lackItems;
    }
    
    private async UniTask CreateTaskAsync(
        ProduceData option, 
        int optionIndex, 
        int slotIndex,
        Func<ProduceTask, int, UniTask> onTaskCreated)
    {
        ConsumeMaterials(option.MaterialItems);

        var newTask = new ProduceTask(option, slotIndex);
        await onTaskCreated(newTask, optionIndex);
    }

    private void ConsumeMaterials(ProduceMaterial[] materials)
    {
        foreach (var material in materials)
        {
            _accountInfo.AddItemCount(material.ID, -material.Amount);
        }
    }
}
