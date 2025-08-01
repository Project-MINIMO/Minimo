using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class EditPanel : UIBase
{
    [SerializeField] private EditCircleHandler _editHandler;
    
    private EditManager _editManager;

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
}
