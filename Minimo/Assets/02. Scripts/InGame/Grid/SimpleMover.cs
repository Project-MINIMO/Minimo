using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMover : MonoBehaviour
{
    [SerializeField] private Vector2 _moveDirection = Vector2.right;
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _tilingDistance = 5f;
    
    private void Update()
    {
        // Calculate the new position based on the move direction and speed
        Vector3 newPosition = transform.position + (Vector3)_moveDirection.normalized * _moveSpeed * Time.deltaTime;
        
        // Check if the object has moved beyond the tiling distance
        if (Vector2.Distance(transform.position, newPosition) >= _tilingDistance)
        {
            // Reset position to create a tiling effect
            newPosition = Vector3.zero;
        }
        
        // Apply the new position
        transform.position = newPosition;
    }
}
