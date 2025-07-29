using System.Collections.Generic;

using UnityEngine;

public class MinimoWalkState : State<MinimoObject>
{
    private const float SPEED = 0.3f;

    private bool _isUpdating = false;
    private readonly PathManager _pathManager;
    private List<Vector3Int> _path;
    private Vector3 _targetPosition;

    private int _currIndex;

    public MinimoWalkState(MinimoObject owner) : base(owner) 
    {
        _pathManager = App.GetManager<PathManager>();
    }

    public override void Enter()
    {
        _path = _pathManager.GetRandomPath(_owner.transform.position);

        if (_path != null && _path.Count > 0)
        {
            _owner.SetAnimation("IsWalk", true);
            
            _currIndex = 0;
            _targetPosition = _pathManager.GetTileWorldPosition(_path[_currIndex]);

            _isUpdating = true;
        }
        else
        {
            _owner.FSM.ChangeState(MinimoState.Idle);
        }
    }

    public override void Execute()
    {
        if (!_isUpdating)
        {
            return;
        }

        if (_currIndex < _path.Count)
        {
            if ((_targetPosition - _owner.transform.position).sqrMagnitude > 0.05f)
            {
                var deltaX = _targetPosition.x - _owner.transform.position.x;
                var deltaY = _targetPosition.y - _owner.transform.position.y;
                
                if (deltaX > 0 && deltaY > 0)
                {
                    _owner.SetAnimation("TR");
                }
                else if (deltaX < 0 && deltaY > 0)
                {
                    _owner.SetAnimation("TL");
                }
                else if (deltaX < 0 && deltaY < 0)
                {
                    _owner.SetAnimation("BL");
                }
                else if (deltaX > 0 && deltaY < 0)
                {
                    _owner.SetAnimation("BR");
                }
                
                _owner.transform.position = Vector3.MoveTowards(_owner.transform.position, _targetPosition, SPEED * Time.deltaTime);
                
            }
            else
            {
                if (++_currIndex < _path.Count)
                {
                    _targetPosition = _pathManager.GetTileWorldPosition(_path[_currIndex]);
                }
            }
        }
        else
        {
            _isUpdating = false;
            _owner.SetChillState();
        }
    }

    public override void Exit()
    {
        _owner.SetAnimation("IsWalk", false);
        _isUpdating = false;
        _path = null;

        _currIndex = 0;
    }
}
