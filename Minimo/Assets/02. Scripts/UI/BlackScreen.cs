using System;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BlackScreen : MonoBehaviour
{
    private Image _blackBlur;

    private void Awake()
    {
        _blackBlur = GetComponent<Image>();
    }

    public void FadeIn(Action onComplete = null)
    {
        _blackBlur.gameObject.SetActive(true);

        _blackBlur.DOKill();
        _blackBlur.DOFade(1f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void FadeOut(float duration, Action onComplete = null)
    {
        _blackBlur.DOKill();
        _blackBlur.DOFade(0f, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            _blackBlur.gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }

    public void FadeInOut(float duration, Action midAction = null)
    {
        _blackBlur.gameObject.SetActive(true);

        _blackBlur.DOKill();
        _blackBlur.DOFade(1f, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            midAction?.Invoke();
            FadeOut(duration);
        });
    }
}
