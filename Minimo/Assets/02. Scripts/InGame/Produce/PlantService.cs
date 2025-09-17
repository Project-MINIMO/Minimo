using System;
using System.Collections.Generic;

public class PlantService
{
    private readonly UseCashPanel _useCashPanel;

    public PlantService(UseCashPanel useCashPanel)
    {
        _useCashPanel = useCashPanel;
    }
    
    public void TryPlant(
        ProduceObject target,
        ProduceData option,
        Action<ProduceTask> onSuccess,
        Action<NotifyType> onFailed)
    {
        var preCheck = target.CheckPlantCondition(option);
        if (preCheck != NotifyType.Success)
        {
            onFailed?.Invoke(preCheck);
            return;
        }

        var lack = GetLackItems(option.MaterialItems);
        if (lack.Count > 0)
        {
            _useCashPanel.OpenPanel(
                lack,
                onConfirm: () =>
                {
                    foreach (var (id, cnt) in lack)
                    {
                        AccountInfo.Instance.AddItem(id, cnt);
                    }
                    CreateTask(target, option, onSuccess);
                },
                onCancel: () =>
                {
                    onFailed?.Invoke(NotifyType.MaterialLack);
                }
            );
            return;
        }
        
        CreateTask(target, option, onSuccess);
    }

    private List<(Item item, int amount)> GetLackItems(ProduceMaterial[] materials)
    {
        var list = new List<(Item, int)>();
        
        foreach (var material in materials)
        {
            var item = AccountInfo.Instance.Items[material.ID];
            if (item.Count < material.Amount)
            {
                list.Add((item, material.Amount - item.Count));
            }
        }
        
        return list;
    }
    
    private void CreateTask(ProduceObject obj, ProduceData option, Action<ProduceTask> onSuccess)
    {
        ConsumeMaterials(option.MaterialItems);
        var task = obj.CreateTask(option);
        onSuccess?.Invoke(task);
    }

    private void ConsumeMaterials(ProduceMaterial[] materials)
    {
        foreach (var material in materials)
        {
            AccountInfo.Instance.RemoveItem(material.ID, material.Amount);
        }
    }
}
