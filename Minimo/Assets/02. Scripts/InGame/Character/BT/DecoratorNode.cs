public abstract class DecoratorNode : Node
{
    protected readonly Node Child;

    protected DecoratorNode(Blackboard blackboard, Node node) : base(blackboard)
    {
        Child = node;
    }
}

/// <summary>
/// Repeats the child until it fails; returns Success when child fails.
/// </summary>
public class RepeatUntilFail : DecoratorNode
{
    protected RepeatUntilFail(Blackboard blackboard, Node node) : base(blackboard, node) { }

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
public class RepeatWhileSuccess : DecoratorNode
{
    protected RepeatWhileSuccess(Blackboard blackboard, Node node) : base(blackboard, node) { }

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