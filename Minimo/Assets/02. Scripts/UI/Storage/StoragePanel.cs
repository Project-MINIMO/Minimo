using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoragePanel : UIBase
{
    public ReactiveProperty<int> StorageChanged = new ReactiveProperty<int>(0);
    
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private Button[] _storageBtns;
    [SerializeField] private Sprite[] _btnSprites;
    [SerializeField] private StorageBack _storageBack;

    [Header("Buttons")]
    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn;

    public override void Initialize()
    {
        SetString();
        SetButtonEvent();
        
        _storageBack.InitStorageBtns();
    }

    public override void OpenPanel()
    {
        base.OpenPanel();

        OnClickStorageBtn(0);
        _storageBack.FilterStorageBtns(0);
    }

    private void SetString()
    {
        var titleData = App.GetData<TitleData>();

        _titleTMP.text = titleData.GetString("STR_STORAGE_UI_TITLE");

        _storageBtns[0].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_STORAGE_UI_ENTIRE");
        _storageBtns[1].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_STORAGE_UI_RESOURCE");
        _storageBtns[2].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_STORAGE_UI_PRODUCT");
        _storageBtns[3].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_STORAGE_UI_CONSTRUCTION");
    }

    private void SetButtonEvent()
    {
        _openBtn.onClick.AddListener(OpenPanel);
        _closeBtn.onClick.AddListener(ClosePanel);

        for (int i = 0; i < _storageBtns.Length; i++)
        {
            int idx = i;

            _storageBtns[idx].onClick.AddListener(() => OnClickStorageBtn(idx));
        }
    }

    private void OnClickStorageBtn(int index)
    {
        for (int i = 0; i < _storageBtns.Length; i++)
        {
            var idx = i;
            
            if (index == idx)
            {
                _storageBtns[idx].image.sprite = _btnSprites[0];
                _storageBack.FilterStorageBtns(idx);
            }
            else
            {
                _storageBtns[idx].image.sprite = _btnSprites[1];
            }
        }
    }
    
    public void OnStorageChanged()
    {
        StorageChanged.Value++;
    }
    
    public StorageBtn GetStorageBtn(string id)
    {
        return _storageBack._storageBtns.Find(x => x.Item.Code == id);
    }
    
    public int GetActiveStorageBtnCount()
    {
        return _storageBack._storageBtns.FindAll(x => x.CanShow).Count;
    }
}
