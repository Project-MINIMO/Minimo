using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChargeCashBack : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _descriptionTMP;
    [SerializeField] private Button _confirmBtn;
    [SerializeField] private Button _cancelBtn;
    
    private Action _closeAction;

    public void Initialize(Action closeAction)
    {
        var titleData = App.GetData<TitleData>();
        _closeAction = closeAction;

        _titleTMP.text = titleData.GetString("STR_CHARGECASH_NAME");
        _descriptionTMP.text = titleData.GetString("STR_CHARGECASH_DESC");

        _confirmBtn.GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_BUTTON_YES");
        _cancelBtn.GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_BUTTON_NO");

        _confirmBtn.onClick.AddListener(OnClickChargeYes);
        _cancelBtn.onClick.AddListener(() => _closeAction?.Invoke());
    }

    private void OnClickChargeYes()
    {
        AccountInfo.Instance.blueStar += 100;
        _closeAction.Invoke();
    }
}
