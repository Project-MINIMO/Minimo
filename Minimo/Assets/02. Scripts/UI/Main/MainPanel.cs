using UniRx;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainPanel : UIBase
{
    [SerializeField] private RectTransform _mainRect;
    
    [SerializeField] private Button _mainBtn;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _storageBtn;
    [SerializeField] private Button _buildingBtn;
    [SerializeField] private Button _minimoBtn;
    [SerializeField] private Button _friendBtn;
    [SerializeField] private Button _optionBtn;
    
    private bool _isOpened = false;

    public override void Initialize()
    {
        var screenStateManager = App.GetManager<ScreenStateManager>();
        screenStateManager.CurrentState.Subscribe((currentState) =>
        {
            var isActive = currentState is ScreenState.Town or ScreenState.Sky;
            if (isActive)
            {
                OpenPanel();
            }
            else
            {
                ClosePanel();
            }
        }).AddTo(gameObject);
        
        _mainBtn.onClick.AddListener(OnClickMain);
        _closeBtn.onClick.AddListener(()=>
        {
            if (!_isOpened) return;
            OnClickMain();
        });

        _buildingBtn.onClick.AddListener(() =>
        {
            OnClickMain();
            App.GetManager<UIManager>().GetPanel<BuildingPanel>().OpenPanel();
        });
        _storageBtn.onClick.AddListener(() =>
        {
            OnClickMain();
            App.GetManager<UIManager>().GetPanel<StoragePanel>().OpenPanel();
        });
    }
    
    private void OnClickMain()
    {
        _isOpened = !_isOpened;
        
        _closeBtn.gameObject.SetActive(_isOpened);
        _mainRect.DOKill();
        _mainRect.DOAnchorPosY(_isOpened? 0f : -150f, 0.5f);
        
        _storageBtn.gameObject.SetActive(_isOpened);
        _buildingBtn.gameObject.SetActive(_isOpened);
        _minimoBtn.gameObject.SetActive(_isOpened);
        _friendBtn.gameObject.SetActive(_isOpened);
        _optionBtn.gameObject.SetActive(_isOpened);
    }
}
