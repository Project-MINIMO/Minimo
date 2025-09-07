using System.Collections;

using DG.Tweening;
using TMPro;
using UnityEngine;

public class MinimoAcquireHandler : InteractObject
{
    [SerializeField] private GameObject _coinObj;
    [SerializeField] private TextMeshProUGUI _coinTMP;
    
    private readonly Vector3 _startScale = new(0.2f, 0.2f, 0.2f);
    private readonly Vector3 _endScale = new(0.17f, 0.17f, 0.17f);
    
    private MinimoManager _minimoManager;
    private MinimoObject _minimoObject;
    private Coroutine _clickAnimationRoutine;
    private PopUpPanel _popUpPanel;
    
    public int RequiredCurreny { get; private set; }
    
    private void Awake()
    {
        _minimoManager = App.GetManager<MinimoManager>();
        _minimoObject = GetComponent<MinimoObject>();

        _popUpPanel = App.GetManager<UIManager>().GetPanel<PopUpPanel>();
    }
    
    private void Start()
    {
        if (_minimoObject.CurrentState == MinimoState.Idle)
        {
            _minimoManager.ActiveMinimos.Add(_minimoObject.Data);
            _coinObj.SetActive(false);
            GetComponent<Collider2D>().enabled = false;
            enabled = false;
        }
    }

    private void OnEnable()
    {
        RequiredCurreny = 100 + AccountInfo.Instance.Level.Count * 20;
        _coinTMP.text = RequiredCurreny.ToString();
        _coinObj.SetActive(true);
    }

    public override void OnLongPress() { }

    public override void OnClickUp()
    {
        switch (_minimoObject.CurrentState)
        {
            case MinimoState.Swim:
                _popUpPanel.OpenPanel(PopUpType.MinimoAcquire, this);
                StartCoroutine(WaitForPopUpClosed());
                break;
        }
    }
    
    public override void OnClickDown()
    {
        if (_minimoObject.CurrentState == MinimoState.Acquire) return;
        
        if (_clickAnimationRoutine != null)
        {
            StopCoroutine(_clickAnimationRoutine);
            _clickAnimationRoutine = null;
        }
        
        _clickAnimationRoutine = StartCoroutine(ClickAnimation());
    }

    private IEnumerator ClickAnimation()
    {
        transform.DOKill();
        transform.DOScale(_endScale, 0.3f);
        yield return new WaitForSeconds(0.3f);

        transform.DOScale(_startScale, 0.3f);
    }

    public void Acquire()
    {
        if (AccountInfo.Instance.MinimoCapacity <= _minimoManager.ActiveMinimos.Count)
        {
            App.Notification(NotifyType.MinimoCapacityLack);
            _popUpPanel.OpenPanel(PopUpType.MinimoExpand);
            return;
        }
        
        if (AccountInfo.Instance.Gold.Count < RequiredCurreny)
        {
            App.Notification(NotifyType.GoldLack);
            return;
        }
        
        _coinObj.SetActive(false);
        
        if (_clickAnimationRoutine != null)
        {
            StopCoroutine(_clickAnimationRoutine);
            _clickAnimationRoutine = null;
        }
        
        AccountInfo.Instance.Gold.AddCount(-RequiredCurreny);
        
        _minimoObject.ApplyState(MinimoState.Acquire);
        _minimoManager.ActiveMinimos.Add(_minimoObject.Data);

        GetComponent<Collider2D>().enabled = false;
        enabled = false;
    }
    
    private IEnumerator WaitForPopUpClosed()
    {
        _minimoObject.IsClicked = true;
        
        yield return new WaitUntil(() => !_popUpPanel.gameObject.activeSelf);

        _minimoObject.IsClicked = false;
    }
}
