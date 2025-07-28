using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class EditCirclePanel : UIBase
{
    [SerializeField] private RectTransform _rect;
    
    [SerializeField] private Button _confirmBtn;
    [SerializeField] private Button _cancelBtn;
    [SerializeField] private Button _rotateBtn;
    [SerializeField] private Button _deleteBtn;

    private EditManager _editManager;
    private Transform _target;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _editManager = App.GetManager<EditManager>();
        
        _editManager.IsBuildingEditing
            .Subscribe(isEditing =>
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
        
        _confirmBtn.onClick.AddListener(() => ConfirmEdit().Forget());
        _cancelBtn.onClick.AddListener(_editManager.CancelEdit);
        _rotateBtn.onClick.AddListener(_editManager.RotateObject);
        _deleteBtn.onClick.AddListener(_editManager.DeleteObject);
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

    private async UniTask ConfirmEdit()
    {
        _confirmBtn.interactable = false;
        await _editManager.ConfirmEdit();
        _confirmBtn.interactable = true;
    }
}
