using System;

using UnityEngine;
using TMPro;

public class QuestCompactSlot : QuestSlot
{
    [SerializeField] private TextMeshProUGUI _nameTMP; 
    
    [SerializeField] private GameObject _defaultBackground;
    [SerializeField] private GameObject _completeBackground;
    [SerializeField] private GameObject[] _iconObjs;
    
    private Quest _activeQuest;
    
    protected override void Awake()
    {
        base.Awake();
        
        var titleData = App.GetData<TitleData>();
        for (var i = 0; i < _iconObjs.Length; i++)
        {
            var text = _iconObjs[i].GetComponentInChildren<TextMeshProUGUI>();
            text.SetText(GetTitleString(i, titleData));
        }
    }

    private void Update()
    {
        if (_activeQuest == null) return;
        if (_activeQuest.Condition != QuestCondition.Normal) return;
        
        foreach (var clear in _activeQuest.Clear)
        {
            if (!clear.IsCompleted) return;
        }
        
        _defaultBackground.SetActive(false);
        _completeBackground.SetActive(true);
        
        transform.SetAsFirstSibling();

        _activeQuest = null;
    }
    
    public override void Initialize(Quest data)
    {
        base.Initialize(data);
        
        _activeQuest = data;
        
        _iconObjs[GetIndex(data.Type)].SetActive(true);
        _defaultBackground.SetActive(true);
        _completeBackground.SetActive(false);
    }

    private string GetTitleString(int index, TitleData titleData) => index switch
    {
        0 => titleData.GetString("STR_QUEST_MAIN"),
        1 => titleData.GetString("STR_QUEST_SIDE"),
        2 => titleData.GetString("STR_QUEST_CONSTELLATION"),
        _ => string.Empty
    };
    
    private int GetIndex(QuestType questType) => questType switch
    {
        QuestType.Guide => 0,
        QuestType.Story => 0,
        QuestType.Side => 1,
        QuestType.Wish => 1,
        QuestType.Constellation => 2,
        _ => 0
    };
}
