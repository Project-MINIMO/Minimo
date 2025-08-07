using UnityEngine;
using UnityEngine.UI;

public class TertiaryPanel : ElevatedPanel
{
    [SerializeField] private Button _prevBtn;
    [SerializeField] private Button _nextBtn;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _prevBtn.onClick.AddListener(() => _produceManager.MoveToNextTertiary(-1));
        _nextBtn.onClick.AddListener(() => _produceManager.MoveToNextTertiary(1));
    }
}
