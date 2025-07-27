using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFloater : MonoBehaviour
{
    [SerializeField] private float _floatSpeed = 1f;
    [SerializeField] private float _floatHeight = 0.5f;
    [SerializeField] private float _floatWidth = 0.1f;
    [SerializeField] private float _widthHeightOffset = 0f;

    private Transform _transform;
    private float _randomOffset;
    
    private void Start()
    {
        // Cache the transform for performance
        _transform = transform;
        // Add a random offset to the floating effect
        _randomOffset = Random.Range(0f, Mathf.PI * 2f);
    }
    
    private void Update()
    {
        // Calculate the new position based on sine wave for floating effect
        float newY = Mathf.Sin(Time.time * _floatSpeed + _randomOffset) * _floatHeight;
        float newX = Mathf.Cos(Time.time * _floatSpeed + _randomOffset + _widthHeightOffset) * _floatWidth;
        
        // Apply the new position to the object
        var localPosition = _transform.localPosition;
        localPosition = new Vector3(newX, newY, localPosition.z);
        _transform.localPosition = localPosition;
    }
}
