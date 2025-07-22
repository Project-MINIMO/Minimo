using System.Collections.Generic;

using UnityEngine;

public class MinimoObject : MonoBehaviour
{
    public Minimo Data { get; private set; }
    public MinimoFSM FSM { get; private set; }
    public ProduceAdvanced AssignedBuilding { get; private set; }

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private Transform _parent;    //temp
    
    public MinimoManager _minimoManager;
    
    private void Awake()
    {
        var titleData = App.GetData<TitleData>();
        _parent = transform.parent;   //temp
        Data = titleData.UserMinimo[transform.GetSiblingIndex()];    //temp
        
        _minimoManager = App.GetManager<MinimoManager>();

        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        FSM = new MinimoFSM(this);
        SetChillState();

        AddLevel(1);
    }

    private void Update()
    {
        FSM.Update();
    }
    
    public void SetSpriteFilp(bool isFlip)
    {
        _spriteRenderer.flipX = isFlip;
    }
    
    public void SetSpriteOrder(int order)
    {
        _spriteRenderer.sortingOrder = order;
    }

    public void SetAnimation(string trigger, bool isActive)
    {
        _animator.SetBool(trigger, isActive);
    }

    public void SetChillState()
    {
        //var randomIndex = Random.Range(0, 2);
        //FSM.ChangeState(randomIndex == 0 ? MinimoState.Idle : MinimoState.Walk);
        FSM.ChangeState(MinimoState.Idle);
        
        transform.SetParent(_parent);   //temp
        transform.localPosition = Vector3.zero;   //temp

        if (AssignedBuilding != null)
        {
            AssignedBuilding.UnplaceMinimo();
            AssignedBuilding = null;
            _minimoManager.OnMinimoUnassigned(this);
        }
    }

    public void SetWorkState(ProduceAdvanced produceObject)
    {
        FSM.ChangeState(MinimoState.Work);
        //_animator.SetTrigger(produceObject.AnimTrigger);
        
        transform.SetParent(produceObject.MinimoWorkingPosition);
        transform.localPosition = Vector3.zero;
        
        SetSpriteFilp(false);

        if (AssignedBuilding != null)
        {
            AssignedBuilding.UnplaceMinimo();
            _minimoManager.OnMinimoUnassigned(this);
        }
        AssignedBuilding = produceObject;
        produceObject.PlaceMinimo(this);
        
        _minimoManager.OnMinimoAssigned(this);
        
        foreach (var ability in Data.Abilities)
        {
            if (ability.IsApplicableTo(produceObject))
            {
                ability.Apply(produceObject);
            }
        }
    }

    public void AddLevel(int amount)
    {
        if (AssignedBuilding != null)
        {
            _minimoManager.OnMinimoUnassigned(this);
        }

        Data.AddLevel(amount);
        
        if (AssignedBuilding != null)
        {
            foreach (var ability in Data.Abilities)
            {
                if (ability.IsApplicableTo(AssignedBuilding))
                {
                    ability.Apply(AssignedBuilding);
                }
            }
            
            _minimoManager.OnMinimoAssigned(this);
        }
    }
}
