using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Prolog_6 : PrologBase
{
    [SerializeField] private Image _background;
    
    protected override IEnumerator ShowProlog()
    {
        var soundManager = App.GetManager<SoundManager>();
        FadeIn(_background, 1f);
        yield return new WaitForSeconds(0.4f);
        _background.rectTransform
            .DOShakePosition(
                duration: 0.6f,                   // 전체 흔들림 시간
                strength: new Vector3(20f, 10f, 0f), // X/Y 진폭 (픽셀 단위)
                vibrato: 15,                      // 진동 횟수
                randomness: 45f,                   // 랜덤 각도
                snapping: false,
                fadeOut: true
            )
            .SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(2.6f);
        soundManager.StopSFX("Siren");
        EndProlog();
    }
}
