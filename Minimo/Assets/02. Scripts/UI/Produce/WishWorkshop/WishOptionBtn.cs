using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WishOptionBtn : MonoBehaviour
{
    private bool _canShow => _item is { Count: > 0 };
    private Item _item; 
    
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

    public void Initialize(int id)
    {
        var item = AccountInfo.Instance.Items[id];
        _item = item;
        _iconImg.sprite = item.Icon;
    }
    
    private void OnClickChoiceBtn()
    {
        _wishPanel.SetItemOnSlot(_item);
    }

    private void SetCount()
    {
        if (_item == null) return;
        
        gameObject.SetActive(_canShow);
        _countTMP.text = _item.Count.ToString();
    }
}
