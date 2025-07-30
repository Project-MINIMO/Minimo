using System.Collections;

using DG.Tweening;
using UnityEngine;

public class MinimoAcquireHadler : InteractObject
{
    [SerializeField] private StrayCoinEffect _coinEffect;
    [SerializeField] private GameObject _happyEffect;
    
    private readonly Vector3 _startScale = new(0.2f, 0.2f, 0.2f);
    private readonly Vector3 _endScale = new(0.17f, 0.17f, 0.17f);
    
    private MinimoManager _minimoManager;
    private MinimoObject _minimoObject;
    private Coroutine _clickAnimationRoutine;
    
    private int _requiredCurreny;
    private int _lostCurrentAmount;
    
    private void Awake()
    {
        _requiredCurreny = 100 + AccountInfo.Instance.Level.Count * 20;
        var lostCurrentAmount = _requiredCurreny / 3f;
        _lostCurrentAmount = Mathf.RoundToInt(lostCurrentAmount);

        _minimoManager = App.GetManager<MinimoManager>();
        _minimoObject = GetComponent<MinimoObject>();
    }

    public override void OnLongPress() { }

    public override void OnClickUp()
    {
        switch (_minimoObject.CurrentState)
        {
            case MinimoState.Swim:
                var getCurrency = _requiredCurreny - _lostCurrentAmount > 0 ? _lostCurrentAmount : _requiredCurreny;
                if (AccountInfo.Instance.Gold.Count < getCurrency)
                {
                    App.Notification(NotifyType.GoldLack);
                    return;
                }
                _requiredCurreny -= getCurrency;
                AccountInfo.Instance.Gold.AddCount(-getCurrency);
                _coinEffect.ShowEffect(-getCurrency);
                if (_requiredCurreny <= 0)
                {
                    _minimoObject.ApplyState(MinimoState.Happy);
                    _happyEffect.SetActive(true);
                }
                break;
            
            case MinimoState.Happy:
                if (AccountInfo.Instance.MinimoCapacity <= _minimoManager.ActiveMinimos.Count)
                {
                    App.Notification(NotifyType.MinimoCapacityLack);
                    return;
                }
                _minimoObject.ApplyState(MinimoState.Acquire);
                _minimoManager.ActiveMinimos.Add(_minimoObject.Data);
                _happyEffect.SetActive(false);
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
}
