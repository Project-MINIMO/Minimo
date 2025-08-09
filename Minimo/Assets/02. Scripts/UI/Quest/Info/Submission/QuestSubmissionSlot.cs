using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestSubmissionSlot : MonoBehaviour
{
    [SerializeField] private Button _guideBtn;
    [SerializeField] private Image _itemImg;
    [SerializeField] private TextMeshProUGUI _amountTMP;

    private UIManager _uiManager;
    private EditManager _editManager;
    private ProduceManager _produceManager;
    private BuildingPanel _buildingPanel;
    
    private Item _currentTarget;
    
    private TitleData _titleData;
    
    private Dictionary<int, ProduceData> _resultIdToProduce;
    
    private void Awake()
    {
        _uiManager = App.GetManager<UIManager>();
        _editManager =  App.GetManager<EditManager>();
        _produceManager = App.GetManager<ProduceManager>();
        _buildingPanel = _uiManager.GetPanel<BuildingPanel>();
        
        _titleData = App.GetData<TitleData>();
        
        _guideBtn.onClick.AddListener(Guide);
        
        _resultIdToProduce = new Dictionary<int, ProduceData>(_titleData.Item.Count);
        foreach (var data in _titleData.Produce.Values)
        {
            foreach (var result in data.ResultItems)
            {
                _resultIdToProduce[result.ID] = data;
            }
                
        }
    }

    public void Initialize(IQuestClearTarget target, int currentAmount, int totalAmount)
    {
        _currentTarget = target as Item;
        _itemImg.sprite = target.Icon;
        _amountTMP.text = $"{currentAmount} / {totalAmount}";
        _guideBtn.gameObject.SetActive(currentAmount < totalAmount);
    }
    
    public void Initialize(Sprite icon)
    {
        _itemImg.gameObject.SetActive(true);
        _itemImg.sprite = icon;
        _guideBtn.gameObject.SetActive(false);
        _amountTMP.gameObject.SetActive(false);
    }

    public void ClearItem()
    {
        _itemImg.gameObject.SetActive(false);
        _guideBtn.gameObject.SetActive(false);
        _amountTMP.gameObject.SetActive(false);
    }

    private void Guide()
    {
        _uiManager.PopAllPanels();

        if (!_resultIdToProduce.TryGetValue(_currentTarget.ID, out var produce))
        {
            return;
        }

        var candidates = _editManager.ActiveProduces
            .Where(building => building.BuildingData.Code == produce.Building)
            .ToList();
        
        if (candidates.Count == 0)
        {
            _buildingPanel.OpenPanel();
            return;
        }
        
        var idle = candidates.FirstOrDefault(building => building.CurrentState == ProduceState.Idle); 
        var selected = idle ?? candidates.First();
        _produceManager.Select(selected);
    }
}
