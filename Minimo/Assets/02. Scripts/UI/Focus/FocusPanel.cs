using System;
using DG.Tweening;
using UnityEngine;

public class FocusPanel : UIBase
{
    private BoxCollider2D _boundsCollider;
    private Camera _mainCamera;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        
        _boundsCollider = GameObject.FindWithTag("MapBounds").GetComponent<BoxCollider2D>();
        _mainCamera = Camera.main;
    }

    public void FocusOn(Vector3 worldPosition, 
        float targetZoom = -1, 
        float duration = 0.3f, 
        Action onComplete = null,
        bool closeOnComplete = true)
    {
        OpenPanel();

        if (Mathf.Approximately(targetZoom, -1))
        {
            targetZoom = _mainCamera.orthographicSize;
        } 
        
        var clampedPos = GetClampedCameraPosition(worldPosition, targetZoom);
        _mainCamera.transform.DOMove(clampedPos, duration).SetEase(Ease.InOutSine);
        
        _mainCamera.DOOrthoSize(targetZoom, duration).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                if (closeOnComplete) ClosePanel();
                onComplete?.Invoke();
            });
    }
    
    private Vector3 GetClampedCameraPosition(Vector3 target, float targetZoom)
    {
        var cameraHalfHeight = targetZoom;
        var cameraHalfWidth  = targetZoom * _mainCamera.aspect;

        var bounds = _boundsCollider.bounds;
        var min = bounds.min;
        var max = bounds.max;

        var clampedX = Mathf.Clamp(target.x, min.x + cameraHalfWidth, max.x - cameraHalfWidth);
        var clampedY = Mathf.Clamp(target.y, min.y + cameraHalfHeight, max.y - cameraHalfHeight);

        return new Vector3(clampedX, clampedY, _mainCamera.transform.position.z);
    }
}
