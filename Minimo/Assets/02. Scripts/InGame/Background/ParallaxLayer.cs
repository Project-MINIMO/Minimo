using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Tooltip("카메라 이동량에 곱해질 계수 (0에 가까울수록 더 멀리 있는 효과)")]
    [Range(0f, 1f)]
    [SerializeField] private float _parallaxFactor = 0.5f;

    private Transform _camera;
    private Vector3 _previousCamPos;

    private void Start()
    {
        _camera = Camera.main.transform;
        _previousCamPos = _camera.position;
    }

    private void LateUpdate()
    {
        var deltaCam = _camera.position - _previousCamPos;
        transform.position += new Vector3(deltaCam.x * _parallaxFactor, deltaCam.y * _parallaxFactor, 0);
        _previousCamPos = _camera.position;
    }
}