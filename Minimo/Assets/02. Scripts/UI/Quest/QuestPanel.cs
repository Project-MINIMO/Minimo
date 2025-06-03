using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    
    [SerializeField] private Transform _questParent;
    [SerializeField] private GameObject _questPrefab;
    [SerializeField] private ScrollRect _scrollRect;

    [SerializeField] private MenuButton[] _menuBtns;
    
    private QuestManager _questManager;
    
    private List<QuestInfo> _questInfos;
    
    public override void Initialize()
    {
        _questManager = App.GetManager<QuestManager>();

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

    public void UpdateQuest()
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
    }
}
