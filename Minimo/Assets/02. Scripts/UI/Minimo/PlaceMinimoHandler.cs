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
    
    private string _stat1Code;
    private string _stat2Code;
    private string _stat3Code;

    private void Awake()
    {
        var index = transform.GetSiblingIndex();
        var titleData = App.GetData<TitleData>();
        _produceManager = App.GetManager<ProduceManager>();
        _minimoPanel = App.GetManager<UIManager>().GetPanel<PlaceMinimoPanel>();

        _nameTMP.text = $"테스트모{index + 1}";
        _minimo = App.GetManager<MinimoManager>().Minimos[index];

        _stat1Code = titleData.GetString(titleData.UMStat[_minimo.Data.StatType1].Name);
        _stat2Code = titleData.GetString(titleData.UMStat[_minimo.Data.StatType2].Name);
        _stat3Code = titleData.GetString(titleData.UMStat[_minimo.Data.StatType3].Name);
  
        _placeBtn.onClick.AddListener(() =>
        {
            var currentBuilding = (ProduceAdvanced)_produceManager.CurrentProduceObject;
            //currentBuilding.;
            _minimo.SetWorkState(currentBuilding);
            _minimoPanel.ClosePanel();
        });
    }

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(_stat3Code)) return;
        
        _stat1TMP.text = string.Format(_stat1Code, _minimo.AbilityValue1);
        _stat2TMP.text = string.Format(_stat2Code, _minimo.AbilityValue2);
        _stat3TMP.text = string.Format(_stat3Code, _minimo.AbilityValue3);

        _currentPlaceTMP.text = _minimo.AssignedBuilding == null ? 
            string.Empty : _minimo.AssignedBuilding.BuildingData.Name;
    }
}
