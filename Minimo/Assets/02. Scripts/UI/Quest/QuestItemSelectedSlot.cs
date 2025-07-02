using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestItemSelectedSlot : MonoBehaviour
{
    public bool CanSelected => _count < _maxCount;
    public ItemData Item { get; private set; }
    
    [SerializeField] private Button _choiceBtn;
    
    [SerializeField] private Image _iconImg;
    [SerializeField] private TextMeshProUGUI _countTMP;

    private int _count;
    private int _maxCount;
    
    private void Awake()
    {
        _choiceBtn.onClick.AddListener(OnClickChoice);
    }

    public void Initialize(ItemData item, int amount)
    {
        Item = item;
        
        _iconImg.gameObject.SetActive(true);
        _iconImg.sprite = Resources.Load<Sprite>($"Item/{item.Name}");
        
        _countTMP.gameObject.SetActive(true);
        _count = 0;
        _maxCount = amount;
        
        _choiceBtn.gameObject.SetActive(false);
    }
    
    public void Initialize(QuestCondition condition)
    {
        var isChoice = condition == QuestCondition.Choice;
        
        _iconImg.gameObject.SetActive(false);
        
        _countTMP.gameObject.SetActive(false);
        _count = isChoice ? 1 : 0;
        _maxCount = 1;

        _choiceBtn.gameObject.SetActive(isChoice);
    }

    public void AddItem(ItemData item)
    {
        if (Item == null)
        {
            Item = item;
            
            _iconImg.gameObject.SetActive(true);
            _iconImg.sprite = Resources.Load<Sprite>($"Item/{item.Name}");
        }

        _count++;
        _countTMP.text = $"{_count}/{_maxCount}";
    }

    private void OnClickChoice()
    {
        
    }
}
