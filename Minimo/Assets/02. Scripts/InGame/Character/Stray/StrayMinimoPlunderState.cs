using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrayMinimoPlunderState : State<StrayMinimoObject>
{
    private static readonly int IsWalk = Animator.StringToHash("IsWalk");
    
    private readonly BehaviorTree _plunderTree;

    public StrayMinimoPlunderState(StrayMinimoObject owner) : base(owner)
    {
        var blackboard = new Blackboard(owner.gameObject);
        
        var idleSequence = new SequenceNode
        (
            blackboard,
            new FindCompletePositionAction(blackboard),
            new MoveForwardAction(blackboard),
            new PlunderAction(blackboard),
            new FindNearestCornerPositionAction(blackboard),
            new MoveForwardAction(blackboard)
        );
        
        _plunderTree = new BehaviorTree(idleSequence);
    }

    public override void Enter()
    {
        _plunderTree.Reset();
    }

    public override void Execute()
    {
        var result = _plunderTree.Tick();
        if (result is NodeStatus.Success or NodeStatus.Failure)
        {
            Owner.ApplyState(StrayMinimoState.Idle);
        }
    }

    public override void Exit()
    {
        Animator.speed = 1;
        Animator.SetBool(IsWalk, false);
    }
}
