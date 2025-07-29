using System.Collections.Generic;

public abstract class CompositeNode : Node
{
    protected readonly List<Node> Children = new();

    protected CompositeNode(Blackboard blackboard, params Node[] nodes) : base(blackboard)
    {
        Children.AddRange(nodes);
    }

    public void AddChild(Node node)
    {
        Children.Add(node);
    }
}

public class Sequence : CompositeNode
{
    private int _currentIndex = 0;

    protected Sequence(Blackboard blackboard, params Node[] nodes) : base(blackboard) { }

    public override NodeStatus Tick()
    {
        while (_currentIndex < Children.Count)
        {
            var status = Children[_currentIndex].Tick();
            switch (status)
            {
                case NodeStatus.Running:
                    return NodeStatus.Running;
                
                case NodeStatus.Failure:
                    _currentIndex = 0;
                    return NodeStatus.Failure;
                
                case NodeStatus.Success:
                    _currentIndex++;
                    break;
            }
        }

        _currentIndex = 0;
        return NodeStatus.Success;
    }
}

public class Selector : CompositeNode
{
    private int _currentIndex = 0;

    protected Selector(Blackboard blackboard, params Node[] nodes) : base(blackboard) { }

    public override NodeStatus Tick()
    {
        while (_currentIndex < Children.Count)
        {
            var status = Children[_currentIndex].Tick();
            switch (status)
            {
                case NodeStatus.Running:
                    return NodeStatus.Running;
                
                case NodeStatus.Success:
                    _currentIndex = 0;
                    return NodeStatus.Success;
                
                case NodeStatus.Failure:
                    _currentIndex++;
                    break;
            }
        }

        _currentIndex = 0;
        return NodeStatus.Failure;
    }
}