using UnityEngine;
using TMPro;
using DG.Tweening;

public class ProduceCostEffectCtrl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _costTMP;
    [SerializeField] private CanvasGroup _costGroup;
    
    public void Install(int cost)
    {
        _costTMP.text = $" - {cost}";
        _costGroup.alpha = 1;
        _costGroup.DOFade(0f, 1f).SetEase(Ease.Linear);
        _costGroup.GetComponent<RectTransform>().DOAnchorPosY(125, 1f).SetEase(Ease.Linear);
    }
}
