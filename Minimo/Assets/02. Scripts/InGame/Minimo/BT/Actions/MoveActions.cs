using UnityEngine;

public class MoveAction : ActionNode
{
    private readonly int _isWalk = Animator.StringToHash("IsWalk");
    private const string TopRight = "Walk_TR";
    private const string TopLeft = "Walk_TL";
    private const string BottomRight = "Walk_BR";
    private const string BottomLeft = "Walk_BL";
    private const string Default = "Default";
    
    private readonly PathManager _pathManager;
    private const float Speed = 0.3f;
    private Vector3 _targetPosition;
    private Vector3 _prevPosition;
    private int _currentIndex;
    private bool _shouldReset = true;
    private MinimoObject _minimo;
    
    private int _lastXSign = 1;
    
    public MoveAction(Blackboard blackboard) : base(blackboard)
    {
        _pathManager = App.GetManager<PathManager>();
        if (blackboard.Agent.TryGetComponent<MinimoObject>(out var component))
        {
            _minimo = component;
        }
    }

    public override void Reset()
    {
        _shouldReset = true;
    }

    public override NodeStatus Tick()
    {
        if (_shouldReset)
        {
            _lastXSign = 1;
            _shouldReset = false;
            _currentIndex = 0;
            _targetPosition = _pathManager.GetTileWorldPosition(Blackboard.Path[_currentIndex]);
            _prevPosition = Blackboard.Agent.transform.position;
            Blackboard.Animator.speed = Blackboard.Speed;
            Blackboard.Animator.SetBool(_isWalk, true);
        }

        if (Blackboard.AnimatorIsPlaying("LayToStand")) return NodeStatus.Running;
        if (Blackboard.AnimatorIsPlaying("SitToStand")) return NodeStatus.Running;
        if (Blackboard.AnimatorIsPlaying("Boast01")) return NodeStatus.Running;
        if (Blackboard.AnimatorIsPlaying("Boast02")) return NodeStatus.Running;

        float speed = Blackboard.Speed;
        if (_minimo != null)
        {
            var multiplier = _minimo.Energy switch
            {
                >= 75 => 1f,
                >= 50 => 0.8f,
                >= 25 => 0.6f,
                > 0   => 0.4f,
                _     => 0f
            };
            speed *= multiplier;
        }
        if (_currentIndex < Blackboard.Path.Count)
        {
            if ((_targetPosition - Blackboard.Agent.transform.position).sqrMagnitude > 0f)
            {
                SetAnimationDirection();
                
                Blackboard.Agent.transform.position = Vector3.MoveTowards(
                    Blackboard.Agent.transform.position, 
                    _targetPosition, 
                    speed * Speed * Time.deltaTime);

                if (_prevPosition == Blackboard.Agent.transform.position)
                {
                    Exit();
                    return NodeStatus.Failure;
                }
                _prevPosition = Blackboard.Agent.transform.position;
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
            Exit();
            return NodeStatus.Success;
        }
    }

    private void Exit()
    {
        _shouldReset = true;
        Blackboard.Speed = 1;
        Blackboard.Animator.speed = 1;
        Blackboard.Animator.SetBool(_isWalk, false);
        Blackboard.TargetBuilding = null;
    }
    
    private void SetAnimationDirection()
    {
        var deltaX = _targetPosition.x - Blackboard.Agent.transform.position.x;
        var deltaY = _targetPosition.y - Blackboard.Agent.transform.position.y;

        if (deltaX > 0f) _lastXSign = 1;
        else if (deltaX < 0f) _lastXSign = -1;
        
        var trigger = (deltaX, deltaY) switch
        {
            (> 0, > 0) => TopRight,
            (> 0, < 0) => BottomRight,
            (> 0, 0) => BottomRight,

            (< 0, > 0) => TopLeft,
            (< 0, < 0) => BottomLeft,
            (< 0, 0) => BottomLeft,

            (0, > 0) => _lastXSign > 0 ? TopRight  : TopLeft,    // ↑
            (0, < 0) => _lastXSign > 0 ? BottomRight : BottomLeft, // ↓
            
            (0, 0) => TopRight,
            _ => TopRight
        };
                
        Blackboard.Animator.SetTrigger(trigger);
    }
}

public class MoveForwardAction : ActionNode
{
    private const string TopRight = "Walk_TR";
    private const string TopLeft = "Walk_TL";
    private const string BottomRight = "Walk_BR";
    private const string BottomLeft = "Walk_BL";
    private const string Default = "Default";

    private readonly int _triggerName;
    private const float Speed = 0.3f;
    private Vector3 _targetPosition;
    private Vector3 _prevPosition;
    private bool _shouldReset = true;

    private readonly StrayMinimoObject _strayMinimo;
    private readonly MinimoObject _userMinimo;
    
    private int _lastXSign = 1;

    public MoveForwardAction(Blackboard blackboard, int triggerName) : base(blackboard)
    {
        _triggerName = triggerName;
        if (blackboard.Agent.TryGetComponent<StrayMinimoObject>(out var stray))
        {
            _strayMinimo = stray;
        }
        if (blackboard.Agent.TryGetComponent<MinimoObject>(out var user))
        {
            _userMinimo = user;
        }
    }

    public override void Reset()
    {
        _shouldReset = true;
    }

    public override NodeStatus Tick()
    {
        if (_shouldReset)
        {
            _lastXSign = 1;
            _shouldReset = false;
            _targetPosition = Blackboard.TargetPosition;
            _prevPosition = Blackboard.Agent.transform.position;
            Blackboard.Animator.speed = Blackboard.Speed;
            Blackboard.Animator.SetBool(_triggerName, true);
            
            SetAnimationDirection();
        }

        if (_strayMinimo != null)
        {
            if (_strayMinimo.IsClicked) return NodeStatus.Running;
            else if (Blackboard.TargetBuilding != null && Blackboard.TargetBuilding.CurrentState != ProduceState.Complete)
            {
                Exit();
                return NodeStatus.Failure;
            }
        }
        if (_userMinimo != null && _userMinimo.IsClicked)
        {
            Exit();
            return NodeStatus.Failure;
        }
        
        if ((_targetPosition - Blackboard.Agent.transform.position).sqrMagnitude > 0f)
        {
            Blackboard.Agent.transform.position = Vector3.MoveTowards(
                Blackboard.Agent.transform.position, 
                _targetPosition, 
                Blackboard.Speed * Speed * Time.deltaTime);

            if (_prevPosition == Blackboard.Agent.transform.position)
            {
                Exit();
                return NodeStatus.Failure;
            }
            _prevPosition = Blackboard.Agent.transform.position;
            
            return NodeStatus.Running;
        }
        
        Exit();
        return NodeStatus.Success;
    }

    private void Exit()
    {
        _shouldReset = true;
        Blackboard.Speed = 1;
        Blackboard.Animator.speed = 1;
        Blackboard.Animator.SetBool(_triggerName, false);
    }

    private void SetAnimationDirection()
    {
        var deltaX = _targetPosition.x - Blackboard.Agent.transform.position.x;
        var deltaY = _targetPosition.y - Blackboard.Agent.transform.position.y;

        if (deltaX > 0f) _lastXSign = 1;
        else if (deltaX < 0f) _lastXSign = -1;
        
        var trigger = (deltaX, deltaY) switch
        {
            (> 0, > 0) => TopRight,
            (> 0, < 0) => BottomRight,
            (> 0, 0) => BottomRight,

            (< 0, > 0) => TopLeft,
            (< 0, < 0) => BottomLeft,
            (< 0, 0) => BottomLeft,
            
            (0, > 0) => _lastXSign > 0 ? TopRight  : TopLeft,    // ↑
            (0, < 0) => _lastXSign > 0 ? BottomRight : BottomLeft, // ↓
            
            _ => TopRight
        };
                
        Blackboard.Animator.SetTrigger(trigger);
    }
}

public class MoveOrderAction : ActionNode
{
    private readonly int _isWalk = Animator.StringToHash("IsWalk");
    private const string TopRight = "Walk_TR";
    private const string TopLeft = "Walk_TL";
    private const string BottomRight = "Walk_BR";
    private const string BottomLeft = "Walk_BL";
    private const string Default = "Default";
    
    private readonly PathManager _pathManager;
    private const float Speed = 0.3f;
    private Vector3 _targetPosition;
    private Vector3 _prevPosition;
    private int _currentIndex;
    private bool _shouldReset = true;
    private float _lastTime;

    private readonly MinimoObject _minimo;
    
    private int _lastXSign = 1;
    
    public MoveOrderAction(Blackboard blackboard) : base(blackboard)
    {
        _pathManager = App.GetManager<PathManager>();
        _minimo = blackboard.Agent.GetComponent<MinimoObject>();
    }

    public override void Reset()
    {
        _shouldReset = true;
    }

    public override NodeStatus Tick()
    {
        if (_shouldReset)
        {
            _lastXSign = 1;
            _shouldReset = false;
            _currentIndex = 0;
            _targetPosition = _pathManager.GetTileWorldPosition(Blackboard.Path[_currentIndex]);
            _prevPosition = Blackboard.Agent.transform.position;
            Blackboard.Animator.speed = Blackboard.Speed;
            Blackboard.Animator.SetBool(_isWalk, true);
            _lastTime = Time.time;
        }

        if (Blackboard.AnimatorIsPlaying("LayToStand")) return NodeStatus.Running;
        if (Blackboard.AnimatorIsPlaying("SitToStand")) return NodeStatus.Running;
        if (Blackboard.AnimatorIsPlaying("Boast01")) return NodeStatus.Running;
        if (Blackboard.AnimatorIsPlaying("Boast02")) return NodeStatus.Running;

        if (Time.time - _lastTime >= 1f) 
        {
            _lastTime = Time.time;
            _minimo.Energy -= 0.5f;
        }

        if (_minimo.Energy < 0)
        {
            Exit();
            return NodeStatus.Failure;
        }
        
        float speed = Blackboard.Speed;
        if (_minimo != null)
        {
            var multiplier = _minimo.Energy switch
            {
                >= 75 => 1f,
                >= 50 => 0.8f,
                >= 25 => 0.6f,
                > 0   => 0.4f,
                _     => 0f
            };
            speed *= multiplier;
        }
        
        if (_currentIndex < Blackboard.Path.Count)
        {
            if ((_targetPosition - Blackboard.Agent.transform.position).sqrMagnitude > 0f)
            {
                SetAnimationDirection();
                
                Blackboard.Agent.transform.position = Vector3.MoveTowards(
                    Blackboard.Agent.transform.position, 
                    _targetPosition, 
                    speed * Speed * Time.deltaTime);

                if (_prevPosition == Blackboard.Agent.transform.position)
                {
                    Exit();
                    return NodeStatus.Failure;
                }
                _prevPosition = Blackboard.Agent.transform.position;
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
            Exit();
            return NodeStatus.Success;
        }
    }

    private void Exit()
    {
        _shouldReset = true;
        Blackboard.Speed = 1;
        Blackboard.Animator.speed = 1;
        Blackboard.Animator.SetBool(_isWalk, false);
        Blackboard.TargetBuilding = null;
    }
    
    private void SetAnimationDirection()
    {
        var deltaX = _targetPosition.x - Blackboard.Agent.transform.position.x;
        var deltaY = _targetPosition.y - Blackboard.Agent.transform.position.y;

        if (deltaX > 0f) _lastXSign = 1;
        else if (deltaX < 0f) _lastXSign = -1;
        
        var trigger = (deltaX, deltaY) switch
        {
            (> 0, > 0) => TopRight,
            (> 0, < 0) => BottomRight,
            (> 0, 0) => BottomRight,

            (< 0, > 0) => TopLeft,
            (< 0, < 0) => BottomLeft,
            (< 0, 0) => BottomLeft,

            (0, > 0) => _lastXSign > 0 ? TopRight  : TopLeft,    // ↑
            (0, < 0) => _lastXSign > 0 ? BottomRight : BottomLeft, // ↓
            
            (0, 0) => TopRight,
            _ => TopRight
        };
                
        Blackboard.Animator.SetTrigger(trigger);
    }
}