using UnityEngine;
using UnityEngine.UI;

public class TertiaryPanel : ElevatedPanel
{
    protected override bool IsUseGuide => true;
    
    [SerializeField] private Button _prevBtn;
    [SerializeField] private Button _nextBtn;

    [SerializeField] private Button _infoBtn;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _prevBtn.onClick.AddListener(() => _produceManager.MoveToNextTertiary(-1));
        _nextBtn.onClick.AddListener(() => _produceManager.MoveToNextTertiary(1));
        _infoBtn.onClick.AddListener(ShowGuide);
    }
}
