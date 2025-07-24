using System.Collections.Generic;

using UnityEngine;

public class InfiniteScroller : MonoBehaviour
{
    [SerializeField] private Collider2D boundaryTrigger;
    [SerializeField] private Vector2 moveDirection = new(1f, -0.5f);
    [SerializeField] private float speed = 0.1f;

    private List<Transform> _tiles;
    private const float TileWidth = 7.18f;
    private const float TotalWidth = 28.72f;
    private const float TileHeight = 12.8f;
    private const float TotalHeight = 25.6f;

    private void Start()
    {
        _tiles = new List<Transform>();
        foreach (Transform child in transform)
        {
            _tiles.Add(child);
        }
    }

    private void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
        
        var boundaryX = boundaryTrigger.bounds.max.x;
        var boundaryY = boundaryTrigger.bounds.min.y;
        
        foreach (var tile in _tiles)
        {
            var spriteLeftEdge = tile.position.x - TileWidth * 0.5f;
            var spriteTopEdge = tile.position.y + TileHeight * 0.5f;
            
            if (spriteLeftEdge > boundaryX)
            {
                tile.Translate(Vector3.left * TotalWidth, Space.World);
            }
            
            if (spriteTopEdge < boundaryY)
            {
                tile.Translate(Vector3.up * TotalHeight, Space.World);
            }
        }
    }
}