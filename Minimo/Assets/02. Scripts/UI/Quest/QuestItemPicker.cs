using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class QuestItemPicker : MonoBehaviour
{
    [SerializeField] private Transform _slotParent;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private ScrollRect _scrollRect;
    
    private List<QuestItemPickSlot> _slots;

    private void Awake()
    {
        InitSlots();
    }

    private void OnEnable()
    {
        if (_slots == null) return;
        
        _scrollRect.verticalNormalizedPosition = 1f;

        foreach (var slot in _slots)
        {
            slot.SetCount();
        }
    }

    private void InitSlots()
    {
        var itemDataList = App.GetData<TitleData>().Item;
        var existingSlots = GetComponentsInChildren<QuestItemPickSlot>(true);

        _slots = new List<QuestItemPickSlot>(itemDataList.Count);

        var i = 0;
        
        for (; i < itemDataList.Count; i++)
        {
            var slot = i < existingSlots.Length
                ? existingSlots[i]
                : Instantiate(_slotPrefab, _slotParent).GetComponent<QuestItemPickSlot>();

            slot.Initialize(itemDataList[i]);
            _slots.Add(slot);
        }

        for (; i < existingSlots.Length; i++)
        {
            existingSlots[i].gameObject.SetActive(false);
        }
    }
}
