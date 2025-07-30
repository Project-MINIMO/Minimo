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
    public readonly GameObject Agent;
    public readonly Animator Animator;
    public List<Vector3Int> Path;

    public int RandomActionIndex;
    public int Speed = 1;
    public Vector3 TargetPosition;
    public ProduceAdvanced TargetBuilding;

    public Blackboard(GameObject agent)
    {
        Agent = agent;
        Animator = agent.GetComponentInChildren<Animator>();
    }

    public bool AnimatorIsPlaying(string stateName)
    {
        return Animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
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