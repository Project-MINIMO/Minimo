using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum TaskState
{
    Empty,
    Pending,
    Complete,
    Produce,
}

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
    
    public void SetRemainTime(float remainTime, int fullTime)
    {
        switch (remainTime)
        {
            case < 0:
                _remainTimeImg.fillAmount = 0;
                SetRemainText(TaskState.Empty);
                break;
            
            case 0:
                _remainTimeImg.fillAmount = 1;
                SetRemainText(TaskState.Complete);
                break;
            
            case > 0:
                if (remainTime < fullTime)
                {
                    _remainTimeTMP.text = FormatTime(remainTime);
                    _remainTimeImg.fillAmount = 1 - remainTime / fullTime;
                }
                else if (Mathf.Approximately(remainTime, fullTime))
                {
                    _remainTimeImg.fillAmount = 0;
                    SetRemainText(TaskState.Pending);
                }
                break;
        }
    }
    
    private string FormatTime(float time)
    {
        var timeSpan = TimeSpan.FromSeconds(time);
        return $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }
    
    private void SetRemainText(TaskState state)
    {
        if (state is TaskState.Produce) return;

        _remainTimeTMP.text = _stateStrings[(int)state];
    }
}
