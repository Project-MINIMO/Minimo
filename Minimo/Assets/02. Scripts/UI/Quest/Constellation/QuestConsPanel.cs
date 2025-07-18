using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class QuestConsPanel : UIBase
{
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
    
    private const float PrimaryStartX = 120f;
    private const float SecondaryStartX = 200f;
    private const float OpenTargetX = 160f;
    private const float AnimationDuration = 0.3f;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _titleData = App.GetData<TitleData>();
        _questInfos = GetComponentsInChildren<QuestConsInfo>();
        _submissionPanel = manager.GetPanel<QuestSubmissionPanel>();

        _closeBtn.onClick.AddListener(ClosePanel);
    }

    public void OpenSubmissionPanel(DetailQuestData questData)
    {
        _submissionPanel.OpenPanel(questData);
    }

    public override void Show(bool isNew)
    {
        base.Show(isNew);

        AnimateOpen(isNew ? PrimaryStartX : SecondaryStartX);
    }

    public void OpenPanel(DetailQuestData questData)
    {
        OpenPanel();
        SetupQuestInfos(questData);
    }

    private void AnimateOpen(float fromX)
    {
        _canvasGroup2.alpha = 0;
        _canvasGroup2.blocksRaycasts = false;

        _canvasGroup2.alpha = 1;
        _canvasGroup.alpha = 0;
        _canvasRect.anchoredPosition = new Vector2(fromX, 0);

        _canvasRect
            .DOAnchorPosX(OpenTargetX, AnimationDuration)
            .SetEase(Ease.Linear);

        _canvasGroup
            .DOFade(1, AnimationDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => _canvasGroup2.blocksRaycasts = true);
    }

    private void SetupQuestInfos(DetailQuestData questData)
    {
        var quests = _titleData.DetailQuest.Values
            .Where(x => x.ID / 10 == questData.ID / 10)
            .ToList();

        var i = 0;
        
        for (; i < quests.Count && i < _questInfos.Length; i++)
        {
            _questInfos[i].gameObject.SetActive(true);
            _questInfos[i].Initialize(quests[i]);
        }
        for (; i < _questInfos.Length; i++)
        {
            _questInfos[i].gameObject.SetActive(false);
        }

        var questTitleKey = _titleData.Quest[questData.ID / 10].Name;
        
        _titleTMP.text = _titleData.GetString(questTitleKey);

        _iconImg.sprite = _sprites[UnityEngine.Random.Range(0, _sprites.Length)];
    }
}
