using System;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class QuestSubmissionPanel : UIBase
{
    public override bool IsUseBlur => true;
    
    [Serializable]
    public struct RewardInfo
    {
        public GameObject Obj;
        public Image Icon;
        public TextMeshProUGUI Amount;
    }

    [SerializeField] private Button _closeBtn;
    [SerializeField] private QuestTransitioner _transitioner;
    [SerializeField] private TextMeshProUGUI _progressTMP;
    
    [SerializeField] private RewardInfo[] _rewardInfos;
    [SerializeField] private Sprite _goldSprite;
    [SerializeField] private Sprite _expSprite;

    [SerializeField] private QuestItemSelectedSlot[] _selectedSlots;
    
    [SerializeField] private Button _cancelBtn;
    [SerializeField] private Button _submitBtn;

    private QuestManager _questManager;
    private TitleData _titleData;
    private Quest _questData;
    
    private string[] _clearStrings;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _questManager = App.GetManager<QuestManager>();
        _titleData = App.GetData<TitleData>();
        
        _clearStrings = new[]
        {
            _titleData.GetString("STR_QUEST_CLEAR_LEVEL"),
            _titleData.GetString("STR_QUEST_CLEAR_PREP"),
            _titleData.GetString("STR_QUEST_CLEAR_HARVEST"),
            _titleData.GetString("STR_QUEST_CLEAR_CRAFT"),
            _titleData.GetString("STR_QUEST_CLEAR_WISH"),
            _titleData.GetString("STR_QUEST_CLEAR_BUILD"),
        };
  
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
       
        _progressTMP.text = "0 / 0";
        _progressTMP.gameObject.SetActive(_questData.Condition == QuestCondition.Normal);

        SetRewardInfo();
        SetClearInfo();
    }

    private void SetRewardInfo()
    {
        var i = 0;
        
        for (; i < _questData.Reward.Length; i++)
        {
            var reward = _questData.Reward[i];
            var info = _rewardInfos[i];
            
            info.Obj.SetActive(true);

            switch (reward.Type)
            {
                case RewardType.Gold:
                    info.Icon.sprite = _goldSprite;
                    break;
                
                case RewardType.Exp:
                    info.Icon.sprite = _expSprite;
                    break;
                
                case RewardType.Item:
                {
                    var item = _titleData.Item[reward.Target];
                    info.Icon.sprite = Resources.Load<Sprite>($"Item/{item.Name}");
                    break;
                }
            }
            
            info.Amount.text = reward.Amount.ToString();
        }

        for (; i < _rewardInfos.Length; i++)
        {
            _rewardInfos[i].Obj.SetActive(false);
        }
    }

    private void SetClearInfo()
    {
        var i = 0;
        
        for (; i < _questData.Clear.Length; i++)
        {
            var clear = _questData.Clear[i];
            var info = _selectedSlots[i];
            
            info.gameObject.SetActive(true);

            switch (clear.Type)
            {
                case ClearType.UserLevel:
                case ClearType.Plant:
                case ClearType.Harvest:
                case ClearType.Craft:
                case ClearType.Build:
                    info.gameObject.SetActive(false);
                    break;
                
                case ClearType.Wish:
                    if (_questData.Condition == QuestCondition.Normal)
                    {
                        var item = AccountInfo.Instance.Items[clear.Target];
                        info.Initialize(item, clear.Amount);
                    }
                    else
                    {
                        info.Initialize(_questData.Condition);
                    }
                    break;
            }
        }

        for (; i < _selectedSlots.Length; i++)
        {
            _selectedSlots[i].gameObject.SetActive(false);
        }
    }

    public bool CanSelectItem(Item item)
    {
        if (_questData.Condition == QuestCondition.Normal)
        {
            var slot = _selectedSlots.FirstOrDefault(x => x.Item == item);
            
            if (slot == null) return false;
            if (!slot.CanSelected) return false;
            
            slot.AddItem(item);
        }
        else
        {
            var slot = _selectedSlots.FirstOrDefault(x => x.CanSelected);

            if (slot == null) return false;
            
            slot.AddItem(item);
        }

        return true;
    }
}
