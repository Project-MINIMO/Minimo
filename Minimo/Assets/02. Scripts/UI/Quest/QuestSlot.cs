using System;

using UnityEngine;
using UnityEngine.UI;

public class QuestSlot : MonoBehaviour
{
    public event Action<Quest> OnSlotSelected;
    public Quest CurrentQuest { get; protected set; }
    
    [SerializeField] protected Button _selectBtn;
    [SerializeField] private QuestInfoUpdater _infoUpdater;
    
    protected virtual void Awake()
    {
        _selectBtn.onClick.AddListener(() => OnSlotSelected?.Invoke(CurrentQuest));
    }
    
    public virtual void Initialize(Quest data)
    {
        CurrentQuest = data;
        _infoUpdater.UpdateQuest(data);
    }
}
