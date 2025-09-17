using UnityEngine;

public class MinimoIdleState : State<MinimoObject>
{
    private readonly int _isWalk = Animator.StringToHash("IsWalk");
    private readonly int _isLay = Animator.StringToHash("IsLay");
    private readonly int _idle = Animator.StringToHash("Idle");
    
    private readonly BehaviorTree _idleTree;

    public MinimoIdleState(MinimoObject owner) : base(owner)
    {
        var blackboard = new Blackboard(owner.gameObject);
        
        var idleSequence = new SelectorNode
        (
            blackboard,
            new GetMinimoIdleIndex(blackboard),
            new SequenceNode
            (
                blackboard,
                new IsEqualIndex(blackboard, 0),
                new FindRestPositionAction(blackboard),
                new MoveAction(blackboard)
            ),
            new SequenceNode
            (
                blackboard,
                new IsEqualIndex(blackboard, 1),
                new SitAction(blackboard)
            )
        );
        
        _idleTree = new BehaviorTree(idleSequence);
    }

    public override void Enter()
    {
        Animator.SetTrigger(_idle);
        _idleTree.Reset();
    }

    public override void Execute()
    {
        _idleTree.Tick();
    }

    public override void Exit()
    {
        Animator.speed = 1;
        Animator.SetBool(_isWalk, false);
        Animator.SetBool(_isLay, false);
    }
}
