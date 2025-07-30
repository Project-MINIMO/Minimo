using UnityEngine;

public class MinimoAcquireState : State<MinimoObject>
{
    private readonly int _isSwim = Animator.StringToHash("IsSwim");
    
    private readonly BehaviorTree _acquireTree;

    public MinimoAcquireState(MinimoObject owner) : base(owner)
    {
        var blackboard = new Blackboard(owner.gameObject)
        {
            TargetPosition = new Vector3(0, 1.5f, 0),
            Speed = 5
        };

        var acquireSequence = new SequenceNode
        (
            blackboard,
            new IsEqualIndex(blackboard, 0),
            new MoveForwardAction(blackboard, _isSwim),
            new AcquireFadeOutAction(blackboard),
            new AcquireFadeInAction(blackboard)
        );
        
        _acquireTree = new BehaviorTree(acquireSequence);
    }

    public override void Enter()
    {
        _acquireTree.Reset();
    }

    public override void Execute()
    {
        var result = _acquireTree.Tick();
        if (result == NodeStatus.Success)
        {
            Owner.Acquire();
        }
    }

    public override void Exit()
    {
        Animator.speed = 1;
        Animator.SetBool(_isSwim, false);
    }
}