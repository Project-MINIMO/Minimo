using UnityEngine;

public abstract class InteractObject : MonoBehaviour
{
    public virtual void OnClickDown(){ }
    public virtual void OnDrag(){ }
    public abstract void OnLongPress();
    public abstract void OnClickUp();
}
