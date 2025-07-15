using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestItemPickSlot : MonoBehaviour
{
    [SerializeField] private Button _pickBtn;
    
    [SerializeField] private Image _iconImg;
    [SerializeField] private TextMeshProUGUI _nameTMP;
    [SerializeField] private TextMeshProUGUI _countTMP;
    
    private Item _item;
    private QuestSubmissionPanel _submissionPanel;
    
    private void Awake()
    {
        _submissionPanel = App.GetManager<UIManager>().GetPanel<QuestSubmissionPanel>();
        
        _pickBtn.onClick.AddListener(OnClickPick);
    }

    public void Initialize(Item item)
    {
        _item = item;

        _nameTMP.text = App.GetData<TitleData>().GetString($"STR_ITEM_{item.Name.ToUpper()}_NAME");
        _iconImg.sprite = Resources.Load<Sprite>($"Item/{item.Name}");
        
        SetCount();
    }

    public void SetCount()
    {
        if (_item == null) return;

        _countTMP.text = _item.Count.ToString();
        gameObject.SetActive(_item.Count > 0);
    }

    private void OnClickPick()
    {
        if (!_submissionPanel.CanSelectItem(_item)) return;
        
        AccountInfo.Instance.RemoveItem(_item, 1);
        SetCount();
    }
}