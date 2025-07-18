using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class EditCirclePanel : UIBase
{
    [SerializeField] private RectTransform _rect;
    
    [SerializeField] private Button _confirmBtn;
    [SerializeField] private Button _cancelBtn;

    private EditManager _editManager;
    private Transform _target;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _editManager = App.GetManager<EditManager>();
        
        _editManager.IsEditing
            .Subscribe((isEditing) =>
            {
                if (isEditing)
                {
                    OpenPanel();
                }
                else
                {
                    ClosePanel();
                }
            }).AddTo(gameObject);
        
        _editManager.CurrentCellPosition
            .Subscribe(SetPosition).AddTo(gameObject);
        
        _confirmBtn.onClick.AddListener(_editManager.ConfirmEdit);
        _cancelBtn.onClick.AddListener(_editManager.CancelEdit);
    }
    
    private void SetPosition(Vector3 position)
    {
        var screenPos = Camera.main.WorldToScreenPoint(position);
        _rect.position = screenPos;
    }

    public void SetPosition()
    {
        if (!gameObject.activeSelf) return;
        
        var target = _editManager.CurrentEditObject;
        var screenPos = Camera.main.WorldToScreenPoint(target.transform.position);
        _rect.position = screenPos;
    }
}
