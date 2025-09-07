using UnityEngine;

public abstract class InteractObject : MonoBehaviour
{
    public virtual bool IsUseDrag => false;
    public virtual void OnClickDown(){ }
    public virtual void OnDrag(){ }
    public virtual void OnDragEnd(){ }
    public abstract void OnLongPress();
    public abstract void OnClickUp();
}
