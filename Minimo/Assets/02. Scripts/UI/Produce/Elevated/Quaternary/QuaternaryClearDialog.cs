using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuaternaryClearDialog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _descriptionTMP;
    [SerializeField] private Button _confirmBtn;
    [SerializeField] private Button _cancelBtn;

    private Action _confirmAction;
    
    private void Awake()
    {
        var titleData = App.GetData<TitleData>();
        
        _titleTMP.SetText(titleData.GetString("STR_POPUP_CLEARSLOT_NAME"));
        _descriptionTMP.SetText(titleData.GetString("STR_POPUP_CLEARSLOT_DESC1"));

        _confirmBtn.onClick.AddListener(() =>
        {
            _confirmAction?.Invoke();
            gameObject.SetActive(false);
        });
        _cancelBtn.onClick.AddListener(() => gameObject.SetActive(false));
    }

    public void Show(Action confirmCallback)
    {
        gameObject.SetActive(true);
        _confirmAction = confirmCallback;
    }
}
