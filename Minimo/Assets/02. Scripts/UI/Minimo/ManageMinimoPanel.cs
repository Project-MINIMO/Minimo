using UnityEngine;
using UnityEngine.UI;

public class ManageMinimoPanel : UIBase
{
    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn;
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _openBtn.onClick.AddListener(OpenPanel);
        _closeBtn.onClick.AddListener(ClosePanel);
    }
}