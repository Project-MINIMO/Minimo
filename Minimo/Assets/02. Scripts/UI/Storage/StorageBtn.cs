using MinimoShared;

using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StorageBtn : MonoBehaviour
{
    public bool CanShow => AccountInfo.Instance.items.ContainsKey(Item);
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
        
        App.GetManager<UIManager>().GetPanel<StoragePanel>().StorageChanged.Subscribe(_ => SetCount()).AddTo(gameObject);;
    }

    private void OnEnable()
    {
        SetCount();
    }

    public void Initialize(ItemData item)
    {
        Item = item;
        Position = GetComponent<RectTransform>().position;

        _iconImg.sprite = null; //item.Icon;
    }
    
    private void OnClickInfoBtn()
    {
        _infoPanel.OpenPanel(this);
    }

    private void SetCount()
    {
        if (CanShow == false)
        {
            gameObject.SetActive(false);
            return;
        }

        if (AccountInfo.Instance.items.TryGetValue(Item, out var value))
        {
            _countTMP.text = value.ToString();
        }
        else
        {
            _countTMP.text = "0";
        }
    }
}
