using System;
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
    public bool IsClicked { get; private set; }
    
    [SerializeField] private GameObject _plunderItemObj;
    [SerializeField] private Image _plunderItemImg;
    [SerializeField] private GameObject _sadObj;
    [SerializeField] private GameObject _questionMarkObj;
    
    [SerializeField] private StrayCoinEffect _coinEffect;
    [SerializeField] private GameObject _clickGaugeObj;
    [SerializeField] private Image _clickGaugeImg;
    [SerializeField] private GameObject _shineObj;
    [SerializeField] private RectTransform _warningObj;
    
    private StrayMinimoState _currentState = StrayMinimoState.None;
    public event Action<bool> OnDead;

    private readonly Vector3 _startScale = new(0.2f, 0.2f, 0.2f);
    private readonly Vector3 _endScale = new(0.17f, 0.17f, 0.17f);

    private int _totalLife;
    private int _runLife;
    private int _currentLife;
    
    private int _baseCurrency;
    private int _currentCurrency;
    private float _currencyRate;
    
    private (Item, int) _holdItem;
    
    private Coroutine _clickAnimationRoutine;
    private EditManager _editManager;
    
    private readonly Vector3 _shrinkScale = new(0.8f, 0.8f);
    
    public void Initialize(bool isGorden, Dictionary<string, int> common)
    {
        IsGolden = isGorden;

        if (isGorden)
        {
            GetComponentInChildren<Animator>().SetTrigger("Golden");
        }

        _totalLife = isGorden ? 15 : 10;
        _runLife = isGorden ? 10 : 7;
        
        _baseCurrency = common["MiaCurrency"];
        _currencyRate = isGorden ? common["GoldMiaCurrency"] : 1;
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
    }

    public void Spawn(Vector3 position)
    {
        IsClicked = false;
        transform.position = position;
        
        ApplyState(StrayMinimoState.Plunder);
        
        _currentLife = _totalLife;
        var currentCurrency = (_baseCurrency + AccountInfo.Instance.Level.Count * _baseCurrency * 0.1f) * _currencyRate;
        _currentCurrency = Mathf.RoundToInt(currentCurrency);
        
        _holdItem = (null, 0);
        
        _sadObj.SetActive(false);
        _questionMarkObj.SetActive(false);
        _plunderItemObj.SetActive(false);
        _shineObj.SetActive(false);
        
        _warningObj.gameObject.SetActive(true);
        _warningObj.DOScale(_shrinkScale, 0.5f).SetEase(Ease.InCubic).SetLoops(-1 ,LoopType.Yoyo);;
        
        _clickGaugeImg.fillAmount = 1;
    }
    
    public void Despawn()
    {
        if (_clickAnimationRoutine != null)
        {
            StopCoroutine(_clickAnimationRoutine);
            _clickAnimationRoutine = null;
        }
        
        IsClicked = false;
        OnDead?.Invoke(_holdItem.Item1 == null);
        _warningObj.gameObject.SetActive(false);
        _warningObj.DOKill();
        ApplyState(StrayMinimoState.Hide);
    }

    public void TryPlunder()
    {
        _shineObj.SetActive(true);
    }

    public void SuccessPlunder(Item item, int amount)
    {
        _holdItem = (item, amount);
        _plunderItemObj.SetActive(true);
        _plunderItemImg.sprite = item.Icon;
        _shineObj.SetActive(false);
        ApplyState(StrayMinimoState.Run);
    }

    public void FailPlunder()
    {
        StartCoroutine(FailPlunderAnimation());
    }

    private IEnumerator FailPlunderAnimation()
    {
        ApplyState(StrayMinimoState.Idle);
        _questionMarkObj.SetActive(true);
        _shineObj.SetActive(false);
        yield return new WaitForSeconds(1);
        _questionMarkObj.SetActive(false);
        
        if (IsAnyCompleteAdvances())
        {
            ApplyState(StrayMinimoState.Plunder);
        }
        else
        {
            _sadObj.SetActive(true);
            _warningObj.gameObject.SetActive(false);
            _warningObj.DOKill();
            ApplyState(StrayMinimoState.Run);
        }
    }

    private void ApplyState(StrayMinimoState target)
    {
        if (target == _currentState) return;

        _currentState = target;
        FSM.ChangeState(target);
    }
  
    public override void OnLongPress() { }

    public override void OnClickUp()
    {
        _currentLife--;
        _clickGaugeImg.fillAmount = (float)_currentLife / _totalLife;

        if (_currentLife == _runLife)
        {
            _sadObj.SetActive(true);
            _warningObj.gameObject.SetActive(false);
            _warningObj.DOKill();
            ApplyState(StrayMinimoState.Run);
            
            var (item, amount) = _holdItem;
            if (item != null)
            {
                AccountInfo.Instance.AddItem(item.ID, amount);
                _plunderItemObj.SetActive(false);
                _holdItem.Item1 = null;
            }
        }
        else if (_currentLife <= 0)
        {
            AccountInfo.Instance.Gold.AddCount(_currentCurrency);
            _coinEffect.ShowEffect(_currentCurrency);
            Despawn();
        }
    }
    
    public override void OnClickDown()
    {
        if (IsClicked) return;
        
        if (_clickAnimationRoutine != null)
        {
            StopCoroutine(_clickAnimationRoutine);
            _clickAnimationRoutine = null;
        }
        
        _clickAnimationRoutine = StartCoroutine(ClickAnimation());
    }

    private IEnumerator ClickAnimation()
    {
        if (_currentState == StrayMinimoState.Hide) yield break;
        
        IsClicked = true;
        
        transform.DOKill();
        transform.DOShakePosition(
            duration: 0.3f,
            strength: new Vector3(0.1f, 0.1f, 0f),
            vibrato: 10,
            randomness: 90,
            snapping: false,
            fadeOut: true
        );
        
        transform.DOScale(_endScale, 0.3f);
        yield return new WaitForSeconds(0.3f);

        IsClicked = false;
        
        if (_currentState == StrayMinimoState.Hide) yield break;
        
        transform.DOScale(_startScale, 0.1f);
    }
    
    private bool IsAnyCompleteAdvances()
    {
        return _editManager.ActiveProduces.Any(x => x.CurrentState == ProduceState.Complete);
    }
}
