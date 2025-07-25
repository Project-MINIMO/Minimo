using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class ElevatedPanel : UIBase
{
    public override bool IsUseBlur => true;
    
    [SerializeField] private Button _closeBtn;
    [SerializeField] private TextMeshProUGUI _titleTMP;

    [SerializeField] private ItemInfoUpdater _resultInfo;
    [SerializeField] private GameObject _expandHandler;
    [SerializeField] private Button _placeMinimoBtn;
    
    [SerializeField] private UILongPressDetector _longPressDetector;
    [SerializeField] private GameObject _minimoInfoObj;
    [SerializeField] private TextMeshProUGUI _minimoInfoTMP;
    
    protected ProduceManager _produceManager;
    protected ProduceElevated _produceObject;
    private MinimoPlacePanel _placePanel;
    
    private GameObject[] _slots;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _produceManager = App.GetManager<ProduceManager>();
        _placePanel = manager.GetPanel<MinimoPlacePanel>();
        
        var produceSlots = GetComponentsInChildren<ProduceSlot>(true);
        _slots = produceSlots.Select(slot => slot.gameObject).ToArray();

        _longPressDetector.OnLongPress += () =>
        {
            if (_produceObject.AssignedMinimo == null) return;
            _minimoInfoTMP.SetText(GetMinimoInfo());
            _minimoInfoObj.SetActive(true);
        };
        _longPressDetector.OnClickUp += () => _minimoInfoObj.SetActive(false);
        
        _closeBtn.onClick.AddListener(_produceManager.Deselect);
        _placeMinimoBtn.onClick.AddListener(() => 
            _placePanel.OpenPanel(_produceManager.CurrentObject as ProduceAdvanced));
    }

    public override void OpenPanel()
    {
        base.OpenPanel();

        _titleTMP.SetText(_produceManager.CurrentObject.BuildingData.Name);
        _minimoInfoObj.SetActive(false);
        
        _produceObject = _produceManager.CurrentObject as ProduceElevated;
        if (_produceObject == null) return;
        
        _produceObject.OnMaxSlotCountChanged += UpdateSlots;
        _produceObject.OnProduceStateChanged += UpdateResultInfo;
        UpdateSlots();
        UpdateResultInfo(_produceObject.CurrentState);
    }

    public override void ClosePanel()
    {
        if (_produceObject != null)
        {
            _produceObject.OnMaxSlotCountChanged -= UpdateSlots;
            _produceObject.OnProduceStateChanged -= UpdateResultInfo;
            _produceObject = null;
        }
        
        base.ClosePanel();
    }

    private void UpdateSlots()
    {
        var maxCount = _produceObject.MaxSlotCount;
        var i = 0;

        for (; i < maxCount; i++)
        {
            _slots[i].SetActive(true);
        }

        for (; i < _slots.Length; i++)
        {
            _slots[i].SetActive(false);
        }
        
        _expandHandler.SetActive(maxCount < 5);
    }

    private void UpdateResultInfo(ProduceState state)
    {
        if (state == ProduceState.Complete)
        {
            _resultInfo.gameObject.SetActive(true);
            _resultInfo.UpdateItem(_produceObject.AllTasks[0].Data.ResultItems[0].ID);
        }
        else
        {
            _resultInfo.gameObject.SetActive(false);
        }
    }

    private string GetMinimoInfo()
    {
        var info = string.Empty;
        var minimo = _produceObject.AssignedMinimo;
        for (var i = 0; i < minimo.Abilities.Count; i++)
        {
            if (i > 0) info += "\n";
            info += string.Format(minimo.AbilityDescriptions[i], minimo.Abilities[i].Value);
        }

        return info;
    }
}
