public abstract class DecoratorNode : Node
{
    protected readonly Node Child;

    protected DecoratorNode(Blackboard blackboard, Node node) : base(blackboard)
    {
        Child = node;
    }
    
    public override void Reset()
    {
        Child.Reset();
    }
}

/// <summary>
/// Repeats the child until it fails; returns Success when child fails.
/// </summary>
public class RepeatUntilFailNode : DecoratorNode
{
    protected RepeatUntilFailNode(Blackboard blackboard, Node node) : base(blackboard, node) { }

    public override NodeStatus Tick()
    {
        var status = Child.Tick();
        return status switch
        {
            NodeStatus.Running => NodeStatus.Running,
            NodeStatus.Failure => NodeStatus.Success,
            _ => NodeStatus.Running
        };
    }
}

/// <summary>
/// Repeats the child while it succeeds; returns Failure when child fails.
/// </summary>
public class RepeatWhileSuccessNode : DecoratorNode
{
    protected RepeatWhileSuccessNode(Blackboard blackboard, Node node) : base(blackboard, node) { }

    public override NodeStatus Tick()
    {
        var status = Child.Tick();
        return status switch
        {
            NodeStatus.Running => NodeStatus.Running,
            NodeStatus.Failure => NodeStatus.Failure,
            _ => NodeStatus.Running
        };
    }
}