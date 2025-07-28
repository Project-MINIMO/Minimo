using System;
using Cysharp.Threading.Tasks;

using UnityEngine;
using DG.Tweening;

public class LoadingSpinner : MonoBehaviour
{
    [SerializeField] private GameObject _loadingObj;
    [SerializeField] private RectTransform _rect;
    [SerializeField] private float _duration = 1;
    
    public async UniTask RunWithSpinnerAsync(UniTask task)
    {
        StartRotation();
        
        try
        {
            await task;
        }
        finally
        {
            StopRotation();
        }
    }

    public async UniTask<T> RunWithSpinnerAsync<T>(UniTask<T> task)
    {
        StartRotation();
        
        try
        {
            return await task;
        }
        finally
        {
            StopRotation();
        }
    }
    
    private void StartRotation()
    {
        _loadingObj.SetActive(true);
        
        _rect.DORotate(new Vector3(0, 0, 360f), _duration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }
    
    private void StopRotation()
    {
        _rect.DOKill();
        _rect.localRotation = Quaternion.identity;
        
        _loadingObj.SetActive(false);
    }
}
