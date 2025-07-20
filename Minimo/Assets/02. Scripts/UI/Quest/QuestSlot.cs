using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestSlot : MonoBehaviour
{
    public event Action<Quest> OnSlotSelected;
    
    [SerializeField] protected Button _selectBtn;
    [SerializeField] protected TextMeshProUGUI _titleTMP;
    
    protected Quest QuestData;
    
    protected virtual void Awake()
    {
        _selectBtn.onClick.AddListener(() => OnSlotSelected?.Invoke(QuestData));
    }
    
    public virtual void Initialize(Quest data)
    {
        QuestData = data;
        _titleTMP.text = data.Name;
    }
}
