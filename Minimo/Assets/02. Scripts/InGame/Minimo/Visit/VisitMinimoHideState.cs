using UnityEngine;

public class VisitMinimoHideState : State<VisitMinimoObject>
{
    private readonly int _isWalk = Animator.StringToHash("IsWalk");
    private readonly BehaviorTree _hideTree;

    public VisitMinimoHideState(VisitMinimoObject owner) : base(owner)
    {
        var blackboard = new Blackboard(owner.gameObject);
        
        var hideSequence = new SequenceNode
        (
            blackboard,
            new FindSpaceshipPositionAction(blackboard),
            new MoveAction(blackboard)
        );
        
        _hideTree = new BehaviorTree(hideSequence);
    }

    public override void Enter()
    {
        _hideTree.Reset();
    }

    public override void Execute()
    {
        var result = _hideTree.Tick();
        if (result == NodeStatus.Success)
        {
            Owner.Hide();
        }
    }

    public override void Exit()
    {
        Animator.SetBool(_isWalk, false);
    }
}
