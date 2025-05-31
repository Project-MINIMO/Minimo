using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionDelete : OptionBase
{
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private Button _deleteBtn;
    
    [Header("Pop Up")]
    [SerializeField] private GameObject _popUpBack;
    [SerializeField] private TextMeshProUGUI _popuUpTitleTMP;
    [SerializeField] private TextMeshProUGUI _popuUpContentTMP;
    [SerializeField] private Button _yesBtn;
    [SerializeField] private Button _noBtn;

    protected override void Start()
    {
        base.Start();
        
        _popUpBack.SetActive(false);
    }
    
    protected override void SetString()
    {
        var titleData = App.GetData<TitleData>();

        _titleTMP.text = titleData.GetString("STR_OPTION_ACCOUNTSETTING_DELETE");
        _deleteBtn.GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_OPTION_ACCOUNTSETTING_DELETE_DESC");
        
        _popuUpTitleTMP.text = titleData.GetString("STR_OPTION_ACCOUNTSETTING_DELETE_POPUP");
        _popuUpContentTMP.text = titleData.GetString("STR_OPTION_ACCOUNTSETTING_DELETE_POPUP_DESC");

        _yesBtn.GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_BUTTON_YES");
        _noBtn.GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_BUTTON_NO");
    }
    
    protected override void SetEvent()
    {
        _deleteBtn.onClick.AddListener(OnClickDelete);
        _yesBtn.onClick.AddListener(OnClickYes);
        _noBtn.onClick.AddListener(() => _popUpBack.SetActive(false));
    }

    protected override void SetValueFromData() { }

    public override void SaveOption() { }

    private void OnClickDelete()
    {
        _popUpBack.SetActive(true);
    }
    
    private void OnClickYes()
    {
        // TODO : Delete Account
        App.LoadScene(SceneName.Title);
    }
}
