using System;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class QuestConsPanel : UIBase
{
    [SerializeField] private QuestUIGrouper _grouper;
    [SerializeField] private Button _closeBtn;

    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private Image _iconImg;
    [SerializeField] private Sprite[] _sprites;
    
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private CanvasGroup _canvasGroup2;
    [SerializeField] private RectTransform _canvasRect;

    private QuestSubmissionPanel _submissionPanel;
    private QuestConsInfo[] _questInfos;
    private TitleData _titleData;

    public override void Initialize()
    {
        _titleData = App.GetData<TitleData>();
        _questInfos = GetComponentsInChildren<QuestConsInfo>();
        _submissionPanel = App.GetManager<UIManager>().GetPanel<QuestSubmissionPanel>();

        _closeBtn.onClick.AddListener(() =>
        {
            ClosePanel(() =>
            {
                _grouper.OpenListPanel();
            });
        });
    }

    public void OpenSubmissionPanel(DetailQuestData questData)
    {
        ClosePanel2(() =>
        {
            _submissionPanel.OpenPanel(questData);
        });
    }
    
    public override void OpenPanel()
    {
        _canvasGroup2.alpha = 0;
        _canvasGroup2.blocksRaycasts = false;
        
        base.OpenPanel();  
        
        _canvasGroup2.alpha = 1;
        _canvasGroup.alpha = 0;
        _canvasRect.anchoredPosition = new Vector2(120, 0);
        _canvasRect.DOAnchorPosX(160, 0.3f).SetEase(Ease.Linear);
        _canvasGroup.DOFade(1, 0.3f).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _canvasGroup2.blocksRaycasts = true;
            });
    }
    
    public void OpenPanel2()
    {
        _canvasGroup2.alpha = 0;
        _canvasGroup2.blocksRaycasts = false;
        
        base.OpenPanel();  
        
        _canvasGroup2.alpha = 1;
        _canvasGroup.alpha = 0;
        _canvasRect.anchoredPosition = new Vector2(200, 0);
        _canvasRect.DOAnchorPosX(160, 0.3f).SetEase(Ease.Linear);
        _canvasGroup.DOFade(1, 0.3f).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _canvasGroup2.blocksRaycasts = true;
            });
    }
    
    public void ClosePanel(Action callback = null)
    {
        _canvasGroup2.blocksRaycasts = false;
        _canvasRect.anchoredPosition = new Vector2(160, 0);
        _canvasRect.DOAnchorPosX(120, 0.3f).SetEase(Ease.Linear);
        _canvasGroup.DOFade(0, 0.3f).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _canvasGroup2.blocksRaycasts = true;
                callback?.Invoke();
                base.ClosePanel();
            });
    }
    
    public void ClosePanel2(Action callback = null)
    {
        _canvasGroup2.blocksRaycasts = false;
        _canvasRect.anchoredPosition = new Vector2(160, 0);
        _canvasRect.DOAnchorPosX(200, 0.3f).SetEase(Ease.Linear);
        _canvasGroup.DOFade(0, 0.3f).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _canvasGroup2.blocksRaycasts = true;
                callback?.Invoke();
                base.ClosePanel();
            });
    }

    public void OpenPanel(DetailQuestData questData)
    {
        _grouper.CloseAllExcept(this);
        
        OpenPanel();

        var quests = _titleData.DetailQuest.Values.Where(x => x.ID / 10 == questData.ID / 10).ToList();
        
        var i = 0;
        
        for (; i < quests.Count; i++)
        {
            _questInfos[i].gameObject.SetActive(true);
            _questInfos[i].Initialize(quests[i]);
        }

        for (; i < _questInfos.Length; i++)
        {
            _questInfos[i].gameObject.SetActive(false);
        }
        
        var questTitle = _titleData.Quest[questData.ID / 10].Name;
        _titleTMP.text = _titleData.GetString(questTitle);
        
        var randomNum = UnityEngine.Random.Range(0, _sprites.Length);
        _iconImg.sprite = _sprites[randomNum];
    }
    
    public void OpenPanel2(DetailQuestData questData)
    {
        _grouper.CloseAllExcept(this);
        
        OpenPanel2();

        var quests = _titleData.DetailQuest.Values.Where(x => x.ID / 10 == questData.ID / 10).ToList();
        
        var i = 0;
        
        for (; i < quests.Count; i++)
        {
            _questInfos[i].gameObject.SetActive(true);
            _questInfos[i].Initialize(quests[i]);
        }

        for (; i < _questInfos.Length; i++)
        {
            _questInfos[i].gameObject.SetActive(false);
        }
        
        var questTitle = _titleData.Quest[questData.ID / 10].Name;
        _titleTMP.text = _titleData.GetString(questTitle);
        
        var randomNum = UnityEngine.Random.Range(0, _sprites.Length);
        _iconImg.sprite = _sprites[randomNum];
    }
}
