using System.Linq;
using System.Collections.Generic;

using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlaceByMinimoPanel : UIBase
{
    public override bool IsUseBlur => true;
    
    [SerializeField] private MinimoAssignedSlot _slotPrefab;
    [SerializeField] private RectTransform _contentParent;
    
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _descriptionTMP;
    [SerializeField] private RectTransform _content;
    
    [SerializeField] private Button _closeBtn;
    
    private readonly Queue<MinimoAssignedSlot> _slotPool = new();
    private readonly List<MinimoAssignedSlot> _activeSlots = new();
    
    private PopUpPanel _popUpPanel;
    private Minimo _currentMinimo;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        var editManager = App.GetManager<EditManager>();
        editManager.ActiveProduces.ObserveAdd()
            .Subscribe(addEvent => AssignSlot(addEvent.Value))
            .AddTo(this);
        editManager.ActiveProduces.ObserveRemove()
            .Subscribe(removeEvent => ReleaseSlot(removeEvent.Value))
            .AddTo(this);
        
        var slots = GetComponentsInChildren<MinimoAssignedSlot>(true).ToList();
        foreach (var slot in slots)
        {
            slot.OnItemSelected += OnItemSelected;
            slot.gameObject.SetActive(false);
            _slotPool.Enqueue(slot);
        }

        _popUpPanel = manager.GetPanel<PopUpPanel>();
        
        _closeBtn.onClick.AddListener(ClosePanel);
        
        _titleTMP.text = App.GetData<TitleData>().GetString("STR_SELECTBUILDING_NAME");
        _descriptionTMP.text = App.GetData<TitleData>().GetString("STR_SELECTBUILDING_DESC");
    }

    public void OpenPanel(Minimo minimo)
    {
        OpenPanel();
        
        _currentMinimo = minimo;
        foreach (var slot in _activeSlots)
        {
            slot.gameObject.SetActive(true);
        }
    }
    
    private void AssignSlot(ProduceObject building)
    {
        if (building is not ProduceAdvanced advanced) return;
        
        MinimoAssignedSlot slot;
        
        if (_slotPool.Count > 0)
        {
            slot = _slotPool.Dequeue();
        }
        else
        {
            slot = Instantiate(_slotPrefab, _content);
            slot.OnItemSelected += OnItemSelected;
        }
        
        slot.gameObject.SetActive(true);
        slot.Initialize(advanced);
        _activeSlots.Add(slot);
        
        var sorted = _activeSlots
            .OrderBy(s => s.Item.BuildingData.ID)
            .ToList();
        
        for (var i = 0; i < sorted.Count; i++)
        {
            sorted[i].transform.SetSiblingIndex(i);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
    }
    
    private void ReleaseSlot(ProduceObject building)
    {
        if (building is not ProduceAdvanced) return;
        
        var slot = _activeSlots.FirstOrDefault(x => x.Item == building);
        if (slot == null) return;
        slot.gameObject.SetActive(false);
        _activeSlots.Remove(slot);
        _slotPool.Enqueue(slot);
    }
    
    private void OnItemSelected(InventorySlot<ProduceAdvanced> slot)
    {
        var type = slot.Item.AssignedMinimo switch
        {
            null => PopUpType.MinimoAssign,
            var assigned when assigned == _currentMinimo => PopUpType.MinimoUnassign,
            _ => PopUpType.MinimoShift
        };

        _popUpPanel.OpenPanel(type, slot.Item, _currentMinimo);
    }
}
