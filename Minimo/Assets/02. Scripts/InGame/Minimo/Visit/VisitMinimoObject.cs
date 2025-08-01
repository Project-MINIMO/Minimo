using System.Collections;

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class VisitMinimoObject : InteractObject
{
    public VisitMinimoFSM FSM { get; private set; }
    public ProduceAdvanced Target { get; private set; }
    
    [SerializeField] private GameObject _requiredItemObj;
    [SerializeField] private Image _requiredItemImg;
    
    private VisitMinimoState _currentState = VisitMinimoState.Idle;
    
    private readonly Vector3 _startScale = new(0.2f, 0.2f, 0.2f);
    private readonly Vector3 _endScale = new(0.17f, 0.17f, 0.17f);
    
    private int _lifeRemaining;
    private (Item, int) _requiredItem;
    
    private Coroutine _lifeTimeRoutine;
    private Coroutine _clickAnimationRoutine;
    
    private void Start()
    {
        FSM = new VisitMinimoFSM(this);
        ApplyState(VisitMinimoState.None);
    }

    private void Update()
    {
        FSM.Update();
    }

    public void Spawn(Item item, ProduceAdvanced advanced)
    {
        Target = advanced;
        
        ApplyState(VisitMinimoState.Idle);
        _lifeRemaining = 30;

        _requiredItem = (item, 1);
        _requiredItemObj.SetActive(true);
        _requiredItemImg.sprite = item.Icon;
        
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
        if (_clickAnimationRoutine != null)
        {
            StopCoroutine(_clickAnimationRoutine);
            _clickAnimationRoutine = null;
        }
        
        if (_lifeTimeRoutine != null)
        {
            StopCoroutine(_lifeTimeRoutine);
            _lifeTimeRoutine = null;
        }
        
        ApplyState(VisitMinimoState.Hide);
        _requiredItemObj.SetActive(false);
    }

    public void Hide()
    {
        ApplyState(VisitMinimoState.None);
    }

    private void ApplyState(VisitMinimoState target)
    {
        if (target == _currentState) return;

        _currentState = target;
        FSM.ChangeState(target);
    }
  
    public override void OnLongPress() { }

    public override void OnClickUp()
    {
        if (_requiredItem.Item1.Count >= _requiredItem.Item2)
        {
            _requiredItem.Item1.AddCount(-_requiredItem.Item2);
            Despawn();
        }
    }
    
    public override void OnClickDown()
    {
        if (_clickAnimationRoutine != null)
        {
            StopCoroutine(_clickAnimationRoutine);
            _clickAnimationRoutine = null;
        }
        
        _clickAnimationRoutine = StartCoroutine(ClickAnimation());
    }

    private IEnumerator ClickAnimation()
    {
        if (_currentState == VisitMinimoState.Hide) yield break;
        
        transform.DOKill();
        transform.DOScale(_endScale, 0.3f);
        yield return new WaitForSeconds(0.3f);
        
        if (_currentState == VisitMinimoState.Hide) yield break;
        
        transform.DOScale(_startScale, 0.3f);
    }
}