using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : ManagerBase
{
    public InputState CurrentState { get; private set; } = InputState.None;
    public InputTargetType InputTarget { get; private set; } = InputTargetType.None;
    public InteractObject CurrentInteractObject { get; private set; }
    
    private const float DragThreshold = 10f; 
    private const float LongPressThreshold = 1f; 
    
    private Vector2 _startPos;
    private float _startTime;
    
    private bool _isDragging;
    private bool _longPressed;
    
    private int _interactFingerId = -1;
    
    private int _layerMask;

    private void Start()
    {
        _layerMask = LayerMask.GetMask("InteractObject");
    }
    
    private void Update()
    {
#if UNITY_EDITOR
        HandleMouse();
#else
        switch (Input.touchCount)
        {
            case 0:
                if (CurrentState is InputState.DragEnd or InputState.ClickUp)
                {
                    SetState(InputState.None);
                }
                break;
            
            case 1:
                HandleSingleTouch(Input.GetTouch(0));
                break;
            
            case >= 2:
                SetState(InputState.Zoom);
                SetTarget(InputTargetType.Camera);
                CurrentInteractObject = null;
                break;
        }
#endif
    }
    
    private void HandleSingleTouch(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                BeginInput(touch.fingerId, touch.position);
                break;

            case TouchPhase.Moved:
                MoveInput(touch.fingerId, touch.position);
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                EndInput(touch.fingerId);
                break;
        }
    }
    
    private void BeginInput(int fingerId, Vector2 pos)
    {
        _startPos = pos;
        _startTime = Time.time;
        _isDragging = _longPressed = false;
        _interactFingerId = fingerId;
        
#if UNITY_EDITOR || UNITY_STANDALONE
        if (EventSystem.current.IsPointerOverGameObject())
#else
        if (EventSystem.current.IsPointerOverGameObject(fingerId))
#endif
        {
            SetTarget(InputTargetType.UI);
            SetState(InputState.None);
            return;
        }

        CurrentInteractObject = TryGetInteractObject(pos);
        SetTarget(CurrentInteractObject ? InputTargetType.Object : InputTargetType.Camera);

        SetState(InputState.ClickDown);
    }
    
    private void MoveInput(int fingerId, Vector2 pos)
    {
        if (fingerId != _interactFingerId) return;
        if (InputTarget == InputTargetType.UI) return;
        
        switch (_isDragging)
        {
            case false when Vector2.Distance(pos, _startPos) > DragThreshold:
                _isDragging  = true;
                SetState(InputState.Drag);
                if (CurrentInteractObject != null && !CurrentInteractObject.IsUseDrag)
                {
                    SetTarget(InputTargetType.Camera);
                }
                break;
            
            case false when !_longPressed && Time.time - _startTime > LongPressThreshold:
                _longPressed = true;
                SetState(InputState.LongPress);
                break;
        }
    }
    
    private void EndInput(int fingerId)
    {
        if (fingerId != _interactFingerId) return;
        
        switch (InputTarget)
        {
            case InputTargetType.Object:
                if (!_longPressed && !_isDragging) SetState(InputState.ClickUp);
                else if (_isDragging) SetState(InputState.DragEnd);
                else SetState(InputState.None);
                break;

            case InputTargetType.Camera:
            case InputTargetType.UI:
                SetState(InputState.None);
                break;
        }
        
        _interactFingerId = -1;
        _isDragging = _longPressed = false;
        CurrentInteractObject = null;
    }
    
    private void SetState(InputState newState)
    {
        //if (CurrentState != newState) Debug.Log($"[Input] {CurrentState} → {newState}");
        CurrentState = newState;
    }
    
    private void SetTarget(InputTargetType newTarget)
    {
        if (InputTarget != newTarget) Debug.Log($"[Input] {InputTarget} → {newTarget}");
        InputTarget = newTarget;
    }
    
    private InteractObject TryGetInteractObject(Vector2 screenPos)
    {
        var worldPosition = Camera.main.ScreenToWorldPoint(screenPos);
        var hit = Physics2D.OverlapPoint(worldPosition, _layerMask);
        if (hit != null && hit.TryGetComponent<InteractObject>(out var component))
        {
            return component;
        }
        
        return null;
    }

    #region Mouse
    private void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            BeginInput(0, Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            MoveInput(0, Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndInput(0);
        }
        else if (CurrentState is InputState.DragEnd or InputState.ClickUp)
        {
            SetState(InputState.None);
        }
        else
        {
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0) SetState(InputState.Zoom);
        }
    }
    #endregion
}
