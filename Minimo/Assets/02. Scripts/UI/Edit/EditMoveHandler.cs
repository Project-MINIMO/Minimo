using UnityEngine;
using UnityEngine.EventSystems;

public class EditMoveHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private EditManager _editManager;
    
    private Vector3 _offset;
    private bool _isDrag = false;
    
    private void Start()
    {
        _editManager = App.GetManager<EditManager>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDrag = true;
        
        var touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        touchPosition.z = 0;
        _offset = _editManager.CurrentEditObject.transform.position - touchPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDrag = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDrag)
        {
            return;
        }
        
        var touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        touchPosition.z = 0;
        
        var newPosition = touchPosition + _offset;

        _editManager.MoveObject(newPosition);
    }
}
