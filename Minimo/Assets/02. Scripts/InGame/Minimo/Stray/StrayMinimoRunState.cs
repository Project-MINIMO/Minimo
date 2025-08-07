using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrayMinimoRunState : State<StrayMinimoObject>
{
    private readonly int _isPlunder = Animator.StringToHash("IsPlunder");
    
    private readonly BehaviorTree _runTree;

    public StrayMinimoRunState(StrayMinimoObject owner) : base(owner)
    {
        var blackboard = new Blackboard(owner.gameObject);
        
        var runSequence = new SequenceNode
        (
            blackboard,
            new FindNearestCornerPositionAction(blackboard),
            new MoveForwardAction(blackboard, _isPlunder)
        );
        
        _runTree = new BehaviorTree(runSequence);
    }

    public override void Enter()
    {
        _runTree.Reset();
        Animator.SetBool(_isPlunder, true);
    }

    public override void Execute()
    {
        var result = _runTree.Tick();
        if (result is NodeStatus.Success)
        {
            Owner.Despawn();
        }
    }

    public override void Exit()
    {
        Animator.speed = 1;
        Animator.SetBool(_isPlunder, false);
    }
}
