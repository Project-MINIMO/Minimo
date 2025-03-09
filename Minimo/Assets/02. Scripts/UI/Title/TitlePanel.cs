using System;
using MinimoShared;
using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TitlePanel : MonoBehaviour
{
    [SerializeField] private Button _startBtn;
    [SerializeField] private RectTransform _startTextRect;
    [SerializeField] private Image _titleFstImg;
    [SerializeField] private Image _titleSndImg;
    
    [SerializeField] private TitleLoadHandler _loadHandler;
    
    [SerializeField] private Image _blackBlur;
    [SerializeField] private Image _blackBlur2;

    private bool _isNew;
    
    private void Awake()
    {
        _startBtn.onClick.AddListener(OnClickStart);
        _startBtn.gameObject.SetActive(false);
    }

    private void Start()
    {
        _blackBlur2.DOFade(0, 2f).SetEase(Ease.Linear);
    }

    public async UniTask ShowTitle(bool isNew = false)
    {
        Debug.Log(" ShowTitle");
        App.GetData<TitleData>().IsFirstLogin = isNew;
        _isNew = isNew;
        _loadHandler.Setup(10);
        
        App.GetManager<AccountInfoManager>().Setup();
        
        App.GetManager<CheatManager>().UpdateItem(new ItemDTO {ItemType = "Item_Timber", Count = 10}).Forget();
        _loadHandler.UpdateLoad();
       
        foreach (var building in App.GetData<TitleData>().Building.Values)
        {
            if (building.Type >= 1) continue;
          
            if (App.GetManager<AccountInfoManager>().Level.Value >= building.UnlockLevel)
            { 
                var produceSlotCount = building.ID == EBuilding.Orchard ? 5 : 3;
                await App.GetManager<CheatManager>().UpdateBuildingInfo(new BuildingInfoDTO()
                {
                    BuildingType = "Building_Orchard",
                    MaxCount = 5,
                    ProduceSlotCount = produceSlotCount,
                });
                
                _loadHandler.UpdateLoad();
            }
        }
        
        _loadHandler.FinishLoad();
        _startBtn.gameObject.SetActive(true);
        
        var startPositionY = _startTextRect.anchoredPosition.y;
        _startTextRect.DOAnchorPosY(startPositionY + 10f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear);
    }

    private void OnClickStart()
    {
        var currentValue = 100;
        DOTween.To(() => currentValue, x =>
        {
            currentValue = x;
            AkSoundEngine.SetRTPCValue("BGMFade", currentValue);
        }, 0, 1.5f);
        
        _startTextRect.DOKill();
        _startBtn.gameObject.SetActive(false);
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_titleFstImg.DOFade(0, 1))
            .Join(_titleSndImg.DOFade(1, 1))
            .AppendCallback(() => _blackBlur.gameObject.SetActive(true))
            .Append(_blackBlur.DOFade(1, 0.5f))
            .OnComplete(() =>
            {
                App.LoadScene(_isNew? SceneName.Prolog : SceneName.Game);
            });
    }
}
