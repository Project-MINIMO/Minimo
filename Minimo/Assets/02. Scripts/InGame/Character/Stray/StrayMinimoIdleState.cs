using UnityEngine;
using DG.Tweening;

public class StrayMinimoIdleState : State<StrayMinimoObject>
{
    private static readonly int IsWalk = Animator.StringToHash("IsWalk");
    
    private readonly BehaviorTree _idleTree;
    private readonly Vector3 _scale = new(0.4f, 0.4f, 0.4f);
    private readonly Quaternion _startRotation = new(0f, 0f, 0f, 0f);
    private readonly Vector3 _endRotation = new(0f, 0f, 360f);

    public StrayMinimoIdleState(StrayMinimoObject owner) : base(owner)
    {
        var blackboard = new Blackboard(owner.gameObject);
        
        var idleSequence = new SequenceNode
        (
            blackboard,
            new FindStrayPositionAction(blackboard),
            new MoveForwardAction(blackboard)
        );
        
        _idleTree = new BehaviorTree(idleSequence);
    }

    public override void Enter()
    {
        Owner.transform.DOKill();
        Owner.transform.DOScale(_scale, 0.1f);
        Owner.transform.DORotate(_endRotation, 0.1f);
        
        _idleTree.Reset();
    }

    public override void Execute()
    {
        _idleTree.Tick();
    }

    public override void Exit()
    {
        Owner.transform.DOKill();
        Owner.transform.localScale = _scale;
        Owner.transform.rotation = _startRotation;
        
        Animator.speed = 1;
        Animator.SetBool(IsWalk, false);
    }
}
