using System.Collections;

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class VisitMinimoObject : MonoBehaviour
{
    public VisitMinimoFSM FSM { get; private set; }
    public ProduceObject Target { get; private set; }
    
    [SerializeField] private GameObject _requiredItemObj;
    [SerializeField] private Image _requiredItemImg;
    [SerializeField] private Button _requiredItemBtn;
    
    private VisitMinimoState _currentState = VisitMinimoState.Idle;
    
    private VisitMinimoSpawner _spawner;
    
    private int _lifeRemaining;
    private (Item, int) _requiredItem;
    
    private Coroutine _lifeTimeRoutine;
    private Coroutine _clickAnimationRoutine;

    public void Initialize(VisitMinimoSpawner spawner)
    {
        _spawner = spawner;
        _requiredItemBtn.onClick.AddListener(GiveItem);
    }
    
    private void Start()
    {
        FSM = new VisitMinimoFSM(this);
        ApplyState(VisitMinimoState.None);
    }

    private void Update()
    {
        FSM.Update();
    }

    public void Spawn(Item item, ProduceObject target)
    {
        Target = target;
        
        _lifeRemaining = 60;
        _requiredItem = (item, 1);
        _requiredItemImg.sprite = item.Icon;
    }

    public void Land()
    {
        gameObject.SetActive(true);
        transform.DOMoveY(1.3f, 2)
            .OnComplete(() =>
            {
                ApplyState(VisitMinimoState.Idle);
                _requiredItemObj.SetActive(true);
                _lifeTimeRoutine = StartCoroutine(LifetimeRoutine());
            });
    }

    public void Depart()
    {
        transform.DOMoveY(2.3f, 2)
            .OnComplete(() => gameObject.SetActive(false));
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
        _spawner.CallSpaceship(this);
    }

    private void ApplyState(VisitMinimoState target)
    {
        if (target == _currentState) return;

        _currentState = target;
        FSM.ChangeState(target);
    }

    private void GiveItem()
    {
        var item = _requiredItem.Item1;
        var amount = _requiredItem.Item2;
        if (item.Count >= amount)
        {
            _requiredItem.Item1.AddCount(-amount);
            AccountInfo.Instance.Level.AddCount(item.Exp * amount);
            AccountInfo.Instance.Gold.AddCount(item.SellCost * amount);
            Despawn();
        }
        else
        {
            App.Notification(NotifyType.ItemLack);
        }
    }
}