using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class QuizSubmissionView : QuestSubmissionView
{
    [SerializeField] private Button _selectBtn;
    [SerializeField] private MenuToggleGroup _toggleGroup;
    
    private Item _selectedItem;
    
    public override void Initialize(QuestManager questManager, QuestSubmissionPanel submissionPanel, TitleData titleData)
    {
        base.Initialize(questManager, submissionPanel, titleData);
        
        _selectBtn.onClick.AddListener(ClearItem);
    }

    private void Start()
    {
        var itemSlots = GetComponentsInChildren<ItemSlot>(true).ToList();
        foreach (var slot in itemSlots)
        {
            slot.OnItemSelected += OnItemSelected;
        }
    }
    
    public override void Setup(Quest quest)
    {
        base.Setup(quest);

        ClearItem();
        _toggleGroup.Show(true);
    }

    protected override void Submit()
    {
        if (_selectedItem == null) return;

        QuestManager.SubmitQuest(_selectedItem);
        base.Submit();
    }

    private void OnItemSelected(InventorySlot<Item> slot)
    {
        _selectedItem = slot.Item;
        SubmissionSlots[0].Initialize(slot.Item.Icon);
    }

    private void ClearItem()
    {
        _selectedItem = null;
        SubmissionSlots[0].ClearItem();
    }
}