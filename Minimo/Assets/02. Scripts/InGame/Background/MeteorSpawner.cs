using System.Collections;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class MeteorSpawner : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] _meteors;
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
        
        var cam = Camera.main;
        var topY = cam.transform.position.y + cam.orthographicSize;
        var spawnY = topY; 
        
        var halfWidth = cam.orthographicSize * cam.aspect;
        var spawnX = Random.Range(-halfWidth, halfWidth) + cam.transform.position.x;
        
        meteor.transform.position = new Vector3(spawnX, spawnY, 10);
        meteor.transform.localScale = _startScale;

        var offsetRatio = Random.Range(0f, 0.5f);
        var bottomY = cam.transform.position.y - cam.orthographicSize;
        var screenHeight = cam.orthographicSize * 2f;
        var endY = bottomY + offsetRatio * screenHeight;

        var fallHeight = spawnY - endY;
        var endX = spawnX - fallHeight * 2f;
        
        var duration = Random.Range(_speedRange.x, _speedRange.y);

        var sequence = DOTween.Sequence();
        sequence.Append(meteor.transform.DOMove(new Vector3(endX, endY, 10), duration).SetEase(Ease.OutQuad))
            .Insert(duration - 0.3f, meteor.transform.DOScale(Vector3.zero, 0.3f))
            .Play();
    }
}