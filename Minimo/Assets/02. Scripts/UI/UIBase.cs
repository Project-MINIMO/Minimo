using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    protected virtual GameObject Panel => gameObject;
    public virtual bool IsDefaultPanel => false;
    public virtual bool IsUseBlur => false;
    
    private UIManager _manager;

    /// <summary>
    /// Initialize Panel.
    /// Called once on Awake.
    /// </summary>
    public virtual void Initialize(UIManager manager) => _manager = manager;

    public virtual void OpenPanel() => _manager.PushPanel(this);
    public virtual void ClosePanel() => _manager.PopPanel(this);
    
    public virtual void Show(bool isNew) => Panel.SetActive(true);
    public virtual void Hide(bool isNew) => Panel.SetActive(false);
}
