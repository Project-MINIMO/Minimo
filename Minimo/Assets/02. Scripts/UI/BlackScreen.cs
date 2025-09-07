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
    
    private void CheckBlackBlur()
    {
        if (_blackBlur == null)
        {
            _blackBlur = GetComponent<Image>();
            return;
        }
    }

    public void FadeIn(Action onComplete = null)
    {
        CheckBlackBlur();
        
        _blackBlur.gameObject.SetActive(true);

        _blackBlur.DOKill();
        _blackBlur.DOFade(1f, 0.5f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void FadeOut(float duration, Action onComplete = null)
    {
        CheckBlackBlur();
        
        _blackBlur.DOKill();
        _blackBlur.DOFade(0f, duration).SetEase(Ease.InCubic).OnComplete(() =>
        {
            _blackBlur.gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }

    public void FadeInOut(float duration, Action midAction = null)
    {
        CheckBlackBlur();
        
        if (_blackBlur == null)
        {
            Debug.LogError("BlackScreen component is not assigned.");
            midAction?.Invoke();
            return;
        }
        _blackBlur.gameObject.SetActive(true);

        _blackBlur.DOKill();
        _blackBlur.DOFade(1f, duration).SetEase(Ease.InCubic).OnComplete(() =>
        {
            midAction?.Invoke();
            FadeOut(duration);
        });
    }
    
    private void OnDestroy()
    {
        _blackBlur.DOKill();
        _blackBlur.gameObject.SetActive(false);
    }
}
