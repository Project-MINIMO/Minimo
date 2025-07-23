using System.Collections.Generic;

using UnityEngine;

public class MinimoObject : MonoBehaviour
{
    public Minimo Data { get; private set; }
    public MinimoFSM FSM { get; private set; }
    
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private Transform _parent; 

    public void Initialize(Minimo minimo)
    {
        Data = minimo;
        Data.OnAssignmentChanged += HandleAssignmentChanged;
        
        _parent = transform.parent;
        
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        FSM = new MinimoFSM(this);
        
        HandleAssignmentChanged(Data.AssignedBuilding);
    }

    private void HandleAssignmentChanged(ProduceAdvanced building)
    {
        if (building == null)
        {
            SetChillState();
        }
        else
        {
            SetWorkState(building);
        }
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
    }

    public void SetWorkState(ProduceAdvanced produceObject)
    {
        FSM.ChangeState(MinimoState.Work);
        //_animator.SetTrigger(produceObject.AnimTrigger);
        
        transform.SetParent(produceObject.MinimoWorkingPosition);
        transform.localPosition = Vector3.zero;
        
        SetSpriteFilp(false);
    }
}
