using UnityEngine;
using UnityEngine.UI;

public class TradeMinimoSlot : MonoBehaviour
{
    [SerializeField] private ItemInfoUpdater _itemInfo;
    [SerializeField] private Button _saleBtn;
    [SerializeField] private GameObject _specialMark;
    [SerializeField] private Image _fillImg;

    private MinimoRequest _currentRequest;

    private void Start()
    {
        _saleBtn.onClick.AddListener(() => TradeManager.Instance.Serve(_currentRequest));
    }

    public void Initialize(MinimoRequest request)
    {
        gameObject.SetActive(true);
        _currentRequest = request;
        _itemInfo.UpdateItem(request.RequestedItem);
        _fillImg.fillAmount = 1;
        _specialMark.SetActive(request.IsSpecial);
    }
    
    private void Update()
    {
        if (_currentRequest == null) return;
        if (_currentRequest.Timer > 0)
            _fillImg.fillAmount = _currentRequest.Timer / _currentRequest.WaitTime;
        else
            _fillImg.fillAmount = 0;

        if (_currentRequest.IsServed || _currentRequest.IsLeaving)
        {
            _currentRequest = null;
            gameObject.SetActive(false);
        }
    }
}
