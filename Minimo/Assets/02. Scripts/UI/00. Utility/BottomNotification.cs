using System.Collections.Generic;

using UnityEngine;
using TMPro;
using DG.Tweening;

public class BottomNotification : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _messageTMP1;
    [SerializeField] private TextMeshProUGUI _messageTMP2;
    [SerializeField] private float _slideHeight = 25f;
    
    private RectTransform _rect;
    private CanvasGroup _canvasGroup;
    
    private Vector2 _startPosition;
    private float _shownPositionY;
    
    private Sequence _sequence;

    private Dictionary<NotifyType, string> _notifications;

    private void Start()
    {
        _rect = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _startPosition = _rect.anchoredPosition;
        _shownPositionY = _startPosition.y + _slideHeight;

        var titleData = App.GetData<TitleData>();
        _notifications = new Dictionary<NotifyType, string>()
        {
            [NotifyType.TutorialSkip] = "튜토리얼을 스킵합니다.",
            [NotifyType.PrologSkip] = "프롤로그를 스킵합니다.",
            [NotifyType.DeleteUser] = "유저 정보를 삭제합니다.",
            
            [NotifyType.StorageCapacityLack] = titleData.GetString("STR_PRODUCE_STORAGE"),
            [NotifyType.MinimoCapacityLack] = "미니모 거주지를 늘려주세요.",
            
            [NotifyType.SlotLack] = titleData.GetString("STR_NOTIFY_PRODUCE_SLOTLACK"),
            [NotifyType.MissMinimo] = "배치할 수 있는 미니모가 없습니다.",
            [NotifyType.MissRecipe] = titleData.GetString("STR_NOTIFY_PRODUCE_MISSRECIPE"),
            
            [NotifyType.GoldLack] = "골드가 부족합니다.",
            [NotifyType.ItemLack] = "해당 아이템이 부족합니다.",
            
            [NotifyType.CannotReplaceTile] = "해당 타일은 수정할 수 없습니다.",
            [NotifyType.CannotEraseTile] = titleData.GetString("STR_TILEMANAGE_TAB3_DESC2"),
            [NotifyType.CannotInstallWaterTile] = "건물이 설치된 타일입니다.",
            [NotifyType.DeselectTile] = titleData.GetString("STR_SELECTSLOTEMPTY_DESC"),
            
            [NotifyType.DeleteBuildingFailProduce] = "생산을 완료해 주세요.",
            [NotifyType.DeleteBuildingFailMinimo] = "미니모를 해제해 주세요.",
            
            [NotifyType.StarsLimit] = "별 100개 넘었음^^",
        };
        
        Reset();
    }

    public void ShowNotification(NotifyType type)
    {
        if (!_notifications.TryGetValue(type, out var notification)) return;
        
        _messageTMP1.SetText(notification);
        _messageTMP2.SetText(notification);
        
        _sequence = DOTween.Sequence();
        _sequence
            .SetAutoKill(false)
            .OnStart(Reset)
            .Append(_rect.DOAnchorPosY(_shownPositionY, 0.5f).SetEase(Ease.OutCubic))
            .Join(_canvasGroup.DOFade(1f, 0.5f).SetEase(Ease.OutCubic))
            .AppendInterval(2f)
            .Append(_rect.DOAnchorPosY(_startPosition.y, 0.25f).SetEase(Ease.InCubic))
            .Join(_canvasGroup.DOFade(0f, 0.25f).SetEase(Ease.InCubic))
            .Play();
    }

    private void Reset()
    {
        _rect.anchoredPosition = _startPosition;
        _canvasGroup.alpha = 0f;
    }
}
