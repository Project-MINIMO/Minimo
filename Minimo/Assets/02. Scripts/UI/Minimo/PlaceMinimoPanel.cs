using UnityEngine;
using UnityEngine.UI;

public class PlaceMinimoPanel : UIBase
{
    [SerializeField] private Button _closeBtn;
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        
        _closeBtn.onClick.AddListener(ClosePanel);
    }
}
