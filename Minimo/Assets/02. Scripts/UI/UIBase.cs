using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    [SerializeField] private UIGuide _guide;
    
    public virtual bool IsDefaultPanel => false;
    public virtual bool IsUseBlur => false;
    public virtual bool IsUseProduceBlur => false;
    public virtual bool IsUseInput => false;

    protected virtual bool IsUseGuide => false;
    protected bool IsFirstOpen = true;
    
    private UIManager _manager;

    /// <summary>
    /// Initialize Panel.
    /// Called once on Awake.
    /// </summary>
    public virtual void Initialize(UIManager manager)
    {
        _manager = manager;

        if (IsUseGuide)
        {
            _guide.gameObject.SetActive(false);
        }
    }

    public virtual void OpenPanel()
    {
        _manager.PushPanel(this);

        if (IsUseGuide && IsFirstOpen && !TutorialManager.IsTutorialing)
        {
            ShowGuide();
            IsFirstOpen = false;
        }
    }

    public virtual void ClosePanel() => _manager.PopPanel(this);
    
    public virtual void Show(bool isNew) => gameObject.SetActive(true);
    public virtual void Hide(bool isNew) => gameObject.SetActive(false);

    protected void ShowGuide() => _guide.gameObject.SetActive(true);
}
