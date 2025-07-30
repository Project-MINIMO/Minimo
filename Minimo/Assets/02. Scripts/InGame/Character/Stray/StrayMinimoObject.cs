using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StrayMinimoObject : InteractObject
{
    public StrayMinimoFSM FSM { get; private set; }
    public bool IsGolden { get; private set; }
    
    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _goldenSprite;
    [SerializeField] private GameObject _plunderItemObj;
    [SerializeField] private Image _plunderItemImg;
    
    private StrayMinimoState _currentState = StrayMinimoState.Idle;
    
    private EditManager _editManager;

    private readonly Vector3 _startScale = new(0.23f, 0.23f, 0.23f);
    private readonly Vector3 _endScale = new(0.2f, 0.2f, 0.2f);
    
    private int _lifeTime;
    private int _afterPlunderLifeTime;
    private int _currency;
    private float _currencyRate;
    private float _currencyLostRate;
    
    private int _lifeRemaining;
    private int _holdCurreny;
    private int _lostCurrentAmount;
    private (Item, int) _holdItem;
    
    private Coroutine _lifeTimeRoutine;
    private bool _canClick;
    
    public void Initialize(bool isGorden, Dictionary<string, int> common)
    {
        IsGolden = isGorden;
        
        GetComponentInChildren<SpriteRenderer>().sprite = isGorden ? _goldenSprite : _normalSprite;

        _lifeTime = common["MiaLifeTime"];
        _afterPlunderLifeTime = common[isGorden ? "GoldMiaLootLifeTime" : "MiaLootLifeTime"];
        
        _currency = common["MiaCurrency"];
        _currencyRate = isGorden ? common["GoldMiaCurrency"] : 1;
        _currencyLostRate = common["CurrencyLostRate"] / 100f;
    }

    private void Start()
    {
        _editManager = App.GetManager<EditManager>();
        
        FSM = new StrayMinimoFSM(this);
        ApplyState(StrayMinimoState.Hide);
    }

    private void Update()
    {
        FSM.Update();

        if (_currentState == StrayMinimoState.Idle)
        {
            if (IsAnyCompleteAdvances())
            {
                ApplyState(StrayMinimoState.Plunder);
            }
        }
    }

    public void Spawn(Vector3 position)
    {
        _canClick = true;
        
        transform.position = position;
        ApplyState(StrayMinimoState.Idle);
        _lifeRemaining = _lifeTime;
        
        var holdCurreny = (_currency + AccountInfo.Instance.Level.Count * _currency * 0.1f) * _currencyRate;
        _holdCurreny = Mathf.RoundToInt(holdCurreny);
        Debug.Log(holdCurreny);
        Debug.Log(_holdCurreny);

        var lostCurrentAmount = _holdCurreny * _currencyLostRate;
        _lostCurrentAmount = Mathf.RoundToInt(lostCurrentAmount);
        Debug.Log(_lostCurrentAmount);

        _holdItem = (null, 0);
        _plunderItemObj.SetActive(false);
        
        _lifeTimeRoutine = StartCoroutine(LifetimeRoutine());
    }
    
    private IEnumerator LifetimeRoutine()
    {
        while (_lifeRemaining > 0f)
        {
            yield return new WaitForSeconds(1f);
            _lifeRemaining -= 1;
        }
        
        Despawn();
    }

    public void Despawn()
    {
        _canClick = false;
        transform.DOKill();
        
        if (_lifeTimeRoutine != null)
        {
            StopCoroutine(_lifeTimeRoutine);
            _lifeTimeRoutine = null;
        }
        
        ApplyState(StrayMinimoState.Hide);
    }

    public void SuccessPlunder(Item item, int amount)
    {
        _lifeRemaining = _afterPlunderLifeTime;
        _holdItem = (item, amount);
        _plunderItemObj.SetActive(true);
        _plunderItemImg.sprite = item.Icon;
    }

    private void ApplyState(StrayMinimoState target)
    {
        if (target == _currentState) return;

        _currentState = target;
        FSM.ChangeState(target);
    }
   
    private bool IsAnyCompleteAdvances()
    {
        return _editManager.ActiveAdvanceds.Any(x => x.CurrentState == ProduceState.Complete);
    }

    public override void OnLongPress()
    {
        if (!_canClick) return;
        transform.DOKill();
        transform.DOScale(_startScale, 0.3f);
    }

    public override void OnClickUp()
    {
        if (!_canClick) return;
        transform.DOKill();
        transform.DOScale(_startScale, 0.3f);
        
        var getCurrency = _holdCurreny - _lostCurrentAmount > 0 ? _lostCurrentAmount : _holdCurreny;
        _holdCurreny -= getCurrency;
        AccountInfo.Instance.Gold.AddCount(getCurrency);
        if (_holdCurreny <= 0)
        {
            var (item, amount) = _holdItem;
            if (item != null)
            {
                AccountInfo.Instance.AddItem(item.ID, amount);
            }
            Despawn();
        }
    }
    
    public override void OnClickDown()
    {
        if (!_canClick) return;
        transform.DOKill();
        transform.DOScale(_endScale, 0.3f);
    }
}
