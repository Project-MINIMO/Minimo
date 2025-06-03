using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StorageBtn : MonoBehaviour
{
    public bool CanShow => Item != null && AccountInfo.Instance.items.ContainsKey(Item) && AccountInfo.Instance.items[Item] > 0;
    public ItemData Item { get; private set; }
    public Vector2 Position { get; private set; }
    public int SibilingsIndex => transform.GetSiblingIndex() % 4;
    
    [SerializeField] private Button _infoBtn;
    
    [SerializeField] private Image _iconImg;
    [SerializeField] private TextMeshProUGUI _countTMP;
    
    private StorageInfoPanel _infoPanel;
    
    private void Awake()
    {
        _infoPanel= App.GetManager<UIManager>().GetPanel<StorageInfoPanel>();
        
        _infoBtn.onClick.AddListener(OnClickInfoBtn);
    }

    private void OnEnable()
    {
        SetCount();
    }

    public void Initialize(ItemData item)
    {
        Item = item;
        Position = GetComponent<RectTransform>().position;

        _iconImg.sprite = Resources.Load<Sprite>($"Item/{item.Name}");
    }
    
    private void OnClickInfoBtn()
    {
        _infoPanel.OpenPanel(this);
    }

    private void SetCount()
    {
        if (Item == null) return;
        
        gameObject.SetActive(CanShow);

        if (AccountInfo.Instance.items.TryGetValue(Item, out var value))
        {
            _countTMP.text = value.ToString();
        }
    }
}
