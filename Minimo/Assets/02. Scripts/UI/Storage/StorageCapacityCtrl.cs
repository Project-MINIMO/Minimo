using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StorageCapacityCtrl : MonoBehaviour
{
    public int Capacity { get; private set; } = 0;
    
    [SerializeField] private TextMeshProUGUI _capacityTMP;
    [SerializeField] private Button _expandBtn;
    [SerializeField] private Transform _storageParent;

    private StorageExpandPanel _expandPanel;

    private void Start()
    {
        _expandBtn.GetComponentInChildren<TextMeshProUGUI>().text = App.GetData<TitleData>().GetString("STR_STORAGE_UI_EXPAND");
        _expandBtn.onClick.AddListener(() => _expandPanel.OpenPanel());

        _expandPanel = App.GetManager<UIManager>().GetPanel<StorageExpandPanel>();

        SetCapacity();
    }
    
    private void SetCapacity()
    {
        var storageBtns = _storageParent.GetComponentsInChildren<InventorySlot>();
        Capacity = storageBtns.Sum(storageBtn => storageBtn.CanShow ? 1 : 0);

        _capacityTMP.text = $"{Capacity}/{100}"; //TODO : 최대 창고 용량
    }
    
    public void UpdateMaxCapacity()
    {
        _capacityTMP.text = $"{Capacity}/{100}"; //TODO : 최대 창고 용량
    }
}
