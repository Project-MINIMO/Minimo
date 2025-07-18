using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : ManagerBase
{
    public InputState CurrentState { get; private set; } = InputState.None;
    
    private const float DragThreshold = 10f; 
    private const float LongPressThreshold = 1f; 
    
    private Vector2 _startPos;
    private float _startTime;
    private bool _isDragging;
    private bool _longPressed;
    
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
                    CurrentState = InputState.None;
                }
                break;
            
            case 1:
                HandleSingleTouch(Input.GetTouch(0));
                break;
            
            case 2:
                CurrentState = InputState.Zoom;
                break;
        }
#endif
    }
    
    private void HandleSingleTouch(Touch touch)
    {
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            Reset();
            return;
        }

        switch (touch.phase)
        {
            case TouchPhase.Began:
                BeginInput(touch.position);
                break;

            case TouchPhase.Moved:
                MoveInput(touch.position);
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                EndInput();
                break;
        }
    }
    
    private void BeginInput(Vector2 pos)
    {
        _startPos    = pos;
        _startTime   = Time.time;
        _isDragging  = _longPressed = false;
        CurrentState = InputState.ClickDown;
    }
    
    private void MoveInput(Vector2 pos)
    {
        switch (_isDragging)
        {
            case false when Vector2.Distance(pos, _startPos) > DragThreshold:
                _isDragging  = true;
                CurrentState = InputState.Drag;
                break;
            
            case false when !_longPressed && Time.time - _startTime > LongPressThreshold:
                _longPressed = true;
                CurrentState = InputState.LongPress;
                break;
        }
    }
    
    private void EndInput()
    {
        if (_isDragging)        CurrentState = InputState.DragEnd;
        else if (!_longPressed) CurrentState = InputState.ClickUp;
        else                    CurrentState = InputState.None;
    }
   
    private void Reset()
    {
        CurrentState = InputState.None;
        _isDragging = _longPressed = false;
    }

    #region Mouse
    private void HandleMouse()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            BeginInput(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            MoveInput(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndInput();
        }
        else if (CurrentState is InputState.DragEnd or InputState.ClickUp)
        {
            CurrentState = InputState.None;
        }
    }
    #endregion
}
