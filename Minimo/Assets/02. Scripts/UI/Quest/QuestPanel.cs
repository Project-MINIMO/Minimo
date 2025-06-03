using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class QuestPanel : UIBase
{
    [Serializable]
    public struct MenuButton
    {
        public int FilterType;
        public Button Button;
        public TextMeshProUGUI Text;
        public GameObject Alert;
    }

    [SerializeField] private RectTransform _backgroundRect;
    [SerializeField] private Transform _questParent;
    [SerializeField] private GameObject _questPrefab;
    [SerializeField] private ScrollRect _scrollRect;

    [SerializeField] private MenuButton[] _menuBtns;
    
    [SerializeField] private UILongPressDetector _longPressDetector;
    
    private QuestManager _questManager;
    
    private List<QuestInfo> _questInfos = new();
    
    public override void Initialize()
    {
        _questManager = App.GetManager<QuestManager>();
        _longPressDetector.OnLongPress = ExpandPanel;
        SetButtonEvent();
    }
    
    private void SetButtonEvent()
    {
        foreach (var button in _menuBtns)
        {
            button.Button.onClick.AddListener(() =>
            {
                button.Alert.SetActive(false);
                FilterQuests(button.FilterType);
            });
        }
    }
    
    private void FilterQuests(int index)
    {
        foreach (var info in _questInfos)
        {
            var isActive = 
                index == 0 
                || index == info.QuestType;
            
            info.gameObject.SetActive(isActive);
        }
        
        _scrollRect.verticalNormalizedPosition = 1;
    }

    public void UpdateQuest(int questType)
    {
        _questInfos.Clear();
        
        var existingInfos = GetComponentsInChildren<QuestInfo>(true);
        
        var quests = _questManager.ActiveQuests.OrderBy(x => x.ID).ToList();
  
        var i = 0;
        
        for (; i < quests.Count; i++)
        {
            var questInfo = i < existingInfos.Length 
                ? existingInfos[i] 
                : Instantiate(_questPrefab, _questParent).GetComponent<QuestInfo>();

            questInfo.Initialize(quests[i]);
            _questInfos.Add(questInfo);
        }

        for (; i < existingInfos.Length; i++)
        {
            existingInfos[i].gameObject.SetActive(false);
        }

        if (questType != 0)
        {
            _menuBtns[questType].Alert.SetActive(true);
        }
    }

    private void ExpandPanel()
    {
        _backgroundRect.DOSizeDelta(new Vector2(800, 1440), 1f);
    }
}
