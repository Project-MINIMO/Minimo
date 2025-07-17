using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class ElevatedPanel : UIBase
{
    [SerializeField] private Button _closeBtn;
    [SerializeField] private TextMeshProUGUI _titleTMP;

    [SerializeField] private ItemInfoUpdater _resultInfo;
    [SerializeField] private GameObject _expandHandler;
    [SerializeField] private Button _placeMinimoBtn;
    
    protected ProduceManager _produceManager;
    protected ProduceElevated _produceObject;
    private PlaceMinimoPanel _placeMinimoPanel;
    
    private GameObject[] _slots;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _produceManager = App.GetManager<ProduceManager>();
        _placeMinimoPanel = manager.GetPanel<PlaceMinimoPanel>();
        
        var produceSlots = GetComponentsInChildren<ProduceSlot>(true);
        _slots = produceSlots.Select(slot => slot.gameObject).ToArray();
        
        _closeBtn.onClick.AddListener(_produceManager.Deselect);
        _placeMinimoBtn.onClick.AddListener(_placeMinimoPanel.OpenPanel);
    }

    public override void OpenPanel()
    {
        base.OpenPanel();

        _titleTMP.SetText(_produceManager.CurrentObject.BuildingData.Name);
        
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
            _resultInfo.UpdateItem(_produceObject.AllTasks[0].Result.ID);
        }
        else
        {
            _resultInfo.gameObject.SetActive(false);
        }
    }
}
