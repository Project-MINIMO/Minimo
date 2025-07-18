using UnityEngine;

public class ObjectInput : MonoBehaviour
{
    private Camera _mainCamera;
    
    private InputManager _input;
    private EditManager _editManager;

    private InteractObject _currentObject;
    
    private int _layerMask;

    private void Start()
    {
        _mainCamera = Camera.main;
        
        _input = App.GetManager<InputManager>();
        _editManager = App.GetManager<EditManager>();
        
        _layerMask = LayerMask.GetMask("InteractObject");
    }

    private void Update()
    {
        switch (_input.CurrentState)
        {
            case InputState.ClickDown:
                HandleClickDown();
                break;
            
            case InputState.ClickUp:
                HandleClickUp();
                break;
            
            case InputState.LongPress:
                HandleLongPress();
                break;
            
            case InputState.Drag:
                HandleDrag();
                break;
        }
    }

    private void HandleClickDown()
    {
        var hit = GetRaycastObject();
        _currentObject = hit;
    }

    private void HandleClickUp()
    {
        if (_editManager.IsEditing.Value)
        {
            if (_currentObject is not BuildingObject)
            {
                var screenPosition = Input.mousePosition;
                var worldPosition = _mainCamera.ScreenToWorldPoint(
                    new Vector3(screenPosition.x, screenPosition.y, _mainCamera.nearClipPlane));
                worldPosition.z = 0;
                _editManager.MoveObject(worldPosition);
                
                _currentObject = null;
                return;
            }
        }

        if (_currentObject == null) return;
        
        _currentObject.OnClickUp();
        _currentObject = null;
    }

    private void HandleLongPress()
    {
        if (_currentObject == null) return;
        
        _currentObject.OnLongPress();
        _currentObject = null;
    }
    
    private void HandleDrag()
    {
        _currentObject = null;
    }

    private InteractObject GetRaycastObject()
    {
        var worldPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.OverlapPoint(worldPosition, _layerMask);
        if (hit != null && hit.TryGetComponent<InteractObject>(out var component))
        {
            return component;
        }
        
        return null;
    }
}
