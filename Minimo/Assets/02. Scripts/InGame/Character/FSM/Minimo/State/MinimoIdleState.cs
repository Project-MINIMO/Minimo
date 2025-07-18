using UnityEngine;

public class MinimoIdleState : StateBase<Minimo>
{
    private float _stateTimer;
    private bool _isUpdating;

    private readonly string[] _idleAnimations = { "IsSit", "IsStand" };
    private string _currentAnimation;

    public MinimoIdleState(Minimo owner) : base(owner) { }

    public override void Enter()
    {
        _stateTimer = Random.Range(15f, 20f);
        _isUpdating = true;

        var randomIndex = Random.Range(0, _idleAnimations.Length);
        _currentAnimation = _idleAnimations[randomIndex];

        _owner.SetAnimation(_currentAnimation, true);
    }

    public override void Execute()
    {
        if (!_isUpdating)
        {
            return;
        }

        _stateTimer -= Time.deltaTime;

        if (_stateTimer <= 0)
        {
            _owner.SetChillState();
        }
    }

    public override void Exit()
    {
        _isUpdating = false;
        _owner.SetAnimation(_currentAnimation, false);
    }
}
