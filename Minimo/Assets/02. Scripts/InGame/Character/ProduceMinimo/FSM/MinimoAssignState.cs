using UnityEngine;

public class MinimoAssignState : State<MinimoObject>
{
    private static readonly int IsWalk = Animator.StringToHash("IsWalk");
    
    private readonly BehaviorTree _assignTree;

    public MinimoAssignState(MinimoObject owner) : base(owner)
    {
        var blackboard = new Blackboard(owner);
        
        var idleSequence = new SequenceNode
        (
            blackboard,
            new FindNearestWorkPositionAction(blackboard),
            new MoveAction(blackboard),
            new AssignAction(blackboard)
        );
        
        _assignTree = new BehaviorTree(idleSequence);
    }

    public override void Enter()
    {
        _assignTree.Reset();
    }

    public override void Execute()
    {
        var result = _assignTree.Tick();
        if (result is NodeStatus.Success or NodeStatus.Failure)
        {
            Owner.EvaluateAndApplyState();
        }
    }

    public override void Exit()
    {
        Animator.speed = 1;
        Animator.SetBool(IsWalk, false);
    }
}
