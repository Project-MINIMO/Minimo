using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManageMinimoHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameTMP;
    [SerializeField] private TextMeshProUGUI _levelTMP;
    [SerializeField] private TextMeshProUGUI[] _statTMPs;
    [SerializeField] private Button _upBtn;
    [SerializeField] private Button _downBtn;
    
    private MinimoObject _minimo;

    private List<string> _statCodes = new(3);

    private void Awake()
    {
        var index = transform.GetSiblingIndex();
        var titleData = App.GetData<TitleData>();
     
        _nameTMP.text = $"테스트모{index + 1}";
        _minimo = App.GetManager<MinimoManager>().Minimos[index];
        
        _statCodes = new List<string>
        {
            titleData.GetString(titleData.UMStat[_minimo.Data.StatType1].Name),
            titleData.GetString(titleData.UMStat[_minimo.Data.StatType2].Name),
            titleData.GetString(titleData.UMStat[_minimo.Data.StatType3].Name)
        };

        UpdateLevelInfo();
        
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
    }

    private void UpdateLevelInfo()
    {
        _levelTMP.text = _minimo.Level.ToString();
        
        if (_statCodes == null) return;

        for (var i = 0; i < _statCodes.Count; i++)
        {
            _statTMPs[i].text = string.Format(_statCodes[i], _minimo.Abilities[i].Value);

            if (!_minimo.Abilities[i].IsUnlocked)
            {
                _statTMPs[i].text += " <color=red>해금안됨</color>";
            }
        }
    }
}
