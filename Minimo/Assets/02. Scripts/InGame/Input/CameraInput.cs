using UniRx;
using UnityEngine;

public class CameraInput : MonoBehaviour
{
    [Header("Drag")]
    [SerializeField] private float _dragSpeed = 10f;

    [Header("Zoom")]
    [SerializeField] private float _zoomSpeed = 10f; 
    [SerializeField] private float _minZoom = 5f;  
    [SerializeField] private float _maxZoom = 20f; 
    
    [Header("Map Bounds")]
    [SerializeField] private BoxCollider2D _boundsCollider;
    
    private InputManager _input;
    private UIManager _ui;
    private Camera _mainCamera;
    
    private void Start()
    {
        _input = App.GetManager<InputManager>();
        _ui = App.GetManager<UIManager>();

        _mainCamera = Camera.main;
    }
    
    private void Update()
    {
        if (!_ui.IsOnlyDefaultPanelsInStack) return;
        
        if (_input.CurrentState == InputState.Drag)
        {
            Move();
        }
        
        else if (_input.CurrentState == InputState.Zoom)
        {
            Zoom();
        }
    }

    private void Move()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        var delta = new Vector3(-Input.GetAxis("Mouse X") * _dragSpeed, -Input.GetAxis("Mouse Y") * _dragSpeed, 0);
        _mainCamera.transform.Translate(delta * Time.deltaTime, Space.World);
        ClampCameraPosition();
#else
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            var touch = Input.GetTouch(0);
            var delta = touch.deltaPosition;

            // 터치 전후의 스크린 위치를 통해 카메라 기준으로 월드 이동 거리 계산
            Vector3 before = _mainCamera.ScreenToWorldPoint(touch.position - delta);
            Vector3 after  = _mainCamera.ScreenToWorldPoint(touch.position);
            Vector3 worldDelta = before - after;

            _mainCamera.transform.position += worldDelta;
            ClampCameraPosition();
        }
#endif
    }

    private void Zoom()
    {
        var bounds = _boundsCollider.bounds;
        var boundWidth  = bounds.size.x;
        var boundHeight  = bounds.size.y;
        var aspect  = _mainCamera.aspect;
        
        if (Input.touchCount == 2) // Touch
        {
            var t1 = Input.GetTouch(0);
            var t2 = Input.GetTouch(1);
            var prevDist = Vector2.Distance(t1.position - t1.deltaPosition, t2.position - t2.deltaPosition);
            var currDist = Vector2.Distance(t1.position, t2.position);
            var delta = currDist - prevDist;

            var newSizePinch = _mainCamera.orthographicSize - delta * _zoomSpeed * Time.deltaTime;
            newSizePinch = Mathf.Clamp(newSizePinch, _minZoom, _maxZoom);
            
            if (delta < 0f)
            {
                var halfWNew = newSizePinch * aspect;
                var halfHNew = newSizePinch;
                if (halfWNew * 2f > boundWidth || halfHNew * 2f > boundHeight) return;
            }
            
            _mainCamera.orthographicSize = newSizePinch;
            ClampCameraPosition();
            return;
        }
        
        var scroll = Input.GetAxis("Mouse ScrollWheel"); // Mouse
        if (scroll != 0.0f)
        {
            var newSizeWheel = _mainCamera.orthographicSize - scroll * _zoomSpeed;
            newSizeWheel  = Mathf.Clamp(newSizeWheel, _minZoom, _maxZoom);
            
            if (scroll < 0f)
            {
                float halfWNew = newSizeWheel * aspect;
                float halfHNew = newSizeWheel;
                if (halfWNew * 2f > boundWidth || halfHNew * 2f > boundHeight)
                    return;
            }
          
            _mainCamera.orthographicSize = newSizeWheel;
            ClampCameraPosition();
        }
    }
    
    private void ClampCameraPosition()
    {
        if (_boundsCollider == null)
        {
            Debug.LogWarning("Bounds Collider가 할당되지 않았습니다.");
            return;
        }
        
        var cameraHalfHeight = _mainCamera.orthographicSize;
        var cameraHalfWidth  = cameraHalfHeight * _mainCamera.aspect;
        
        var bounds = _boundsCollider.bounds;
        var min = bounds.min;
        var max = bounds.max;
        
        var clampedX = Mathf.Clamp(
            _mainCamera.transform.position.x,
            min.x + cameraHalfWidth,
            max.x - cameraHalfWidth);

        var clampedY = Mathf.Clamp(
            _mainCamera.transform.position.y,
            min.y + cameraHalfHeight,
            max.y - cameraHalfHeight);

        _mainCamera.transform.position = new Vector3(
            clampedX, clampedY,
            _mainCamera.transform.position.z);
    }
}
