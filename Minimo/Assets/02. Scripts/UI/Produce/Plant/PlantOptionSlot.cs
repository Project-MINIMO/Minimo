using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlantOptionSlot : MonoBehaviour
{
    [SerializeField] private Image _itemImg;
    [SerializeField] private TextMeshProUGUI _amountTMP;
    
    private TitleData _titleData;
    [SerializeField] private PlantHandler _plantHandler;

    private void Awake()
    {
        _titleData = App.GetData<TitleData>();
        _plantHandler = GetComponentInChildren<PlantHandler>(true);
    }

    public void SetOption(ProduceData optionData)
    {
        _plantHandler.SetOption(optionData);
        
        SetResultInfo(optionData.ResultItems[0]);
    }

    private void SetResultInfo(ProduceResult result)
    {
        if (!_titleData.Item.TryGetValue(result.ID, out var itemData))
        {
            Debug.LogError($"Cannot find item data with code : {result.ID}");
            return;
        }
        
        _itemImg.sprite = Resources.Load<Sprite>($"Item/{itemData.Name}");
        _amountTMP.text = $"X{result.Amount}";
    }
}

