using UnityEngine;

public class AStarNode
{
    public Vector3Int Position { get; set; }
    public AStarNode Parent { get; set; }
    public float G { get; set; } // Cost from start to current node
    public float H { get; set; } // Heuristic cost from current to target
    public float F => G + H;     // Total cost

    public AStarNode(Vector3Int position, AStarNode parent, float g, float h)
    {
        Position = position;
        Parent = parent;
        G = g;
        H = h;
    }
}