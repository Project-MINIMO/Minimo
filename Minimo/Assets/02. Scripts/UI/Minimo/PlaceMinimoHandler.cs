using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlaceMinimoHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameTMP;
    [SerializeField] private TextMeshProUGUI _currentPlaceTMP;
    [SerializeField] private TextMeshProUGUI[] _statTMPs;
    [SerializeField] private Button _placeBtn;
    
    private MinimoObject _minimo;
    private ProduceManager _produceManager;
    private PlaceMinimoPanel _minimoPanel;

    private List<string> _statCodes = new(3);

    private void Awake()
    {
        var index = transform.GetSiblingIndex();
        var titleData = App.GetData<TitleData>();
        _produceManager = App.GetManager<ProduceManager>();
        _minimoPanel = App.GetManager<UIManager>().GetPanel<PlaceMinimoPanel>();

        _nameTMP.text = $"테스트모{index + 1}";
        _minimo = App.GetManager<MinimoManager>().Minimos[index];

        _statCodes = new List<string>
        {
            titleData.GetString(titleData.UMStat[_minimo.Data.StatType1].Name),
            titleData.GetString(titleData.UMStat[_minimo.Data.StatType2].Name),
            titleData.GetString(titleData.UMStat[_minimo.Data.StatType3].Name)
        };
  
        _placeBtn.onClick.AddListener(() =>
        {
            var currentBuilding = (ProduceAdvanced)_produceManager.CurrentObject;
            if (_minimo.AssignedBuilding == currentBuilding)
            {
                _minimo.SetChillState();
            }
            else
            {
                _minimo.SetWorkState(currentBuilding);
            }
            
            _minimoPanel.ClosePanel();
        });
    }

    private void OnEnable()
    {
        if (_statCodes == null) return;

        for (var i = 0; i < _statCodes.Count; i++)
        {
            _statTMPs[i].text = string.Format(_statCodes[i], _minimo.Abilities[i].Value);
            
            if (!_minimo.Abilities[i].IsUnlocked)
            {
                _statTMPs[i].text += " <color=red>해금안됨</color>";
            }
        }

        _currentPlaceTMP.text = _minimo.AssignedBuilding == null ? 
            string.Empty : _minimo.AssignedBuilding.BuildingData.Name;
    }
}
