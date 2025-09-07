using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionPanel : UIBase
{
    public override bool IsUseBlur => true;
    
    [Header("Buttons")]
    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _restartBtn;
    
    [SerializeField] private Toggle _soundOnTog;
    [SerializeField] private Toggle _soundOffTog;

    /* 
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
        */
    private SoundManager _soundManager;

    //#region Override
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        
        _soundManager = App.GetManager<SoundManager>();
        
        _soundOnTog.onValueChanged.AddListener((isOn) =>
        {
            if (isOn) _soundManager.ToggleMute(false);
        });
        _soundOffTog.onValueChanged.AddListener((isOn) =>
        {
            if (isOn) _soundManager.ToggleMute(true);
        });
        
        _openBtn.onClick.AddListener(OpenPanel);
        _closeBtn.onClick.AddListener(ClosePanel);
        _restartBtn.onClick.AddListener(Restart);

        /*
        _optionBases = GetComponentsInChildren<OptionBase>(true);

        SetString();
        SetButtonEvent();
        */
    }

    /*
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
    */

    private async void Restart()
    {
        App.LoadScene(SceneName.Empty);
    }
}
