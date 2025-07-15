using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RemainTimeUpdater : MonoBehaviour
{
    [SerializeField] private Image _remainTimeImg;
    [SerializeField] private TextMeshProUGUI _remainTimeTMP;

    private string[] _stateStrings;
    private float _remainTime;
    private float _fullTime;
    
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
    
    public void UpdateTime(float remainTime, int fullTime)
    {
        _remainTime = remainTime;
        _fullTime = fullTime;
        
        switch (remainTime)
        {
            case < 0:
                SetRemainImg(TaskState.Empty);
                SetRemainText(TaskState.Empty);
                break;
            
            case 0:
                SetRemainImg(TaskState.Complete);
                SetRemainText(TaskState.Complete);
                break;
            
            case > 0:
                if (remainTime < fullTime)
                {
                    SetRemainImg(TaskState.Produce);
                    SetRemainText(TaskState.Produce);
                }
                else if (Mathf.Approximately(remainTime, fullTime))
                {
                    SetRemainImg(TaskState.Pending);
                    SetRemainText(TaskState.Pending);
                }
                break;
        }
    }

    private void SetRemainImg(TaskState state)
    {
        if (_remainTimeImg == null) return;

        _remainTimeImg.fillAmount = state switch
        {
            TaskState.Empty => 0,
            TaskState.Pending => 0,
            TaskState.Produce => 1 - _remainTime / _fullTime,
            TaskState.Complete => 1,
            _ => _remainTimeImg.fillAmount
        };
    }
    
    private void SetRemainText(TaskState state)
    {
        if (_remainTimeTMP == null) return;
        
        _remainTimeTMP.text = state switch
        {
            TaskState.Empty or TaskState.Pending or TaskState.Complete => _stateStrings[(int)state],
            TaskState.Produce => FormatTime(_remainTime),
            _ => _remainTimeTMP.text
        };
    }
    
    private string FormatTime(float time)
    {
        var timeSpan = TimeSpan.FromSeconds(time);
        return $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }
}
