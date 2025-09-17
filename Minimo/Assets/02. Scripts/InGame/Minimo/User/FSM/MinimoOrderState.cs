using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimoOrderState : State<MinimoObject>
{
    private readonly int _isWalk = Animator.StringToHash("IsWalk");
    private readonly int _isWork = Animator.StringToHash("IsWork");
    private readonly int _randomIndex = Animator.StringToHash("RandomIndex");
    private readonly int _work = Animator.StringToHash("Work");
    
    private readonly BehaviorTree _workTree;

    public MinimoOrderState(MinimoObject owner) : base(owner)
    {
        var blackboard = new Blackboard(owner.gameObject);
        
        var workSequence = new SequenceNode
        (
            blackboard,
            new FindOrderPositionAction(blackboard),
            new MoveOrderAction(blackboard),
            new OrderAction(blackboard)
        );
        
        _workTree = new BehaviorTree(workSequence);
    }

    public override void Enter()
    {
        Animator.SetTrigger(_work);
        _workTree.Reset();
    }

    public override void Execute()
    {
        if (_workTree.Tick() == NodeStatus.Failure)
        {
            Owner.ApplyState(MinimoState.Idle);
            OrderManager.Instance.FailOrder();
        }
    }

    public override void Exit()
    {
        Animator.speed = 1;
        Animator.SetBool(_isWalk, false);
        Animator.SetBool(_isWork, false);
        Animator.SetInteger(_randomIndex, 2);
    }
}
