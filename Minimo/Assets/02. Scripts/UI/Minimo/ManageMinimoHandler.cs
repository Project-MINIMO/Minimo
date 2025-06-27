using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManageMinimoHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameTMP;
    [SerializeField] private TextMeshProUGUI _levelTMP;
    [SerializeField] private TextMeshProUGUI _stat1TMP;
    [SerializeField] private TextMeshProUGUI _stat2TMP;
    [SerializeField] private TextMeshProUGUI _stat3TMP;
    [SerializeField] private Button _upBtn;
    [SerializeField] private Button _downBtn;
    
    private Minimo _minimo;

    private string _stat1Code;
    private string _stat2Code;
    private string _stat3Code;

    private void Awake()
    {
        var index = transform.GetSiblingIndex();
        var titleData = App.GetData<TitleData>();
     
        _nameTMP.text = $"테스트모{index + 1}";
        _minimo = App.GetManager<MinimoManager>().Minimos[index];

        _stat1Code = titleData.GetString(titleData.UMStat[_minimo.Data.StatType1].Name);
        _stat1TMP.text = string.Format(_stat1Code, _minimo.AbilityValue1);
        
        _stat2Code = titleData.GetString(titleData.UMStat[_minimo.Data.StatType2].Name);
        _stat2TMP.text = string.Format(_stat2Code, _minimo.AbilityValue2);
        
        _stat3Code = titleData.GetString(titleData.UMStat[_minimo.Data.StatType3].Name);
        _stat3TMP.text = string.Format(_stat3Code, _minimo.AbilityValue3);
        
        _upBtn.onClick.AddListener(() =>
        {
            _minimo.AddLevel(1);
            UpdateLevelInfo();
        });
        _downBtn.onClick.AddListener(() =>
        {
            _minimo.AddLevel(-1);
            UpdateLevelInfo();
        });

        _levelTMP.text = _minimo.Level.ToString();
    }

    private void UpdateLevelInfo()
    {
        _levelTMP.text = _minimo.Level.ToString();
        
        _stat1TMP.text = string.Format(_stat1Code, _minimo.AbilityValue1);
        _stat2TMP.text = string.Format(_stat2Code, _minimo.AbilityValue2);
        _stat3TMP.text = string.Format(_stat3Code, _minimo.AbilityValue3);
    }
}
