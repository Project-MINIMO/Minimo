using System.Linq;
using System.Collections.Generic;

using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinimoAssignedPanel : UIBase
{
    public override bool IsUseBlur => true;
    
    [SerializeField] private MinimoAssignedSlot _slotPrefab;
    [SerializeField] private RectTransform _contentParent;   
    
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _descriptionTMP;
    [SerializeField] private RectTransform _content;
    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Toggle _autoAssignTog;
    
    private readonly Queue<MinimoAssignedSlot> _slotPool = new();
    private readonly List<MinimoAssignedSlot> _activeSlots = new();
    private PlaceByBuildingPanel _placePanel;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _placePanel = manager.GetPanel<PlaceByBuildingPanel>();

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

        _titleTMP.text = App.GetData<TitleData>().GetString("STR_MANAGEPORDBUILDING_NAME");
        _descriptionTMP.text = App.GetData<TitleData>().GetString("STR_MANAGEPORDBUILDING_DESC");
        
        _openBtn.onClick.AddListener(OpenPanel);
        _closeBtn.onClick.AddListener(ClosePanel);
        _autoAssignTog.onValueChanged.AddListener(isOn => AccountInfo.Instance.AutoAssign(isOn));
    }
    
    private void AssignSlot(ProduceAdvanced building)
    {
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
        slot.Initialize(building);
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

    private void OnItemSelected(InventorySlot<ProduceAdvanced> slot)
    {
        _placePanel.OpenPanel(slot.Item);
    }
}
