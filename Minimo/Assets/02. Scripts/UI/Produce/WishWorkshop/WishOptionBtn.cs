using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WishOptionBtn : MonoBehaviour
{
    private bool _canShow => _item != null && AccountInfo.Instance.Items.ContainsKey(_item) && AccountInfo.Instance.Items[_item] > 0;
    private ItemData _item; 
    
    [SerializeField] private Button _choiceBtn;
    
    [SerializeField] private Image _iconImg;
    [SerializeField] private TextMeshProUGUI _countTMP;

    private WishPanel _wishPanel;
    
    private void Awake()
    {
        _choiceBtn.onClick.AddListener(OnClickChoiceBtn);
        _wishPanel = App.GetManager<UIManager>().GetPanel<WishPanel>();
    }

    private void OnEnable()
    {
        SetCount();
    }

    public void Initialize(ItemData item)
    {
        _item = item;
        
        _iconImg.sprite = Resources.Load<Sprite>($"Item/{item.Name}");
    }
    
    private void OnClickChoiceBtn()
    {
        _wishPanel.SetItemOnSlot(_item);
    }

    private void SetCount()
    {
        if (_item == null) return;
        
        gameObject.SetActive(_canShow);

        if (AccountInfo.Instance.Items.TryGetValue(_item, out var value))
        {
            _countTMP.text = value.ToString();
        }
    }
}
