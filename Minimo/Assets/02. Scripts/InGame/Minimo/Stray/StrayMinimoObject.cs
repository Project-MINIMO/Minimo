using System.Collections;
using System.Collections.Generic;

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
    [SerializeField] private StrayCoinEffect _coinEffect;
    [SerializeField] private GameObject _clickGaugeObj;
    [SerializeField] private Image _clickGaugeImg;
    [SerializeField] private GameObject _shineObj;
    
    private StrayMinimoState _currentState = StrayMinimoState.None;

    private readonly Vector3 _startScale = new(0.2f, 0.2f, 0.2f);
    private readonly Vector3 _endScale = new(0.17f, 0.17f, 0.17f);

    private const int TotalLife = 10;
    private int _currentLife;
    
    private int _baseCurrency;
    private int _currentCurrency;
    private float _currencyRate;
    
    private (Item, int) _holdItem;
    
    private Coroutine _clickAnimationRoutine;
    
    public void Initialize(bool isGorden, Dictionary<string, int> common)
    {
        IsGolden = isGorden;

        if (isGorden)
        {
            GetComponentInChildren<Animator>().SetTrigger("Golden");
        }
        
        _baseCurrency = common["MiaCurrency"];
        _currencyRate = isGorden ? common["GoldMiaCurrency"] : 1;
    }

    private void Start()
    {
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
        transform.localScale = _startScale;
        
        ApplyState(StrayMinimoState.Plunder);
        
        _currentLife = TotalLife;
        var currentCurrency = (_baseCurrency + AccountInfo.Instance.Level.Count * _baseCurrency * 0.1f) * _currencyRate;
        _currentCurrency = Mathf.RoundToInt(currentCurrency);
        
        _holdItem = (null, 0);
        _plunderItemObj.SetActive(false);
        _clickGaugeImg.fillAmount = 0;
        
        _shineObj.SetActive(false);
    }
    
    public void Despawn()
    {
        if (_clickAnimationRoutine != null)
        {
            StopCoroutine(_clickAnimationRoutine);
            _clickAnimationRoutine = null;
        }
        
        IsClicked = false;
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
        _clickGaugeImg.fillAmount = (float)_currentLife / TotalLife;

        if (_currentLife <= 3)
        {
            ApplyState(StrayMinimoState.Run);
        }
        else if (_currentLife <= 0)
        {
            var (item, amount) = _holdItem;
            if (item != null)
            {
                AccountInfo.Instance.AddItem(item.ID, amount);
            }
            
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
}
