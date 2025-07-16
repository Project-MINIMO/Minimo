using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdvancedPanel : UIBase
{
    [SerializeField] private Button _closeBtn;
    [SerializeField] private TextMeshProUGUI _titleTMP;
    
    [SerializeField] private GameObject _expandHandler;
    [SerializeField] private Button _placeMinimoBtn;

    private ProduceManager _produceManager;
    private ProduceTertiary _produceObject;
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
        
        _titleTMP.text = App.GetData<TitleData>()
            .GetString($"STR_BUILDING_{_produceManager.CurrentObject.BuildingData.Name.ToUpper()}_NAME");
        
        _produceObject = _produceManager.CurrentObject as ProduceTertiary;
        if (_produceObject == null) return;
        
        _produceObject.OnMaxSlotCountChanged += UpdateSlots;
        UpdateSlots();
    }

    public override void ClosePanel()
    {
        if (_produceObject != null)
        {
            _produceObject.OnMaxSlotCountChanged -= UpdateSlots;
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
}
