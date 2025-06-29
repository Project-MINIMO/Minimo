using UnityEngine;
using UnityEngine.UI;

public class PlaceMinimoPanel : UIBase
{
    [SerializeField] private Button _closeBtn;
    public override void Initialize()
    {
        _closeBtn.onClick.AddListener(ClosePanel);
    }
}
