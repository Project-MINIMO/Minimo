using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChargeCashBack : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _descriptionTMP;
    [SerializeField] private Button _yesBtn;
    [SerializeField] private Button _noBtn;
    
    private TitleData _titleData;
    private Action _closeAction;

    public void Initialize(TitleData titleData, Action closeAction)
    {
        _titleData = titleData;
        _closeAction = closeAction;

        _titleTMP.text = _titleData.GetString("STR_CHARGECASH_TITLE");
        _descriptionTMP.text = _titleData.GetString("STR_CHARGECASH_DESC");

        _yesBtn.GetComponentInChildren<TextMeshProUGUI>().text = _titleData.GetString("STR_BUTTON_YES");
        _noBtn.GetComponentInChildren<TextMeshProUGUI>().text = _titleData.GetString("STR_BUTTON_NO");

        _yesBtn.onClick.AddListener(OnClickChargeYes);
        _noBtn.onClick.AddListener(() => _closeAction?.Invoke());
    }

    private void OnClickChargeYes()
    {
        //TODO : Charge Panel Open
        _closeAction.Invoke();
    }
}
