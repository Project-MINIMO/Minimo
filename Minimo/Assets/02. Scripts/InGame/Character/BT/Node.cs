using System.Collections.Generic;

using UnityEngine;

public enum NodeStatus
{
    Success,
    Failure,
    Running
}

/// <summary>
/// Shared context containing references to the agent and game state.
/// </summary>
public class Blackboard
{
    public readonly MinimoObject Agent;
    public readonly Animator Animator;
    public List<Vector3Int> Path;
    
    public ProduceAdvanced Building => Agent.Data.AssignedBuilding;

    public Blackboard(MinimoObject agent)
    {
        Agent = agent;
        Animator = agent.GetComponent<Animator>();
    }
    
    // TODO: add other shared data (e.g., target positions, timers)
}

public abstract class Node
{
    protected readonly Blackboard Blackboard;

    protected Node(Blackboard blackboard)
    {
        Blackboard = blackboard;
    }

    /// <summary>
    /// Executes the node logic and returns its status.
    /// </summary>
    public abstract NodeStatus Tick();
    public abstract void Reset();
}

public abstract class ActionNode : Node
{
    protected ActionNode(Blackboard blackboard) : base(blackboard) { }
    public override void Reset() { }
}

public abstract class ConditionNode : Node
{
    protected ConditionNode(Blackboard blackboard) : base(blackboard) { }
    public override void Reset() { }
}