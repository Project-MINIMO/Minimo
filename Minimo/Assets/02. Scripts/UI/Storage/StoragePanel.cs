using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoragePanel : UIBase
{
    private enum StorageType
    {
        Entire,
        Food,
        Flower,
        Amulet
    }
    
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private Button[] _menuBtns;
    [SerializeField] private Sprite[] _btnSprites;
    
    [SerializeField] private Transform _buttonParent;
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private ScrollRect _scrollRect;

    [Header("Buttons")]
    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn;
    
    private List<StorageBtn> _storageBtns;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        SetString();
        SetButtonEvent();
        
        InitStorageBtns();
    }
    
    private void InitStorageBtns()
    {
        var existingButtons = GetComponentsInChildren<StorageBtn>(true);

        var items = AccountInfo.Instance.Items;
        _storageBtns = new List<StorageBtn>(items.Count);

        var i = 0;
        
        for (; i < items.Count; i++)
        {
            var storageBtn = i < existingButtons.Length 
                ? existingButtons[i] 
                : Instantiate(_buttonPrefab, _buttonParent).GetComponent<StorageBtn>();

            storageBtn.Initialize(items[i]);
            _storageBtns.Add(storageBtn);
        }

        for (; i < existingButtons.Length; i++)
        {
            existingButtons[i].gameObject.SetActive(false);
        }
    }

    public override void OpenPanel()
    {
        base.OpenPanel();

        OnClickStorageBtn(0);
    }

    private void SetString()
    {
        var titleData = App.GetData<TitleData>();

        _titleTMP.text = titleData.GetString("STR_STORAGE_UI_TITLE");

        _menuBtns[0].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_STORAGE_UI_ENTIRE");
        _menuBtns[1].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_STORAGE_UI_RESOURCE");
        _menuBtns[2].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_STORAGE_UI_PRODUCT");
        _menuBtns[3].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_STORAGE_UI_CONSTRUCTION");
    }

    private void SetButtonEvent()
    {
        _openBtn.onClick.AddListener(OpenPanel);
        _closeBtn.onClick.AddListener(ClosePanel);

        for (var i = 0; i < _menuBtns.Length; i++)
        {
            var idx = i;

            _menuBtns[idx].onClick.AddListener(() => OnClickStorageBtn(idx));
        }
    }

    private void OnClickStorageBtn(int index)
    {
        for (var i = 0; i < _menuBtns.Length; i++)
        {
            if (index == i)
            {
                _menuBtns[i].image.sprite = _btnSprites[0];
                FilterStorageBtns(i);
            }
            else
            {
                _menuBtns[i].image.sprite = _btnSprites[1];
            }
        }
    }
    
    private void FilterStorageBtns(int index)
    {
        var targetType = (StorageType)index;
        
        foreach (var button in _storageBtns)
        {
            var isActive = 
                targetType == StorageType.Entire 
                || index == button.Item.Data.Type;
            
            button.gameObject.SetActive(isActive);
        }
        
        _scrollRect.verticalNormalizedPosition = 1;
    }

    public void Refresh()
    {
        foreach (var button in _storageBtns)
        {
            button.SetCount();
        }
    }
}
