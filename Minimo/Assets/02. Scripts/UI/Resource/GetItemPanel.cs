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
    
    public bool IsComplete { get; private set; } = false;

    public override void Initialize()
    {
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

    public void OpenPanel(List<ItemData> items)
    {
        OpenPanel();
        SetItems(items);
    }


    public void OpenPanel(ItemData item)
    {
        OpenPanel();
        SetItem(item);
    }
    
    private void SetItems(List<ItemData> items)
    {
        for (var i = 0; i < items.Count; i++)
        {
            SetItem(items[i], i);
        }
    }

    private void SetItem(ItemData item, int index = 0)
    {
        _iconImgs[index].gameObject.SetActive(true);
        _iconImgs[index].sprite = null;//item.Icon;
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
