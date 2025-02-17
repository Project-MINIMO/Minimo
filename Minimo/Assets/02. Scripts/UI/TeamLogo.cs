using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TeamLogo : MonoBehaviour
{
    [SerializeField] private Image _logoImg;
    [SerializeField] private Image _blackBlurImg;
    
    private void Start()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(0.5f)
            .Append(_logoImg.DOFade(1, 1))
            .AppendInterval(1f)
            .Append(_blackBlurImg.DOFade(1, 0.5f))
            .OnComplete(() =>
            {
                App.LoadScene(SceneName.Title);
            });
    }
}
