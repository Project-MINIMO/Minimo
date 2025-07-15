using UnityEngine;
using UnityEngine.EventSystems;

public class SwitchPanel : UIBase, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public override bool IsDefaultPanel => true;
    
    [SerializeField] private float _dragThreshold = 30f;
    
    private ScreenStateManager _screenStateManager;
    private Vector2 _startPos;
    private bool _isDrag;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _screenStateManager = App.GetManager<ScreenStateManager>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        _startPos = eventData.position;
        _isDrag = true;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDrag) return;
        
        var dragDelta = eventData.position - _startPos;
        if (Mathf.Abs(dragDelta.y) > _dragThreshold)
        {
            _isDrag = false;
            
            if (dragDelta.y > 0)
            {
                OnDragUp();
            }
            else
            {
                OnDragDown();
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDrag = false;
    }

    private void OnDragUp()
    {
        _screenStateManager.ChangeState(-1);
    }

    private void OnDragDown()
    {
        _screenStateManager.ChangeState(1);
    }
}
