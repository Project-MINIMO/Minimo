using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingPanel : UIBase
{
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private Button[] _buildingBtns;
    [SerializeField] private GameObject[] _buildingBacks;
    [SerializeField] private Sprite[] _btnSprites;

    [Header("Buttons")]
    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn;

    public override void Initialize()
    {
        SetString();
        SetButtonEvent();
    }

    public override void OpenPanel()
    {
        base.OpenPanel();

        OnClickBuildingBtn(0);
    }

    private void SetString()
    {
        var titleData = App.GetData<TitleData>();

        _titleTMP.text = titleData.GetString("STR_BUILDING_UI_TITLE");

        _buildingBtns[0].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_BUILDING_UI_PRODUCTION");
        _buildingBtns[1].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_BUILDING_UI_DECORATION");
        _buildingBtns[2].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_BUILDING_UI_UTILITY");
        _buildingBtns[3].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_BUILDING_UI_MINIMO");
    }

    private void SetButtonEvent()
    {
        _openBtn.onClick.AddListener(OpenPanel);
        _closeBtn.onClick.AddListener(ClosePanel);

        for (int i = 0; i < _buildingBtns.Length; i++)
        {
            int idx = i;

            _buildingBtns[idx].onClick.AddListener(() => OnClickBuildingBtn(idx));

            _buildingBacks[idx].SetActive(true);
            _buildingBacks[idx].SetActive(false);
        }
    }

    private void OnClickBuildingBtn(int index)
    {
        for (int i = 0; i < _buildingBtns.Length; i++)
        {
            if (index == i)
            {
                _buildingBtns[i].image.sprite = _btnSprites[0];
                _buildingBacks[i].SetActive(true);
            }
            else
            {
                _buildingBtns[i].image.sprite = _btnSprites[1];
                _buildingBacks[i].SetActive(false);
            }
        }
    }
}
