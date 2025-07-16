using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StorageBtn : MonoBehaviour
{
    public bool CanShow => Item != null && Item.Count > 0;
    public Item Item { get; private set; }
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

    public void Initialize(Item item)
    {
        Item = item;
        Position = GetComponent<RectTransform>().position;

        _iconImg.sprite = item.Icon;
    }
    
    private void OnClickInfoBtn()
    {
        _infoPanel.OpenPanel(this);
    }

    public void SetCount()
    {
        if (Item == null) return;
        
        gameObject.SetActive(CanShow);
        _countTMP.text = Item.Count.ToString();
    }
}
