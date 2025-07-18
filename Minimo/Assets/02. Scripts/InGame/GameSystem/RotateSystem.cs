using UnityEngine;

public class RotateSystem : MonoBehaviour
{
    [SerializeField] private Transform _constellation;
    [SerializeField] private float _totalDegrees = 360f;
    [SerializeField] private float _totalTimeInSeconds = 120f; 

    private float _rotationSpeed;

    private void Start()
    {
        _rotationSpeed = _totalDegrees / _totalTimeInSeconds;
    }

    private void Update()
    {
        _constellation.Rotate(0f, 0f, _rotationSpeed * Time.deltaTime);
    }
}
