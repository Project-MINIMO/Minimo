using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionPanel : UIBase
{
    protected override GameObject Panel => _optionBack;
    
    [Header("Buttons")]
    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn;

    [Header("Options")]
    [SerializeField] private GameObject _optionBack;
    [SerializeField] private Button[] _optionBtns;
    [SerializeField] private GameObject[] _optionBacks;
    [SerializeField] private Sprite[] _btnSprites;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _gameSettingTMP;
    [SerializeField] private TextMeshProUGUI _accountSettingTMP;
    [SerializeField] private TextMeshProUGUI _serviceCenterTMP;

    private OptionBase[] _optionBases;

    #region Override
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _optionBases = GetComponentsInChildren<OptionBase>(true);

        SetString();
        SetButtonEvent();
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
        
        OnClickOptionBtn(0);
    }

    public override void ClosePanel()
    {
        SaveOptionData();

        base.ClosePanel();
    }
    #endregion

    private void SetString()
    {
        var titleData = App.GetData<TitleData>();

        _gameSettingTMP.text = titleData.GetString("STR_OPTION_GAMESETTING");
        _accountSettingTMP.text = titleData.GetString("STR_OPTION_ACCOUNTSETTING");
        _serviceCenterTMP.text = titleData.GetString("STR_OPTION_SERVICECENTER");
    }

    private void SetButtonEvent()
    {
        _openBtn.onClick.AddListener(OpenPanel);
        _closeBtn.onClick.AddListener(ClosePanel);

        for (int i = 0; i < _optionBtns.Length; i++)
        {
            int idx = i;

            _optionBtns[idx].onClick.AddListener(() => OnClickOptionBtn(idx));

            _optionBacks[idx].SetActive(true);
            _optionBacks[idx].SetActive(false);
        }
    }

    private void OnClickOptionBtn(int index)
    {
        for (int i = 0; i < _optionBtns.Length; i++)
        {
            if (index == i)
            {
                _optionBtns[i].image.sprite = _btnSprites[0];
                _optionBacks[i].SetActive(true);
            }
            else
            {
                _optionBtns[i].image.sprite = _btnSprites[1];
                _optionBacks[i].SetActive(false);
            }
        }
    }

    private void SaveOptionData()
    {
        for (int i = 0; i < _optionBases.Length; i++)
        {
            _optionBases[i].SaveOption();
        }

        App.GetData<SettingData>().SaveToLocal();
    }
}
