using System;
using UnityEngine;
using DG.Tweening;

public class QuestTransitioner : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private CanvasGroup _parentCanvasGroup;

    private RectTransform _rect;
    
    private const float PrimaryStartX = 120f;
    private const float SecondaryStartX = 200f;
    private const float OpenTargetX = 160f;
    private const float AnimationDuration = 0.3f;

    private void Awake()
    {
        _rect = _canvasGroup.GetComponent<RectTransform>();
    }

    public void Open(bool isNew)
    {
        AnimateOpen(isNew ? PrimaryStartX : SecondaryStartX);
    }
    
    private void AnimateOpen(float fromX)
    {
        _canvasGroup.alpha = 0;
        _parentCanvasGroup.blocksRaycasts = false;

        _rect.anchoredPosition = new Vector2(fromX, 0);

        _rect
            .DOAnchorPosX(OpenTargetX, AnimationDuration)
            .SetEase(Ease.Linear);

        _canvasGroup
            .DOFade(1, AnimationDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => _parentCanvasGroup.blocksRaycasts = true);
    }
}
