using System.Linq;
using System.Collections.Generic;

using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinimoAssignedPanel : UIBase
{
    public override bool IsUseBlur => true;
            
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _descriptionTMP;
    [SerializeField] private RectTransform _content;
    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn;
    
    private readonly Queue<MinimoAssignedSlot> _slotPool = new();
    private readonly List<MinimoAssignedSlot> _activeSlots = new();
    private MinimoPlacePanel _placePanel;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _placePanel = manager.GetPanel<MinimoPlacePanel>();

        App.GetManager<EditManager>().ActiveAdvanceds.ObserveAdd()
            .Subscribe(addEvent => AssignSlot(addEvent.Value))
            .AddTo(this);
        
        var slots = GetComponentsInChildren<MinimoAssignedSlot>(true).ToList();
        foreach (var slot in slots)
        {
            slot.OnItemSelected += OnItemSelected;
            slot.gameObject.SetActive(false);
            _slotPool.Enqueue(slot);
        }

        _titleTMP.text = App.GetData<TitleData>().GetString("STR_POPUP_PRODUCEPLACE_NAME");
        _descriptionTMP.text = App.GetData<TitleData>().GetString("STR_POPUP_PRODUCEPLACE_DESC");
        
        _openBtn.onClick.AddListener(OpenPanel);
        _closeBtn.onClick.AddListener(ClosePanel);
    }
    
    private void AssignSlot(ProduceAdvanced building)
    {
        var activeSlot = _slotPool.Dequeue();

        activeSlot.gameObject.SetActive(true);
        activeSlot.Initialize(building);
        _activeSlots.Add(activeSlot);
        
        var sorted = _activeSlots.OrderBy(slot => slot.Item.BuildingData.ID).ToList();
        for (var i = 0; i < sorted.Count; i++)
        {
            sorted[i].transform.SetSiblingIndex(i);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
    }

    private void OnItemSelected(InventorySlot<ProduceAdvanced> slot)
    {
        _placePanel.OpenPanel(slot.Item);
    }
}
