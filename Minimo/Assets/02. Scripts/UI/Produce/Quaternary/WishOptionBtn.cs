using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WishOptionBtn : MonoBehaviour
{
    private bool _canShow => Item is { Count: > 0 };
    public Item Item; 
    
    [SerializeField] private Button _choiceBtn;
    
    [SerializeField] private Image _iconImg;
    [SerializeField] private TextMeshProUGUI _countTMP;

    private QuaternaryPanel _wishPanel;
    
    private void Awake()
    {
        _choiceBtn.onClick.AddListener(OnClickChoiceBtn);
        _wishPanel = App.GetManager<UIManager>().GetPanel<QuaternaryPanel>();
    }

    private void OnEnable()
    {
        SetCount();
    }

    public void Initialize(int id)
    {
        var item = AccountInfo.Instance.Items[id];
        Item = item;
        _iconImg.sprite = item.Icon;
    }
    
    private void OnClickChoiceBtn()
    {
        _wishPanel.SetItemOnSlot(Item);
    }

    private void SetCount()
    {
        if (Item == null) return;
        
        gameObject.SetActive(_canShow);
        _countTMP.text = Item.Count.ToString();
    }
}
