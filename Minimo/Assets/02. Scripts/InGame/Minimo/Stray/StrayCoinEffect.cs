using System;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using UnityEngine;
using TMPro;

public class StrayCoinEffect : MonoBehaviour
{
    [SerializeField] private EffectItem[] _effects;
    
    [Serializable]
    private class EffectItem
    {
        public CanvasGroup CanvasGroup;
        public RectTransform Rect;
        public TextMeshProUGUI AmountTMP;
        public Coroutine Routine;
    }
    
    private readonly Queue<EffectItem> _pool = new();
    private readonly List<EffectItem> _activeEffects = new();

    private const string PositiveString = "+ {0}";
    private const string NegativeString = "- {0}";

    private void Awake()
    {
        foreach (var effect in _effects)
        {
            _pool.Enqueue(effect);
        }
    }

    public void ShowEffect(int amount)
    {
        if (_pool.Count == 0)
        {
            var oldest = _activeEffects[0];
            _activeEffects.RemoveAt(0);
            StopCoroutine(oldest.Routine);
            ReturnToPool(oldest);
        }
        
        var item = _pool.Dequeue();
        
        var number = amount > 0 ? amount : -amount;
        item.AmountTMP.text = amount >= 0 
            ? string.Format(PositiveString, number) 
            : string.Format(NegativeString, number);
        _activeEffects.Add(item);
        
        item.Rect.gameObject.SetActive(true);
        item.Rect.transform.SetAsLastSibling();
        
        item.Routine = StartCoroutine(Lifecycle(item));
    }
   
    private IEnumerator Lifecycle(EffectItem item)
    {
        item.CanvasGroup.DOFade(1f, 0.3f);
        yield return new WaitForSeconds(1.3f);
        
        item.CanvasGroup.DOFade(0f, 0.3f);
        yield return new WaitForSeconds(0.3f);

        ReturnToPool(item);
    }

    private void ReturnToPool(EffectItem item)
    {
        item.CanvasGroup.DOKill();
        item.CanvasGroup.alpha = 0;
        item.Rect.gameObject.SetActive(false);
        
        _activeEffects.Remove(item);
        _pool.Enqueue(item);
    }
}
