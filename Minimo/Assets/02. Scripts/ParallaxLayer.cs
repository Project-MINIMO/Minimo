using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Tooltip("카메라 이동량에 곱해질 계수 (0에 가까울수록 더 멀리 있는 효과)")]
    [Range(0f, 1f)]
    [SerializeField] private float parallaxFactor = 0.5f;

    private Transform cam;
    private Vector3 previousCamPos;
    private Vector3 startPos;

    void Start()
    {
        cam = Camera.main.transform;
        previousCamPos = cam.position;
        startPos = transform.position;
    }

    void LateUpdate()
    {
        Vector3 deltaCam = cam.position - previousCamPos;
        // 레이어 이동: 카메라 이동량의 parallaxFactor만큼
        transform.position += new Vector3(deltaCam.x * parallaxFactor, deltaCam.y * parallaxFactor, 0);
        previousCamPos = cam.position;
    }
}