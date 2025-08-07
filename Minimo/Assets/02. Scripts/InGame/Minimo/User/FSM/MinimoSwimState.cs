using UnityEngine;

public class MinimoSwimState : State<MinimoObject>
{
    private readonly int _isSwim = Animator.StringToHash("IsSwim");
    private readonly int _isSwimIdle = Animator.StringToHash("IsSwimIdle");
    
    private readonly BehaviorTree _swimTree;
    private readonly Blackboard _blackboard;
    private readonly CameraBoundsUpdater _mapBounds;

    public MinimoSwimState(MinimoObject owner) : base(owner)
    {
        _mapBounds = GameObject.FindWithTag("MapBounds").GetComponent<CameraBoundsUpdater>();
        _blackboard = new Blackboard(owner.gameObject);
        
        var swimSequence = new SelectorNode
        (
            _blackboard,
            new SequenceNode
            (
                _blackboard,
                new IsClicked(_blackboard),
                new SwimIdleAction(_blackboard)
            ),
            new SequenceNode
            (
                _blackboard,
                new MoveForwardAction(_blackboard, _isSwim)
            )
        );
        
        _swimTree = new BehaviorTree(swimSequence);
    }

    public override void Enter()
    {
        _swimTree.Reset();
        
        var corners = _mapBounds.GetCorners();
        var randomNum = Random.Range(0, corners.Length);
        var endPoint = _mapBounds.GetCornerEndPoint(randomNum);
        
        _blackboard.Agent.transform.position = corners[randomNum];
        _blackboard.TargetPosition = endPoint;
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
