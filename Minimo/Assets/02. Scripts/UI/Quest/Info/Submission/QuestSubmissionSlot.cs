using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestSubmissionSlot : MonoBehaviour
{
    [SerializeField] private Button _guideBtn;
    [SerializeField] private Image _itemImg;
    [SerializeField] private TextMeshProUGUI _amountTMP;

    private UIManager _uiManager;
    private BuildingPanel _buildingPanel;

    private ClearType _currentClearType;
    private IQuestClearTarget _currentTarget;
    
    private void Awake()
    {
        _uiManager = App.GetManager<UIManager>();
        _buildingPanel = _uiManager.GetPanel<BuildingPanel>();
        
        _guideBtn.onClick.AddListener(Guide);
    }

    public void Initialize(ClearType type, IQuestClearTarget target, int currentAmount, int totalAmount)
    {
        _currentClearType = type;
        _currentTarget = target;
        _itemImg.sprite = target.Icon;
        _amountTMP.text = $"{currentAmount} / {totalAmount}";
        _guideBtn.gameObject.SetActive(currentAmount < totalAmount);
    }
    
    public void Initialize(Sprite icon)
    {
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
        if (_currentClearType == ClearType.Build)
        {
            _buildingPanel.OpenPanel();
        }
    }
}
