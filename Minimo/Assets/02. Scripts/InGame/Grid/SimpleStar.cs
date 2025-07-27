using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleStar : MonoBehaviour
{
    [Header("Scale Settings")]
    public float scaleSpeed = 2f;
    public float minScale = 0.9f;
    public float maxScale = 1.1f;
    public float standardScale = 0.1f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 30f;

    private float _timeOffset;

    void Start()
    {
        // 랜덤 오프셋으로 여러 별이 비동기적으로 반짝이게
        _timeOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        // 반짝임: 사인파로 scale 변화
        float scale = standardScale * Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.time * scaleSpeed + _timeOffset) + 1f) / 2f);
        transform.localScale = new Vector3(scale, scale, scale);

        // 회전
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}