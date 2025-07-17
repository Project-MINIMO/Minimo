using System;

using UnityEngine;
using UnityEngine.UI;

public abstract class InventorySlot<T> : MonoBehaviour
{
    public abstract bool CanShow();
    public T Item { get; protected set; }
    
    public event Action<InventorySlot<T>> OnItemSelected;
    
    [SerializeField] private Button _inventoryBtn;
    
    public virtual void Initialize(T item)
    {
        _inventoryBtn.onClick.AddListener(() => OnItemSelected?.Invoke(this));
    }
}
