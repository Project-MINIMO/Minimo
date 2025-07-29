using UnityEngine;

public class MoveToBuildingAction : ActionNode
{
    public MoveToBuildingAction(Blackboard blackboard) : base(blackboard) { }
    
    public override NodeStatus Tick()
    {
        // TODO: implement movement towards building
        bool arrived = /* movement logic */ false;
        if (!arrived)
            return NodeStatus.Running;

        return NodeStatus.Success;
    }
}

public class PlaceMinimoAction : ActionNode
{
    public PlaceMinimoAction(Blackboard blackboard) : base(blackboard) { }

    public override NodeStatus Tick()
    {
        // TODO: implement placement logic
        return NodeStatus.Success;
    }
}

public class PlayProductionAnimationAction : ActionNode
{
    public PlayProductionAnimationAction(Blackboard blackboard) : base(blackboard) { }

    public override NodeStatus Tick()
    {
        // TODO: trigger production animation
        bool animationDone = /* check animation state */ true;
        return animationDone ? NodeStatus.Success : NodeStatus.Running;
    }
}

public class PlayCompletionAnimationAction : ActionNode
{
    public PlayCompletionAnimationAction(Blackboard blackboard) : base(blackboard) { }

    public override NodeStatus Tick()
    {
        // TODO: trigger completion animation
        bool animationDone = /* check animation state */ true;
        return animationDone ? NodeStatus.Success : NodeStatus.Running;
    }
}

public class PlayIdleAnimationAction : ActionNode
{
    public PlayIdleAnimationAction(Blackboard blackboard) : base(blackboard) { }

    public override NodeStatus Tick()
    {
        // TODO: trigger idle animation
        return NodeStatus.Success;
    }
}

public class WaitAction : ActionNode
{
    private readonly float duration;
    private float startTime;
    private bool started;

    public WaitAction(Blackboard blackboard, float waitTime) : base(blackboard) 
    {
        duration = waitTime;
    }

    public override NodeStatus Tick()
    {
        if (!started)
        {
            startTime = Time.time;
            started = true;
        }

        return (Time.time - startTime) >= duration ? NodeStatus.Success : NodeStatus.Running;
    }
}