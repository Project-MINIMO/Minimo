using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProduceStarBtn : MonoBehaviour
{
    [SerializeField] private Button _starBtn;
    [SerializeField] private TextMeshProUGUI _starText;

    private ProduceManager _produceManager;
    private UseCashPanel _useCashPanel;
    
    private int _starValue;
    private int _currentStarCount;
    
    private void Start()
    {
        _useCashPanel = App.GetManager<UIManager>().GetPanel<UseCashPanel>();

        _starValue = 1;//App.GetData<TitleData>().Common["BlueStarValue"];

        _produceManager = App.GetManager<ProduceManager>();

        _starBtn.onClick.AddListener(OnClickStarBtn);
    }

    private void OnClickStarBtn()
    {
        _useCashPanel.OpenPanel(UseCashType.Produce, 
            _currentStarCount, 
            _produceManager.HarvestEarly);
    }

    private void SetStarText(float remainTime)
    {
        _currentStarCount = (int)(remainTime / _starValue) + 1;
        _starText.text = _currentStarCount.ToString();
    }
}
