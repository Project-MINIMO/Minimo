using UnityEngine;
using UnityEngine.UI;

public class QuestListSlot : QuestSlot
{
    [SerializeField] private Image _iconImg;
    
    public override void Initialize(Quest data)
    {
        base.Initialize(data);
        
        _iconImg.sprite = data.Icon;
    }
}