using UnityEngine;

public class VisitMinimoIdleState : State<VisitMinimoObject>
{
    private readonly int _isWalk = Animator.StringToHash("IsWalk");
    private readonly int _isWait = Animator.StringToHash("IsWait");
    
    private readonly BehaviorTree _idleTree;

    public VisitMinimoIdleState(VisitMinimoObject owner) : base(owner)
    {
        var blackboard = new Blackboard(owner.gameObject);
        
        var idleSequence = new SequenceNode
        (
            blackboard,
            new FindVisitTargetPositionAction(blackboard),
            new MoveAction(blackboard),
            new VisitWaitAction(blackboard)
        );
        
        _idleTree = new BehaviorTree(idleSequence);
    }

    public override void Enter()
    {
        _idleTree.Reset();
    }

    public override void Execute()
    {
        _idleTree.Tick();
    }

    public override void Exit()
    {
        Animator.SetBool(_isWalk, false);
        Animator.SetBool(_isWait, false);
    }
}
