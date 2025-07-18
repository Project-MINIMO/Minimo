using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimoInput : MonoBehaviour
{
    private Camera _mainCamera;
    
    private InputManager _input;
    private CameraInput _cameraInput;

    private Minimo _currentObject;
    
    private int _layerMask;
    private int _layerMask2;

    private bool _isClicked = false;

    private void Start()
    {
        _mainCamera = Camera.main;
        
        _input = App.GetManager<InputManager>();
        _cameraInput = GetComponent<CameraInput>();
        
        _layerMask = LayerMask.GetMask("Minimo");
        _layerMask2 = LayerMask.GetMask("InteractObject");
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
        if (_isClicked) return;
        
        var hit = GetRaycastMinimo();
        _currentObject = hit;
        if (_currentObject != null) 
        {
            _currentObject.FSM.ChangeState(MinimoState.Drag);
            _cameraInput.enabled = false;
            
            _isClicked = true;
        }
    }

    private void HandleClickUp()
    {
        DropMinimo();
    }
    
    private void HandleDragEnd()
    {
        DropMinimo();
    }

    private void DropMinimo()
    {
        if (_currentObject == null) return;
        
        var hit = GetRaycastProduce();
        if (hit && !hit.IsMinimoWorking && hit.AnimTrigger != null) 
        {
            _currentObject.SetWorkState(hit);
        }
        else 
        {
            _currentObject.SetChillState();
        }
        
        _cameraInput.enabled = true;
        _currentObject = null;
        _isClicked = false;
    }

    private void HandleDrag()
    {
        if (_currentObject == null) return;
        
        var screenPosition = Input.mousePosition;
        var worldPosition = _mainCamera.ScreenToWorldPoint(
            new Vector3(screenPosition.x, screenPosition.y, _mainCamera.nearClipPlane));
        worldPosition.z = 0;
        _currentObject.transform.position = worldPosition;
    }

    private Minimo GetRaycastMinimo()
    {
        var worldPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.OverlapPoint(worldPosition, _layerMask);
        if (hit != null && hit.TryGetComponent<Minimo>(out var component))
        {
            return component;
        }
        
        return null;
    }
    
    private ProduceAdvanced GetRaycastProduce()
    {
        var worldPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.OverlapPoint(worldPosition, _layerMask2);
        if (hit != null && hit.TryGetComponent<ProduceAdvanced>(out var component))
        {
            return component;
        }
        
        return null;
    }
}
