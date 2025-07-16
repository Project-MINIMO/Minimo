using System.Collections.Generic;

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GetItemPanel : MonoBehaviour
{
    [SerializeField] private Image[] _iconImgs;
    [SerializeField] private RectTransform _storageRect;
    [SerializeField] private float _spawnInterval = 0.1f;
    
    private readonly Queue<int> _itemQueue = new(); 
    private readonly Queue<Image> _iconPool = new();
    
    private float _nextSpawnTime;
    private Dictionary<Image, Vector2> _startPositionMap;
    
    private bool _storageVisible;

    private void Awake()
    {
        _startPositionMap = new Dictionary<Image, Vector2>(_iconImgs.Length);
        foreach (var icon in _iconImgs)
        {
            _startPositionMap[icon] = icon.rectTransform.anchoredPosition;
        }
        
        foreach (var img in _iconImgs)
        {
            img.gameObject.SetActive(false);
            _iconPool.Enqueue(img);
        }
    }
    
    private void Update()
    {
        if (Time.time >= _nextSpawnTime 
            && _itemQueue.Count > 0 
            && _iconPool.Count > 0)
        {
            SpawnNext();
            _nextSpawnTime = Time.time + _spawnInterval;
        }
        
        var shouldShow = _iconPool.Count < _iconImgs.Length;
        if (shouldShow == _storageVisible) return;
        
        _storageVisible = shouldShow;
        _storageRect.gameObject.SetActive(_storageVisible);
    }
    
    public void EnqueueItem(int id) => _itemQueue.Enqueue(id);
    public void EnqueueItems(IEnumerable<int> ids)
    {
        foreach (var id in ids) _itemQueue.Enqueue(id);
    }
    
    private void SpawnNext()
    {
        var itemId = _itemQueue.Dequeue();
        var img = _iconPool.Dequeue();

        SetItemIcon(itemId, img);

        img.rectTransform
            .DOScale(1f, 0.5f).SetEase(Ease.OutElastic)
            .OnComplete(() =>
            {
                img.rectTransform.DOAnchorPos(_storageRect.anchoredPosition, 0.5f).SetEase(Ease.InBack);
                img.rectTransform.DOScale(0f, 0.3f).SetEase(Ease.InCubic).SetDelay(0.2f);
                img.rectTransform.DOScale(0f, 0.3f).SetEase(Ease.InCubic).SetDelay(0.2f)
                    .OnComplete(() =>
                    {
                        img.gameObject.SetActive(false);
                        _iconPool.Enqueue(img);
                    });
            });
    }
    
    private void SetItemIcon(int itemId, Image image)
    {
        image.sprite = AccountInfo.Instance.Items[itemId].Icon;
        image.rectTransform.anchoredPosition = _startPositionMap[image];
        image.rectTransform.localScale = Vector3.zero;  
        
        image.gameObject.SetActive(true);
    }
}
