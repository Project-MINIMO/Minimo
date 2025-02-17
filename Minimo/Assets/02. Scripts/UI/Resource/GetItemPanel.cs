using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum ResourceType
{
    Resource,
    SpecialResource
}

public class GetItemPanel : UIBase
{
    [SerializeField] private GameObject _itemBack;
    [SerializeField] private Image[] _iconImgs;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Vector2 _endPosition;
    
    private Vector2[] _startPosition = new Vector2[8];
    
    private ItemSO _itemSO;

    public bool IsComplete { get; private set; } = false;

    public override void Initialize()
    {
        _itemSO = App.GetData<TitleData>().ItemSO;
        _closeBtn.onClick.AddListener(ClosePanel);
        
        for (var i = 0; i < _iconImgs.Length; i++)
        {
            _startPosition[i] = _iconImgs[i].rectTransform.anchoredPosition;
        }
        
        var storagePanel = App.GetManager<UIManager>().GetPanel<StoragePanel>();
        if (storagePanel.GetActiveStorageBtnCount() > 6)
        {
            IsComplete = true;
        }
    }
    
    public override void OpenPanel()
    {
        base.OpenPanel();
        
        SetItemsNull();
    }

    public void OpenPanel(int index)
    {
        base.OpenPanel();

        SetItemsNull();
        var items = new List<Item>()
        {
            _itemSO.GetItem("Item_Wheat"),
            _itemSO.GetItem("Item_Corn"),
            _itemSO.GetItem("Item_Pumpkin"),
            _itemSO.GetItem("Item_Sugarcane"),
            _itemSO.GetItem("Item_Pepper"),
            _itemSO.GetItem("Item_Apple"),
            _itemSO.GetItem("Item_Blueberry"),
            _itemSO.GetItem("Item_Pineapple"),
        };
        
        foreach(var item in items)
        {
            App.GetManager<AccountInfoManager>().AddItemCount(item.Code, 5);
        }
        
        SetItems(items);
        StartCoroutine(ShowResources());
    }

    public void OpenPanel(List<Item> items)
    {
        OpenPanel();
        SetItems(items);
    }


    public void OpenPanel(Item item)
    {
        OpenPanel();
        SetItem(item);
    }

    public void OpenPanel(string code, int count)
    {
        var item = _itemSO.GetItem(code);
        OpenPanel(item);
    }
    
    private void SetItems(List<Item> items)
    {
        for (var i = 0; i < items.Count; i++)
        {
            SetItem(items[i], i);
        }
    }

    private void SetItem(Item item, int index = 0)
    {
        _iconImgs[index].gameObject.SetActive(true);
        _iconImgs[index].sprite = item.Icon;
    }

    private void SetItemsNull()
    {
        for (var i = 0; i < _iconImgs.Length; i++) 
        {
            _iconImgs[i].gameObject.SetActive(false);
            _iconImgs[i].rectTransform.anchoredPosition = _startPosition[i];
        }
    }
    
    private IEnumerator ShowResources()
    {
        foreach (var icon in _iconImgs)
        {
            icon.rectTransform.DOScale(1, 0.5f).SetEase(Ease.OutElastic);
            yield return new WaitForSeconds(0.1f);
        }
        
        yield return new WaitForSeconds(0.3f);

        for (var i = _iconImgs.Length - 1; i >= 0; i--) 
        {
            _iconImgs[i].rectTransform.DOAnchorPos(_endPosition, 0.5f).SetEase(Ease.InBack);
            yield return new WaitForSeconds(0.2f);
            _iconImgs[i].rectTransform.DOScale(0, 0.3f).SetEase(Ease.InCubic);
        }
        
        yield return new WaitForSeconds(0.3f);
        
        IsComplete = true;
        ClosePanel();   
    }
}
