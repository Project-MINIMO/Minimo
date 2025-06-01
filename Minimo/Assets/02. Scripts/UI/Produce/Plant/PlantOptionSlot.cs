using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlantOptionSlot : MonoBehaviour
{
    [Serializable]
    private struct PlantInfo
    {
        public GameObject _gameObject;
        public Image _image;
        public TextMeshProUGUI _text;
    }
    
    [SerializeField] private PlantInfo _result;
    [SerializeField] private TextMeshProUGUI _storageTMP;
    
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

        //_storageTMP.text = _accountInfo.GetItem(optionData.ResultItems[0].ID).Count.ToString();
    }

    private void SetResultInfo(ProduceResult result)
    {
        if (!_titleData.Item.TryGetValue(result.ID, out var itemData))
        {
            Debug.LogError($"Cannot find item data with code : {result.ID}");
            return;
        }
        
        _result._image.sprite = Resources.Load<Sprite>($"Item/{result.ID}");
        _result._text.text = $"X{result.Amount}";
    }
}

