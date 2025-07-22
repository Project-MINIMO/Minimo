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
    
    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn;
    
    private readonly Queue<MinimoAssignedSlot> _slotPool = new();
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
        var slot = _slotPool.Dequeue();

        slot.gameObject.SetActive(true);
        slot.Initialize(building);
    }

    private void OnItemSelected(InventorySlot<ProduceAdvanced> slot)
    {
        _placePanel.OpenPanel(slot.Item);
    }
}
