using System.Collections.Generic;

public abstract class CompositeNode : Node
{
    protected readonly List<Node> Children = new();
    protected int CurrentIndex = 0;

    protected CompositeNode(Blackboard blackboard, params Node[] nodes) : base(blackboard)
    {
        Children.AddRange(nodes);
    }

    public void AddChild(Node node)
    {
        Children.Add(node);
    }

    public override void Reset()
    {
        CurrentIndex = 0;
        foreach (var child in Children)
        {
            child.Reset();
        }
    }
}

public class SequenceNode : CompositeNode
{
    public SequenceNode(Blackboard blackboard, params Node[] nodes) : base(blackboard, nodes) { }

    public override NodeStatus Tick()
    {
        while (CurrentIndex < Children.Count)
        {
            var status = Children[CurrentIndex].Tick();
            switch (status)
            {
                case NodeStatus.Running:
                    return NodeStatus.Running;
                
                case NodeStatus.Failure:
                    CurrentIndex = 0;
                    return NodeStatus.Failure;
                
                case NodeStatus.Success:
                    CurrentIndex++;
                    break;
            }
        }

        CurrentIndex = 0;
        return NodeStatus.Success;
    }
}

public class SelectorNode : CompositeNode
{
    public SelectorNode(Blackboard blackboard, params Node[] nodes) : base(blackboard, nodes) { }

    public override NodeStatus Tick()
    {
        while (CurrentIndex < Children.Count)
        {
            var status = Children[CurrentIndex].Tick();
            switch (status)
            {
                case NodeStatus.Running:
                    return NodeStatus.Running;
                
                case NodeStatus.Success:
                    CurrentIndex = 0;
                    return NodeStatus.Success;
                
                case NodeStatus.Failure:
                    CurrentIndex++;
                    break;
            }
        }

        CurrentIndex = 0;
        return NodeStatus.Failure;
    }
}