using System;

using UnityEngine;
using UnityEngine.UI;

public class QuestSlot : MonoBehaviour
{
    public event Action<Quest> OnSlotSelected;
    
    [SerializeField] protected Button _selectBtn;
    [SerializeField] private QuestInfoUpdater _infoUpdater;
    
    protected Quest QuestData;
    
    protected virtual void Awake()
    {
        _selectBtn.onClick.AddListener(() =>
        {
            Debug.Log("Selected");
            OnSlotSelected?.Invoke(QuestData);
        });
    }
    
    public virtual void Initialize(Quest data)
    {
        QuestData = data;
        _infoUpdater.UpdateQuest(data);
    }
}
