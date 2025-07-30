using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

public class EditCirclePanel : MonoBehaviour
{
    [SerializeField] private RectTransform _rect;
    
    [SerializeField] private Button _confirmBtn;
    [SerializeField] private Button _cancelBtn;
    [SerializeField] private Button _rotateBtn;
    [SerializeField] private Button _deleteBtn;

    private EditManager _editManager;
    private Transform _startParent;

    private void Awake()
    {
        _editManager = App.GetManager<EditManager>();
        
        _confirmBtn.onClick.AddListener(ConfirmEdit);
        _cancelBtn.onClick.AddListener(_editManager.CancelEdit);
        _rotateBtn.onClick.AddListener(_editManager.RotateObject);
        _deleteBtn.onClick.AddListener(_editManager.DeleteObject);
        
        _startParent = transform.parent;
    }
    
    public void Attach(BuildingObject buildingObject)
    {
        var canvas = buildingObject.GetComponentInChildren<Canvas>();
        transform.SetParent(canvas.transform);
        _rect.position = Vector3.zero;
        _rect.localScale = Vector3.one;
        gameObject.SetActive(true);
    }

    public void Detach()
    {
        transform.SetParent(_startParent);
        _rect.position = Vector3.zero;
        _rect.localScale = Vector3.one;
        gameObject.SetActive(false);
    }

    private void ConfirmEdit()
    {
        App.Loading.RunWithSpinnerAsync(_editManager.ConfirmEdit()).Forget();
    }
}
