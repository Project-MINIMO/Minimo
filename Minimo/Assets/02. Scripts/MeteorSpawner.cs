using System.Collections;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class MeteorSpawner : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] _meteors;
    [SerializeField] private Vector2 _startXRange = new(-5f, 9f);
    [SerializeField] private float _startY = 4f;
    [SerializeField] private Vector2 _endOffsetRange = new(5f, 11.6f);
    [SerializeField] private Vector2 _spawnInterval = new(3f, 10f);
    [SerializeField] private Vector2 _speedRange = new(0.5f, 1f);

    private Coroutine[] _coroutines;
    private readonly Vector3 _startScale = new(0.2f, 0.2f, 0.2f);

    private void Start()
    {
        _coroutines = new Coroutine[_meteors.Length];
        for (var i = 0; i < _meteors.Length; i++)
        {
            _coroutines[i] = StartCoroutine(SpawnMeteorRoutine(_meteors[i]));
        }
    }

    private IEnumerator SpawnMeteorRoutine(SpriteRenderer meteor)
    {
        while (true)
        {
            var delay = Random.Range(_spawnInterval.x, _spawnInterval.y);
            yield return new WaitForSeconds(delay);

            SpawnMeteor(meteor);
        }
    }

    private void SpawnMeteor(SpriteRenderer meteor)
    {
        meteor.transform.DOKill();
        
        var startX = Random.Range(_startXRange.x, _startXRange.y);
        var startY = _startY;
        meteor.transform.localPosition = new Vector3(startX, startY, 10);
        meteor.transform.localScale = _startScale;
        
        var randomOffset = Random.Range(_endOffsetRange.x, _endOffsetRange.y);
        var endX = startX - randomOffset;
        var endY = startY - randomOffset / 2;
        var duration = Random.Range(_speedRange.x, _speedRange.y);
        
        var sequence = DOTween.Sequence();
        sequence.Append(meteor.transform.DOLocalMove(new Vector3(endX, endY, 10), duration).SetEase(Ease.OutQuad))
            .Insert(duration - 0.3f, meteor.transform.DOScale(Vector3.zero, 0.3f))
            .Play();
    }
}