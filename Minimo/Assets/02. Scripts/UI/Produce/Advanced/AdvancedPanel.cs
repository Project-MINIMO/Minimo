using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdvancedPanel : UIBase
{
    [SerializeField] private Button _closeBtn;
    [SerializeField] private TextMeshProUGUI _titleTMP;
    
    [SerializeField] private Button _expandBtn;
    [SerializeField] private Button _placeMinimoBtn;

    private ProduceManager _produceManager;
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
        _expandBtn.onClick.AddListener(() =>
        {
            ((ProduceTertiary)_produceManager.CurrentObject).AddSlotCount();
            InitializeSlots();
        });
        _placeMinimoBtn.onClick.AddListener(_placeMinimoPanel.OpenPanel);
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
        
        _titleTMP.text = App.GetData<TitleData>()
            .GetString($"STR_BUILDING_{_produceManager.CurrentObject.BuildingData.Name.ToUpper()}_NAME");

        InitializeSlots();
    }

    private void InitializeSlots()
    {
        var maxCount = ((ProduceTertiary)_produceManager.CurrentObject).MaxSlotCount;
        var i = 0;

        for (; i < maxCount; i++)
        {
            _slots[i].SetActive(true);
        }

        for (; i < _slots.Length; i++)
        {
            _slots[i].SetActive(false);
        }
        
        _expandBtn.gameObject.SetActive(maxCount < 5);
    }
}
