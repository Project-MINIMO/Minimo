using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class EditPanel : UIBase
{
    [SerializeField] private Button _buildingBtn;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private EditCircleHandler _editHandler;
    
    private EditManager _editManager;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        var buildingPanel = manager.GetPanel<BuildingPanel>();
        _buildingBtn.onClick.AddListener(buildingPanel.OpenPanel);
        _closeBtn.onClick.AddListener(ClosePanel);
        
        _editManager = App.GetManager<EditManager>();
        
        _editManager.IsBuildingEditing
            .Subscribe(isEditing =>
            {
                if (isEditing)
                {
                    OpenPanel();
                    _editHandler.gameObject.SetActive(true);
                    _editHandler.Attach(_editManager.CurrentEditObject);
                }
                else
                {
                    _editHandler.Detach();
                }
            })
            .AddTo(gameObject);
    }
}
