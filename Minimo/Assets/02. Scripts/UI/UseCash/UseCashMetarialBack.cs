using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UseCashMaterialBack : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _descriptionTMP;
    [SerializeField] private TextMeshProUGUI _priceTMP;
    [SerializeField] private Button _confirmBtn;
    [SerializeField] private Button _cancelBtn;
    
    private ItemInfoUpdater[] _itemInfos;
    
    private Action _confirmAction;
    private Action _cancelAction;
    private Action _closeAction;
    private Action _lackAction;
    private int _price;

    public void Initialize(Action closeAction, Action lackAction)
    {
        _closeAction = closeAction;
        _lackAction = lackAction;
        
        _itemInfos = GetComponentsInChildren<ItemInfoUpdater>();

        var titleData = App.GetData<TitleData>();

        _titleTMP.text = titleData.GetString("STR_SPENDCASH_TITLE");
        _descriptionTMP.text = titleData.GetString("STR_SPENDCASH_DESC_PRODUCE");
        _confirmBtn.onClick.AddListener(Confirm);
        _cancelBtn.onClick.AddListener(() =>
        {
            _cancelAction?.Invoke();
            _closeAction?.Invoke();
        });
    }

    public void Setup(List<(Item, int)> lackItems, Action onConfirm, Action onCancel)
    {
        _price = CalculatePrice(lackItems);
        _priceTMP.text = _price.ToString();
        _confirmAction = onConfirm;
        _cancelAction = onCancel;

        SetMaterialSlots(lackItems);
    }
    
    private int CalculatePrice(List<(Item, int)> lackItems)
    {
        return lackItems.Sum(lackItem => lackItem.Item1.BuyCost * lackItem.Item2);
    }

    private void SetMaterialSlots(List<(Item, int)> lackItems)
    {
        var i = 0;
        
        for (; i < lackItems.Count; i++) 
        {
            _itemInfos[i].gameObject.SetActive(true);
            _itemInfos[i].UpdateItem(lackItems[i].Item1, lackItems[i].Item2);
        }

        for (; i < _itemInfos.Length; i++) 
        {
            _itemInfos[i].gameObject.SetActive(false);
        }
    }
    
    private void Confirm()
    {
        if (AccountInfo.Instance.Cash < _price)
        {
            _lackAction?.Invoke();
        }
        else
        {
            AccountInfo.Instance.Cash -= _price;
            _confirmAction?.Invoke();
            _confirmAction = null;
            _price = 0;

            _closeAction.Invoke();
        }
    }
}
