using System.Collections.Generic;

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GetItemPanel : MonoBehaviour
{
    [SerializeField] private Image[] _iconImgs;
    [SerializeField] private RectTransform _storageRect;
    [SerializeField] private float _spawnInterval = 0.8f;
    [SerializeField] private RectTransform _iconParent;
    
    private readonly Queue<(int, Vector3)> _itemQueue = new(); 
    private readonly Queue<Image> _iconPool = new();
    
    private float _nextSpawnTime;
    private Dictionary<Image, Vector2> _startPositionMap;
    
    private int _storageVisible;

    private void Awake()
    {
        _startPositionMap = new Dictionary<Image, Vector2>(_iconImgs.Length);
        
        foreach (var img in _iconImgs)
        {
            img.gameObject.SetActive(false);
            _startPositionMap[img] = img.rectTransform.anchoredPosition;
            _iconPool.Enqueue(img);
        }
        
        _storageRect.localScale = Vector3.zero; 
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
    }
    
    public void EnqueueItem(int id, Transform trans) => _itemQueue.Enqueue((id, trans.position));
    
    private void SpawnNext()
    {
        if (_storageVisible++ == 0)
        {
            _storageRect.DOKill();
            _storageRect.DOScale(1f, 0.1f).SetEase(Ease.OutCirc);
        } 

        var itemId = _itemQueue.Dequeue();
        var img = _iconPool.Dequeue();
        SetItemIcon(itemId.Item1, img);
        
        var rect = img.rectTransform;

        var screenPos = Camera.main.WorldToScreenPoint(itemId.Item2);
        if (screenPos.z < 0f) { screenPos.z = 0f; } 
        rect.position = screenPos;
        
        var sequence = DOTween.Sequence();
        sequence
            .Append(rect.DOScale(1f, 0.5f).SetEase(Ease.OutElastic))
            .AppendInterval(0.3f)
            .Append(rect.DOAnchorPos(_storageRect.anchoredPosition, 0.5f).SetEase(Ease.InBack))
            .Join(rect.DOScale(0f, 0.3f).SetEase(Ease.InCubic).SetDelay(0.2f))
            .AppendCallback(() =>
            {
                img.gameObject.SetActive(false);
                _iconPool.Enqueue(img);
                _storageVisible--;
                AnimateStorageBounce();
            })
            .Play();
    }
    
    private void SetItemIcon(int itemId, Image image)
    {
        image.sprite = AccountInfo.Instance.Items[itemId].Icon;
        image.rectTransform.anchoredPosition = _startPositionMap[image];
        image.rectTransform.localScale = Vector3.zero;  
        
        image.gameObject.SetActive(true);
    }
    
    private void AnimateStorageBounce()
    {
        var sequence = DOTween.Sequence();
        sequence
            .Append(_storageRect.DOScale(1.5f, 0.05f).SetEase(Ease.OutCirc))
            .Append(_storageRect.DOScale(1f, 0.05f).SetEase(Ease.Linear))
            .OnComplete(() =>
            {
                if (_storageVisible == 0)
                {
                    _storageRect.DOScale(0f, 0.1f).SetEase(Ease.Linear);
                }
            })
            .Play();
    }
}
