using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class QuestSummaryInfo : MonoBehaviour
{
    public int QuestType => _questData.Type;

    [SerializeField] private Button _toggleBtn;
    [SerializeField] private Button _openBtn;
    
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _descTMP;
    [SerializeField] private RectTransform _rect;
    [SerializeField] private Image _detailBackgroundImg;
    [SerializeField] private Image _defaultBackgroundImg;
    [SerializeField] private GameObject[] _iconObjs;
    
    private TitleData _titleData;
    
    private QuestData _questData;
    private DetailQuestData _detailData;
    
    private QuestConsPanel _consPanel;
    private QuestSubmissionPanel _submissionPanel;

    private string[] _descriptionStrings;
    private const float ExpandedHeight = 153;
    private const float CollapsedHeight = 69;
    private readonly Vector2 _titleExpandedPos = new Vector2(36.2f, -47.4f);
    private readonly Vector2 _titleCollapsedPos = new Vector2(96f, -16.2f);
    
    private bool _isExpanded;

    private void Awake()
    {
        _titleData = App.GetData<TitleData>();
        _consPanel = App.GetManager<UIManager>().GetPanel<QuestConsPanel>();
        _submissionPanel = App.GetManager<UIManager>().GetPanel<QuestSubmissionPanel>();

        _descriptionStrings = new[]
        {
            _titleData.GetString("STR_QUEST_CLEAR_LEVEL"),
            _titleData.GetString("STR_QUEST_CLEAR_PREP"),
            _titleData.GetString("STR_QUEST_CLEAR_HARVEST"),
            _titleData.GetString("STR_QUEST_CLEAR_CRAFT"),
            _titleData.GetString("STR_QUEST_CLEAR_WISH"),
            _titleData.GetString("STR_QUEST_CLEAR_BUILD"),
        };
        
        _toggleBtn.onClick.AddListener(Toggle);
        
        _openBtn.onClick.AddListener(()=>
        {
            if (_questData.Type == 1)
            {
                _consPanel.OpenPanel(_detailData);
            }
            else
            {
                _submissionPanel.OpenPanel(_detailData);
            }
        });
    }

    public void Initialize(DetailQuestData data)
    {
        _questData = _titleData.Quest[data.ID / 10];
        _detailData = data;

        _iconObjs[_questData.Type].SetActive(true);
        
        _titleTMP.text = _titleData.GetString($"STR_QUEST_{_detailData.Name}");
        _descTMP.text = string.Empty;
        
        for (var i = 0; i < _detailData.Clear.Length; i++)
        {
            if (i >= 1) _descTMP.text += "\n";
            
            var clear = _detailData.Clear[i];
            
            switch (clear.Type)
            {
                case ClearType.Wish:
                    _descTMP.text += _descriptionStrings[(int)clear.Type];
                    break;
                
                case ClearType.UserLevel:
                    _descTMP.text += string.Format(_descriptionStrings[(int)clear.Type], clear.Amount);
                    break;
                
                case ClearType.Build:
                {
                    var target = _titleData.Building[clear.Target];
                    var name = target.Name;
                    _descTMP.text += string.Format(_descriptionStrings[(int)clear.Type], _titleData.GetString(name), clear.Amount);
                    break;
                }
                
                case ClearType.Plant:
                case ClearType.Harvest:
                case ClearType.Craft:
                {
                    var target = _titleData.Item[clear.Target];
                    var name = _titleData.GetString($"STR_ITEM_{target.Name.ToUpper()}_NAME");
                    _descTMP.text += string.Format(_descriptionStrings[(int)clear.Type], _titleData.GetString(name), clear.Amount);
                    break;
                }
            }
        }
    }

    private void Toggle()
    {
        if (_isExpanded)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    private void Open()
    {
        if (_isExpanded) return;
        
        _isExpanded = true;

        DOTween.To(() => _rect.sizeDelta.y, 
                y =>
                {
                    _rect.sizeDelta = new Vector2(_rect.sizeDelta.x, y);
                },
                ExpandedHeight,
                0.3f)
            .SetEase(Ease.Linear);
        
        _detailBackgroundImg.DOFade(1f, 0.25f).SetEase(Ease.Linear);
        _defaultBackgroundImg.DOFade(0f, 0.25f).SetEase(Ease.Linear);
        
        _titleTMP.rectTransform.DOAnchorPos(_titleExpandedPos, 0.3f).SetEase(Ease.Linear);
        _descTMP.DOFade(1, 0.2f).SetEase(Ease.Linear).SetDelay(0.1f);
        
        DOTween.To(() => _titleTMP.fontSize, 
                x => _titleTMP.fontSize = x, 
                32f, 
                0.3f) 
            .SetEase(Ease.Linear);
        
        _openBtn.gameObject.SetActive(true);
    }

    private void Close()
    {
        if (!_isExpanded) return;
        
        _isExpanded = false;
        DOTween.To(() => _rect.sizeDelta.y, 
                y =>
                {
                    _rect.sizeDelta = new Vector2(_rect.sizeDelta.x, y);
                },
                CollapsedHeight,
                0.3f)
            .SetEase(Ease.Linear);
        
        _detailBackgroundImg.DOFade(0f, 0.25f).SetEase(Ease.Linear);
        _defaultBackgroundImg.DOFade(1f, 0.25f).SetEase(Ease.Linear);
        
        _titleTMP.rectTransform.DOAnchorPos(_titleCollapsedPos, 0.3f).SetEase(Ease.Linear);
        _descTMP.DOFade(0, 0.2f).SetEase(Ease.Linear);
        
        DOTween.To(() => _titleTMP.fontSize, 
                x => _titleTMP.fontSize = x, 
                20f, 
                0.3f) 
            .SetEase(Ease.Linear);
        
        _openBtn.gameObject.SetActive(false);
    }
}
