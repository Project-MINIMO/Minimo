using System;

using UnityEngine;
using DG.Tweening;

public class AcquireFadeOutAction : ActionNode
{
    private float _duration;
    private float _startTime;
    private bool _shouldReset = true;

    public AcquireFadeOutAction(Blackboard blackboard) : base(blackboard) { }

    public override void Reset()
    {
        _shouldReset = true;
    }

    public override NodeStatus Tick()
    {
        if (_shouldReset)
        {
            _shouldReset = false;
            
            _duration = 1f;
            _startTime = Time.time;
            var spriteRender = Blackboard.Agent.GetComponentInChildren<SpriteRenderer>();
            spriteRender.transform.DORotate(new Vector3(0, 0, 360f), 0.2f, RotateMode.FastBeyond360).From();
            spriteRender.transform.DOScale(Vector3.zero, 0.2f);
            spriteRender.DOFade(0, 0.2f);
        }
        
        if (Time.time - _startTime >= _duration)
        {
            _shouldReset = true;
            return NodeStatus.Success;
        }
        
        return NodeStatus.Running;
    }
}

public class AcquireShakeAction : ActionNode
{
    private Transform _spaceship;
    private float _duration;
    private float _startTime;
    private bool _shouldReset = true;
    private readonly FocusPanel _focusPanel;

    public AcquireShakeAction(Blackboard blackboard) : base(blackboard)
    {
        _spaceship = GameObject.FindWithTag("Spaceship").transform;
        _focusPanel = App.GetManager<UIManager>().GetPanel<FocusPanel>();
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
            
            _duration = 1.3f;
            _startTime = Time.time;
            
            _focusPanel.FocusOn(new Vector3(0, 0, Camera.main.transform.position.z),
                3, 0.3f, () =>
                {
                    _spaceship.DOShakePosition(
                        duration: 1f,
                        strength: new Vector3(0.05f, 0f, 0f), // X축만 흔들림
                        vibrato: 10,
                        randomness: 90,
                        fadeOut: true);
                });
        }
        
        if (Time.time - _startTime >= _duration)
        {
            _shouldReset = true;
            return NodeStatus.Success;
        }
        
        return NodeStatus.Running;
    }
}

public class AcquireFadeInAction : ActionNode
{
    private float _duration;
    private float _startTime;
    private bool _shouldReset = true;

    public AcquireFadeInAction(Blackboard blackboard) : base(blackboard) { }

    public override void Reset()
    {
        _shouldReset = true;
    }

    public override NodeStatus Tick()
    {
        if (_shouldReset)
        {
            _shouldReset = false;
            
            _duration = 1f;
            _startTime = Time.time;
            Blackboard.Agent.transform.position = new Vector3(0, -0.75f, 0);
            var spriteRender = Blackboard.Agent.GetComponentInChildren<SpriteRenderer>();
            spriteRender.transform.DORotate(new Vector3(0, 0, 360f), 0.2f, RotateMode.FastBeyond360).From();
            spriteRender.transform.DOScale(Vector3.one, 0.2f);
            spriteRender.DOFade(1, 0.2f);
        }
        
        if (Time.time - _startTime >= _duration)
        {
            _shouldReset = true;
            return NodeStatus.Success;
        }
        
        return NodeStatus.Running;
    }
}