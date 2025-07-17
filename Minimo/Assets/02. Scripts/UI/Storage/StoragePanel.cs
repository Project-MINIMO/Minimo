using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoragePanel : UIBase
{
    [SerializeField] private TextMeshProUGUI _titleTMP;
    
    [SerializeField] private StorageInfoCtrl _infoPanel;
    
    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _openBtn.onClick.AddListener(OpenPanel);
        _closeBtn.onClick.AddListener(ClosePanel);
        
        var slots = GetComponentsInChildren<InventorySlot>(true);
        foreach (var slot in slots)
        {
            slot.OnItemSelected += OnItemSelected;
        }

        _titleTMP.text = App.GetData<TitleData>().GetString("STR_STORAGE_UI_TITLE");
    }

    private void OnItemSelected(InventorySlot slot)
    {
        _infoPanel.Show(slot);
    }
}
