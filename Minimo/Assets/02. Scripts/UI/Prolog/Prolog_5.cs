using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Prolog_5 : PrologBase
{
    [SerializeField] private Image _background;
    [SerializeField] private Image _red;

    private bool _loopEnd;
    
    protected override IEnumerator ShowProlog()
    {
        FadeIn(_background, 1f);
        yield return new WaitForSeconds(1f);

        _red.DOFade(0.02f, 0.5f).SetEase(Ease.Linear).SetLoops(6, LoopType.Yoyo)
            .OnComplete(() => _loopEnd = true);

        yield return new WaitUntil(() => _loopEnd);
        
        EndProlog();
    }
}
