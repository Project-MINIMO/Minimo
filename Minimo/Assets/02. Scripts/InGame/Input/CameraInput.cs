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
    private Camera _mainCamera;
    
    private void Start()
    {
        _input = App.GetManager<InputManager>();

        _mainCamera = Camera.main;
    }
    
    private void Update()
    {
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
        var boundWidth = bounds.size.x;
        var boundHeight = bounds.size.y;
        var aspect = _mainCamera.aspect;

#if UNITY_EDITOR || UNITY_STANDALONE
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            var newSize = _mainCamera.orthographicSize - scroll * _zoomSpeed;
            newSize = Mathf.Clamp(newSize, _minZoom, _maxZoom);

            if (scroll < 0f) // 확대 시 맵 범위 초과 방지
            {
                var halfWNew = newSize * aspect;
                var halfHNew = newSize;
                if (halfWNew * 2f > boundWidth || halfHNew * 2f > boundHeight)
                    return;
            }

            _mainCamera.orthographicSize = newSize;
            ClampCameraPosition();
        }
        
#else
        if (Input.touchCount < 2) return;

        var t1 = Input.GetTouch(Input.touchCount - 2);
        var t2 = Input.GetTouch(Input.touchCount - 1);

        var prevDist = Vector2.Distance(t1.position - t1.deltaPosition, t2.position - t2.deltaPosition);
        var currDist = Vector2.Distance(t1.position, t2.position);
        var zoomDelta = currDist - prevDist;

        var newSize = _mainCamera.orthographicSize - zoomDelta * _zoomSpeed * Time.deltaTime;
        newSize = Mathf.Clamp(newSize, _minZoom, _maxZoom);

        var zoomAtLimit = newSize == _minZoom || newSize == _maxZoom;

        if (zoomDelta < 0f) // 확대 시
        {
            var halfWNew = newSize * aspect;
            var halfHNew = newSize;
            if (halfWNew * 2f > boundWidth || halfHNew * 2f > boundHeight) return;
        }

        _mainCamera.orthographicSize = newSize;

        var moved1 = t1.phase == TouchPhase.Moved;
        var moved2 = t2.phase == TouchPhase.Moved;

        if (moved1 && moved2)
        {
            var avgDelta = (t1.deltaPosition + t2.deltaPosition) / 2f;
            ApplyTouchMove(t2.position, avgDelta);
        }
        else if ((moved1 ^ moved2) && zoomAtLimit)
        {
            var activePos = moved1 ? t1.position : t2.position;
            var delta = moved1 ? t1.deltaPosition : t2.deltaPosition;
            ApplyTouchMove(activePos, delta);
        }

        ClampCameraPosition();  
#endif
    }
    
    private void ApplyTouchMove(Vector2 screenPos, Vector2 delta)
    {
        var before = _mainCamera.ScreenToWorldPoint(screenPos - delta);
        var after  = _mainCamera.ScreenToWorldPoint(screenPos);
        var worldDelta = before - after;

        _mainCamera.transform.position += worldDelta;
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
