using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class QuizSubmissionView : QuestSubmissionView
{
    [SerializeField] private Button _selectBtn;
    
    private Item _selectedItem;
    
    public override void Initialize(QuestManager questManager, TitleData titleData)
    {
        base.Initialize(questManager, titleData);
        
        var itemSlots = GetComponentsInChildren<ItemSlot>(true).ToList();
        foreach (var slot in itemSlots)
        {
            slot.OnItemSelected += OnItemSelected;
        }

        _selectBtn.onClick.AddListener(ClearItem);
    }
    
    public override void Setup(Quest quest)
    {
        base.Setup(quest);

        ClearItem();
    }

    protected override void Submit()
    {
        if (_selectedItem == null) return;

        QuestManager.SubmitQuest(_selectedItem);
    }

    private void OnItemSelected(InventorySlot<Item> slot)
    {
        _selectedItem = slot.Item;
        _infoUpdaters[0].UpdateItem(slot.Item);
    }

    private void ClearItem()
    {
        _selectedItem = null;
        _infoUpdaters[0].ClearItem();
    }
}