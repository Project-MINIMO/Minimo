using UnityEngine;

public class MoveAction : ActionNode
{
    private readonly int _isWalk = Animator.StringToHash("IsWalk");
    private const string TopRight = "Walk_TR";
    private const string TopLeft = "Walk_TL";
    private const string BottomRight = "Walk_BR";
    private const string BottomLeft = "Walk_BL";
    
    private readonly PathManager _pathManager;
    private Vector3 _targetPosition;
    private int _currentIndex;
    private float _speed;
    private bool _shouldReset = true;
    
    public MoveAction(Blackboard blackboard) : base(blackboard)
    {
        _pathManager = App.GetManager<PathManager>();
    }

    public override void Reset()
    {
        _shouldReset = true;
    }

    public override NodeStatus Tick()
    {
        if (_shouldReset)
        {
            _shouldReset = false;
            _currentIndex = 0;
            _targetPosition = _pathManager.GetTileWorldPosition(Blackboard.Path[_currentIndex]);
            _speed = Blackboard.Path.Count > 3 ? 0.6f : 0.3f;
            Blackboard.Animator.speed = Mathf.Approximately(_speed, 0.3f) ? 1 : 2;
            Blackboard.Animator.SetBool(_isWalk, true);
        }

        if (Blackboard.AnimatorIsPlaying("LayToStand")) return NodeStatus.Running;
        if (Blackboard.AnimatorIsPlaying("Boast01")) return NodeStatus.Running;
        if (Blackboard.TargetBuilding != null 
            && Blackboard.TargetBuilding.AssignedMinimo != null) return NodeStatus.Failure;
        
        if (_currentIndex < Blackboard.Path.Count)
        {
            if ((_targetPosition - Blackboard.Agent.transform.position).sqrMagnitude > 0f)
            {
                var deltaX = _targetPosition.x - Blackboard.Agent.transform.position.x;
                var deltaY = _targetPosition.y - Blackboard.Agent.transform.position.y;

                var trigger = (deltaX, deltaY) switch
                {
                    (> 0, > 0) => TopRight,
                    (> 0, < 0) => BottomRight,
                    (> 0, 0) => BottomRight,

                    (< 0, > 0) => TopLeft,
                    (< 0, < 0) => BottomLeft,
                    (< 0, 0) => BottomLeft,

                    (0, > 0) => // 수직 ↑
                        Blackboard.AnimatorIsPlaying(BottomLeft) 
                        || Blackboard.AnimatorIsPlaying(TopLeft) 
                            ? TopLeft : TopRight,

                    (0, < 0) => // 수직 ↓
                        Blackboard.AnimatorIsPlaying(BottomLeft) 
                        || Blackboard.AnimatorIsPlaying(TopLeft)
                            ? BottomLeft : BottomRight,

                    (0, 0) => TopRight,
                    _ => TopRight
                };
                
                Blackboard.Animator.SetTrigger(trigger);
                
                Blackboard.Agent.transform.position = Vector3.MoveTowards(
                    Blackboard.Agent.transform.position, 
                    _targetPosition, 
                    _speed * Time.deltaTime);
            }
            else
            {
                if (++_currentIndex < Blackboard.Path.Count)
                {
                    _targetPosition = _pathManager.GetTileWorldPosition(Blackboard.Path[_currentIndex]);
                }
            }
            
            return NodeStatus.Running;
        }
        else
        {
            _shouldReset = true;
            Blackboard.Animator.speed = 1;
            Blackboard.Animator.SetBool(_isWalk, false);
            return NodeStatus.Success;
        }
    }
}