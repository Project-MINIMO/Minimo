using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditPanel : UIBase
{
    public override bool IsUseInput => true;
    
    [SerializeField] private EditCircleHandler _editHandler;
    
    private EditManager _editManager;
    private InputManager _inputManager;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _editManager = App.GetManager<EditManager>();
        _inputManager = App.GetManager<InputManager>();
        
        _editManager.IsBuildingEditing
            .Subscribe(isEditing =>
            {
                if (isEditing)
                {
                    OpenPanel();
                    _editHandler.Attach(_editManager.CurrentEditObject);
                }
                else
                {
                    ClosePanel();
                    _editHandler.Detach();
                }
            })
            .AddTo(gameObject);
    }
    
    private void Update()
    {
        if (!gameObject.activeInHierarchy) return; 
        if (_inputManager.InputTarget != InputTargetType.Camera) return;
        if (_editManager.CurrentEditObject == null) return;

        if (_inputManager.CurrentState is InputState.ClickDown or InputState.Drag)
        {
            var screenPos = GetCurrentScreenPosition();
            var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            worldPos.z = 0;

            _editManager.MoveObject(worldPos);
        }
    }
    
    private Vector3 GetCurrentScreenPosition()
    {
#if UNITY_EDITOR
        return Input.mousePosition;
#else
        return Input.touchCount > 0 ? Input.GetTouch(0).position : Vector3.zero;
#endif
    }
}
