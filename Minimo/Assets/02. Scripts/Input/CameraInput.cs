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
    [SerializeField] private Vector2 _minBounds; 
    [SerializeField] private Vector2 _maxBounds; 
    
    private InputManager _input;
    private Camera _mainCamera;
    
    private bool _isActive;
    private EditCirclePanel _editCirclePanel;
    
    private void Start()
    {
        _input = App.GetManager<InputManager>();
        var screenStateManager = App.GetManager<ScreenStateManager>();
        screenStateManager.CurrentState.Subscribe((currentState) =>
        {
            _isActive = currentState is ScreenState.Sky;
        }).AddTo(gameObject);
        _mainCamera = Camera.main;
        
        _editCirclePanel = App.GetManager<UIManager>().GetPanel<EditCirclePanel>();
    }
    
    private void Update()
    {
        if (!_isActive) return;
        
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
        var delta = new Vector3(-Input.GetAxis("Mouse X") * _dragSpeed, -Input.GetAxis("Mouse Y") * _dragSpeed, 0);
        _mainCamera.transform.Translate(delta * Time.deltaTime, Space.World);
        
        ClampCameraPosition();
        _editCirclePanel.SetPosition();
    }

    private void Zoom()
    {
        if (Input.touchCount == 2) // Touch
        {
            var touch1 = Input.GetTouch(0);
            var touch2 = Input.GetTouch(1);
            
            var prevPos0 = touch1.position - touch1.deltaPosition;
            var prevPos1 = touch2.position - touch2.deltaPosition;
            
            var prevDist = Vector2.Distance(prevPos0, prevPos1);
            var currDist = Vector2.Distance(touch1.position, touch2.position);

            var delta = currDist - prevDist;
            var newSize = _mainCamera.orthographicSize - delta * _zoomSpeed * Time.deltaTime;
            _mainCamera.orthographicSize = Mathf.Clamp(newSize, _minZoom, _maxZoom);

            ClampCameraPosition();
            _editCirclePanel.SetPosition();
            return; 
        }
        
        var scroll = Input.GetAxis("Mouse ScrollWheel"); // Mouse
        if (scroll != 0.0f)
        {
            _mainCamera.orthographicSize = Mathf.Clamp(_mainCamera.orthographicSize - scroll * _zoomSpeed, _minZoom, _maxZoom);
            
            ClampCameraPosition();
            _editCirclePanel.SetPosition();
        }
    }
    
    private void ClampCameraPosition()
    {
        var cameraHeight = _mainCamera.orthographicSize;
        var cameraWidth = _mainCamera.orthographicSize * _mainCamera.aspect;
        
        if (_maxBounds.x - _minBounds.x < cameraWidth * 2 || _maxBounds.y - _minBounds.y < cameraHeight * 2)
        {
            Debug.LogWarning("Bounds 크기가 카메라 크기보다 작습니다. ClampCameraPosition을 건너뜁니다.");
            return;
        }
        
        var clampedX = Mathf.Clamp(_mainCamera.transform.position.x, _minBounds.x + cameraWidth, _maxBounds.x - cameraWidth);
        var clampedY = Mathf.Clamp(_mainCamera.transform.position.y, _minBounds.y + cameraHeight, _maxBounds.y - cameraHeight);

        _mainCamera.transform.position = new Vector3(clampedX, clampedY, _mainCamera.transform.position.z);
    }
}
