public class BehaviorTree
{
    private readonly Node _root;

    public BehaviorTree(Node rootNode)
    {
        _root = rootNode;
    }

    /// <summary>
    /// Call this every frame or tick interval.
    /// </summary>
    public void Tick()
    {
        _root.Tick();
    }

    public void Reset()
    {
        _root.Reset();
    }
}
