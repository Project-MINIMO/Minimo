using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedScreenSizeCanvas : MonoBehaviour
{
    private Vector3 _initialScaleVec;
    private Vector3 _initialLocalPos;
    
    private float _baseOrtho;
    private float _prevOrtho;

    private const float UpwardMovePerZoom = -0.5f;
    private const float ScaleCompensation = 0.5f;

    private void Awake()
    {
        _initialScaleVec = transform.localScale;
        _initialLocalPos = transform.localPosition;
        
        _baseOrtho = Camera.main.orthographicSize;
        _prevOrtho = Camera.main.orthographicSize;
        
        Apply(_prevOrtho);
    }

    private void LateUpdate()
    {
        var o = Camera.main.orthographicSize;
        if (Mathf.Approximately(o, _prevOrtho)) return;
        _prevOrtho = o;
        Apply(o);
    }

    private void Apply(float currentOrtho)
    {
        var raw  = currentOrtho / _baseOrtho;
        var factor = Mathf.Pow(raw, ScaleCompensation);
        transform.localScale = _initialScaleVec * factor;

        var zoomDelta = currentOrtho - _baseOrtho;
        transform.localPosition = _initialLocalPos + new Vector3(0f, zoomDelta * UpwardMovePerZoom, 0f);
    }
}
