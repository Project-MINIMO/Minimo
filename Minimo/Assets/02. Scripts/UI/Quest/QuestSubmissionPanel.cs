using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestSubmissionPanel : UIBase
{
    public override bool IsUseBlur => true;

    [SerializeField] private Button _closeBtn;
    [SerializeField] private QuestTransitioner _transitioner;
    [SerializeField] private QuestInfoUpdater _infoUpdater;
    
    [SerializeField] private TextMeshProUGUI _progressTMP;
    [SerializeField] private QuestItemSelectedSlot[] _selectedSlots; 
    [SerializeField] private Button _submitBtn;  
    [SerializeField] private Button _cancelBtn;
    
    [SerializeField] private ItemInfoUpdater[] _resultInfos;
    [SerializeField] private Sprite _goldSprite;
    [SerializeField] private Sprite _expSprite;
    
    private Dictionary<QuestCondition, ISubmissionStrategy> _strategies;
    private ISubmissionStrategy _currentStrategy;
    
    private QuestManager _questManager;
    private TitleData _titleData;
    private Quest _questData;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _questManager = App.GetManager<QuestManager>();
        _titleData = App.GetData<TitleData>();
        
        var slots = GetComponentsInChildren<ItemSlot>(true);
        foreach (var slot in slots)
        {
            slot.OnItemSelected += OnItemSelected;
        }
  
        _strategies = new Dictionary<QuestCondition, ISubmissionStrategy>
        {
            { QuestCondition.Normal, new NormalSubmissionStrategy() },
            { QuestCondition.Choice, new ChoiceSubmissionStrategy() },
            { QuestCondition.Quiz,   new QuizSubmissionStrategy() }
        };
        
        foreach (var strategy in _strategies.Values)
        {
            strategy.Initialize(_progressTMP, _selectedSlots);
        }

        _submitBtn.onClick.AddListener(() => _currentStrategy.OnSubmit());
        _cancelBtn.onClick.AddListener(() => _currentStrategy.OnCancel());
        _closeBtn.onClick.AddListener(ClosePanel);
    }
    
    public override void Show(bool isNew)
    {
        base.Show(isNew);

        _transitioner.Open(isNew);
    }

    public override void OpenPanel()
    {
        base.OpenPanel();  
      
        _questData = _questManager.CurrentQuest;
        _infoUpdater.UpdateQuest(_questData);
        UpdateRewardSlots();
        _currentStrategy = _strategies[_questData.Condition];
        _currentStrategy.Setup(_questData);
    }

    private void UpdateRewardSlots()
    {
        var i = 0;
        
        for (; i < _questData.Reward.Length; i++)
        {
            _resultInfos[i].gameObject.SetActive(true);
            
            var reward = _questData.Reward[i];
            var sprite = reward.Type switch
            {
                RewardType.Gold => _goldSprite,
                RewardType.Exp  => _expSprite,
                RewardType.Item => _titleData.Item[reward.Target].Icon,
                _                => _goldSprite
            };
            _resultInfos[i].UpdateItem(sprite, reward.Amount);
        }

        for (; i < _resultInfos.Length; i++)
        {
            _resultInfos[i].gameObject.SetActive(false);
        }
    }

    private void OnItemSelected(InventorySlot<Item> slot) => _currentStrategy?.SelectItem(slot.Item);
}
