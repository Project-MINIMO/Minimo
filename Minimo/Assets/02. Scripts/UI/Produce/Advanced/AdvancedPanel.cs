using System.Linq;

using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdvancedPanel : UIBase
{
    [SerializeField] private Button _closeBtn;
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private ProduceTaskBtn[] _taskBtns;

    private ProduceManager _produceManager;
    
    public override void Initialize()
    {
        _produceManager = App.GetManager<ProduceManager>();
        
        _taskBtns = GetComponentsInChildren<ProduceTaskBtn>(true);
        _closeBtn.onClick.AddListener(()=>
        {
            _produceManager.DeactiveProduce();
        });
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
        
        InitTaskBtns();
        _titleTMP.text = App.GetData<TitleData>().GetString(_produceManager.CurrentProduceObject.BuildingData.Name);
    }

    private void InitTaskBtns()
    {
        foreach (var taskBtn in _taskBtns)
        {
            taskBtn.Initialize(_produceManager.CurrentProduceObject);
        }
    }
    
    public bool ExpandTaskBtn()
    {
        foreach (var taskBtn in _taskBtns)
        {
            if (!taskBtn.IsActive)
            {
                taskBtn.gameObject.SetActive(true);
                break;
            }
        }
        
        return  _taskBtns.Any(btn => !btn.IsActive);
    }
}
