using UnityEngine;

public class MinimoIdleState : State<MinimoObject>
{
    private static readonly int IsWalk = Animator.StringToHash("IsWalk");
    private static readonly int IsLay = Animator.StringToHash("IsLay");
    private readonly int _idle = Animator.StringToHash("Idle");
    
    private readonly BehaviorTree _idleTree;

    public MinimoIdleState(MinimoObject owner) : base(owner)
    {
        var blackboard = new Blackboard(owner.gameObject);
        
        var idleSequence = new SequenceNode
        (
            blackboard,
            new FindRestPositionAction(blackboard),
            new MoveAction(blackboard),
            new LayDownAction(blackboard)
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
        Animator.SetBool(IsWalk, false);
        Animator.SetBool(IsLay, false);
    }
}
