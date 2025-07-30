using UnityEngine;

public class MinimoWorkState : State<MinimoObject>
{
    private readonly int _isWalk = Animator.StringToHash("IsWalk");
    private readonly int _isWork = Animator.StringToHash("IsWork");
    private readonly int _randomIndex = Animator.StringToHash("RandomIndex");
    private readonly int _work = Animator.StringToHash("Work");
    
    private readonly BehaviorTree _workTree;

    public MinimoWorkState(MinimoObject owner) : base(owner)
    {
        var blackboard = new Blackboard(owner.gameObject);
        
        var workSequence = new SequenceNode
        (
            blackboard,
            new FindWorkPositionAction(blackboard),
            new MoveAction(blackboard),
            new RepeatUntilFailNode(blackboard, new SequenceNode
                (
                    blackboard,
                    new ActiveProductionAction(blackboard),
                    new CompleteProduceAction(blackboard)
                ))
        );
        
        _workTree = new BehaviorTree(workSequence);
    }

    public override void Enter()
    {
        Animator.SetTrigger(_work);
        _workTree.Reset();
    }

    public override void Execute()
    {
        _workTree.Tick();
    }

    public override void Exit()
    {
        Animator.speed = 1;
        Animator.SetBool(_isWalk, false);
        Animator.SetBool(_isWork, false);
        Animator.SetInteger(_randomIndex, 2);
    }
}
