using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlaceMinimoHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameTMP;
    [SerializeField] private TextMeshProUGUI _currentPlaceTMP;
    [SerializeField] private TextMeshProUGUI _stat1TMP;
    [SerializeField] private TextMeshProUGUI _stat2TMP;
    [SerializeField] private TextMeshProUGUI _stat3TMP;
    [SerializeField] private Button _placeBtn;
    
    private Minimo _minimo;
    private ProduceManager _produceManager;
    private PlaceMinimoPanel _minimoPanel;

    private void Awake()
    {
        var index = transform.GetSiblingIndex();
        var titleData = App.GetData<TitleData>();
        _produceManager = App.GetManager<ProduceManager>();
        _minimoPanel = App.GetManager<UIManager>().GetPanel<PlaceMinimoPanel>();

        _nameTMP.text = $"테스트모{index + 1}";
        _minimo = App.GetManager<MinimoManager>().Minimos[index];

        var stat1 = titleData.UMStat[_minimo.Data.StatType1];
        _stat1TMP.text = titleData.GetFormatString(stat1.Name, 0.ToString());
        
        var stat2 = titleData.UMStat[_minimo.Data.StatType2];
        _stat2TMP.text = titleData.GetFormatString(stat2.Name, 0.ToString());
        
        var stat3 = titleData.UMStat[_minimo.Data.StatType3];
        _stat3TMP.text = titleData.GetFormatString(stat3.Name, 0.ToString());
        
        _placeBtn.onClick.AddListener(() =>
        {
            var currentBuilding = (ProduceAdvanced)_produceManager.CurrentProduceObject;
            //currentBuilding.;
            _minimo.SetWorkState(currentBuilding);
            _minimoPanel.ClosePanel();
        });
        
    }

    public void UpdateCurrentPlaceInfo()
    {
        _currentPlaceTMP.text = _minimo.AssignedBuilding == null ? 
            string.Empty : _minimo.AssignedBuilding.BuildingData.Name;
    }
}
