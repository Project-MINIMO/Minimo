using UnityEngine;

public class StrayMinimoPlunderState : State<StrayMinimoObject>
{
    private readonly int _isWalk = Animator.StringToHash("IsWalk");
    private readonly int _isPlunder = Animator.StringToHash("IsPlunder");
    
    private readonly BehaviorTree _plunderTree;

    public StrayMinimoPlunderState(StrayMinimoObject owner) : base(owner)
    {
        var blackboard = new Blackboard(owner.gameObject);
        
        var plunderSequence = new SequenceNode
        (
            blackboard,
            new FindCompletePositionAction(blackboard),
            new MoveForwardAction(blackboard, _isWalk),
            new PlunderAction(blackboard)
        );
        
        _plunderTree = new BehaviorTree(plunderSequence);
    }

    public override void Enter()
    {
        _plunderTree.Reset();
        Animator.SetBool(_isPlunder, true);
    }

    public override void Execute()
    {
        var result = _plunderTree.Tick();
        if (result is NodeStatus.Success)
        {
            Owner.Despawn();
        }
    }

    public override void Exit()
    {
        Animator.speed = 1;
        Animator.SetBool(_isWalk, false);
        Animator.SetBool(_isPlunder, false);
    }
}
