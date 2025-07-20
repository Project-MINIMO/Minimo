using UnityEngine;
using DG.Tweening;

public class QuestTransitioner : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private CanvasGroup _canvasGroup2;
    [SerializeField] private RectTransform _canvasRect;
    
    private const float PrimaryStartX = 120f;
    private const float SecondaryStartX = 200f;
    private const float OpenTargetX = 160f;
    private const float AnimationDuration = 0.3f;

    public void Open(bool isNew)
    {
        AnimateOpen(isNew ? PrimaryStartX : SecondaryStartX);
    }
    
    private void AnimateOpen(float fromX)
    {
        _canvasGroup2.alpha = 0;
        _canvasGroup2.blocksRaycasts = false;

        _canvasGroup2.alpha = 1;
        _canvasGroup.alpha = 0;
        _canvasRect.anchoredPosition = new Vector2(fromX, 0);

        _canvasRect
            .DOAnchorPosX(OpenTargetX, AnimationDuration)
            .SetEase(Ease.Linear);

        _canvasGroup
            .DOFade(1, AnimationDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => _canvasGroup2.blocksRaycasts = true);
    }
}
