using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RemainTimeUpdater : MonoBehaviour
{
    [SerializeField] private Image _remainTimeImg;
    [SerializeField] private TextMeshProUGUI _remainTimeTMP;

    private string[] _stateStrings;
    
    private void Awake()
    {
        _remainTimeTMP.text = string.Empty;
        
        var titleData = App.GetData<TitleData>();
        _stateStrings = new[]
        {
            titleData.GetString("STR_PRODUCE_SLOTSTATE_EMPTY"),
            titleData.GetString("STR_PRODUCE_SLOTSTATE_PENDING"),
            titleData.GetString("STR_PRODUCE_SLOTSTATE_COMPLETE"),
        };
    }

    public void ClearTime()
    {   
        _remainTimeTMP.SetText(_stateStrings?[(int)TaskState.Empty]);
        
        if (_remainTimeImg == null) return;
        _remainTimeImg.fillAmount = 0;
    }
    
    public void UpdateTime(float remainTime, float fullTime)
    {
        var state = DetermineState(remainTime, fullTime);
        
        SetRemainImg(state, remainTime, fullTime);
        SetRemainText(state, remainTime);
    }
    
    private TaskState DetermineState(float remain, float full)
    {
        if (Mathf.Approximately(remain, 0)) return TaskState.Complete;
        return remain < full ? TaskState.Produce : TaskState.Pending;
    }

    private void SetRemainImg(TaskState state, float remain, float full)
    {
        if (_remainTimeImg == null) return;

        _remainTimeImg.fillAmount = state switch
        {
            TaskState.Pending => 0,
            TaskState.Produce => 1 - remain / full,
            TaskState.Complete => 1,
            _ => _remainTimeImg.fillAmount
        };
    }
    
    private void SetRemainText(TaskState state, float remain)
    {
        if (_remainTimeTMP == null) return;
        
        _remainTimeTMP.text = state switch
        {
            TaskState.Pending or TaskState.Complete => _stateStrings?[(int)state],
            TaskState.Produce => FormatTime(remain),
            _ => _remainTimeTMP.text
        };
    }
    
    private string FormatTime(float time)
    {
        var timeSpan = TimeSpan.FromSeconds(time);
        return $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }
}
