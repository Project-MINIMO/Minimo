using UnityEngine;

public class ObjectInput : MonoBehaviour
{
    private Camera _mainCamera;
    
    private InputManager _input;
    private EditManager _editManager;

    private InteractObject _currentObject;
    
    private void Start()
    {
        _mainCamera = Camera.main;
        
        _input = App.GetManager<InputManager>();
        _editManager = App.GetManager<EditManager>();
    }

    private void Update()
    {
        if (_input.InputTarget != InputTargetType.Object) return;
        
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
            
            case InputState.DragEnd:
                HandleDragEnd();
                break;
        }
    }

    private void HandleClickDown()
    {
        var hit = _input.CurrentInteractObject;
        if (hit == null) return;

        _currentObject = hit;
        _currentObject.OnClickDown();
    }

    private void HandleClickUp()
    {
        if (_currentObject == null) return;
        if (_editManager.IsTileEditing.Value) return;
        
        if (_editManager.IsBuildingEditing.Value)
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
        if (_currentObject == null) return;
        
        _currentObject.OnDrag();
    }
    
    private void HandleDragEnd()
    {
        if (_currentObject == null) return;
        
        _currentObject.OnDragEnd();
        _currentObject = null;
    }
}
