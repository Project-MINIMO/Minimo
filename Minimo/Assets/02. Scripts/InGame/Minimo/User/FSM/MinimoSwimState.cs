using UnityEngine;

public class MinimoSwimState : State<MinimoObject>
{
    private readonly int _isSwim = Animator.StringToHash("IsSwim");
    private readonly int _isSwimIdle = Animator.StringToHash("IsSwimIdle");
    
    private readonly BehaviorTree _swimTree;

    public MinimoSwimState(MinimoObject owner) : base(owner)
    {
        var blackboard = new Blackboard(owner.gameObject);
        
        var swimSequence = new SelectorNode
        (
            blackboard,
            new GetRandomActionIndex(blackboard, 2),
            new SequenceNode
            (
                blackboard,
                new IsEqualIndex(blackboard, 0),
                new FindSwimPositionAction(blackboard),
                new MoveForwardAction(blackboard, _isSwim)
            ),
            new SequenceNode
            (
                blackboard,
                new IsEqualIndex(blackboard, 1),
                new SwimIdleAction(blackboard)
            )
        );
        
        _swimTree = new BehaviorTree(swimSequence);
    }

    public override void Enter()
    {
        _swimTree.Reset();
    }

    public override void Execute()
    {
        _swimTree.Tick();
    }

    public override void Exit()
    {
        Animator.speed = 1;
        Animator.SetBool(_isSwim, false);
        Animator.SetBool(_isSwimIdle, false);
    }
}
