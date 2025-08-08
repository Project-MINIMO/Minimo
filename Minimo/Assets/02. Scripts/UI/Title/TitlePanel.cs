using System;
using System.Collections;
using System.Linq;
using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Random = UnityEngine.Random;

public class TitlePanel : MonoBehaviour
{
    [SerializeField] private Button _startBtn;
    [SerializeField] private RectTransform _startTextRect;
    [SerializeField] private RectTransform[] _stars;
    [SerializeField] private Vector2 _width;
    [SerializeField] private float _height;
    [SerializeField] private Vector2 _diff;
    [SerializeField] private TitleLoadHandler _loadHandler;
    [SerializeField] private Vector2 _spawnInterval;
    
    private Coroutine[] _starCoroutines;
    
    [SerializeField] private UILongPressDetector _tutorial;
    [SerializeField] private UILongPressDetector _prolog;
     
    private void Awake()
    {
        _startBtn.onClick.AddListener(OnClickStart);
        var startPositionY = _startTextRect.anchoredPosition.y;
        _startTextRect.DOAnchorPosY(startPositionY + 10f, 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear);
        
        _starCoroutines = new Coroutine[_stars.Length];
        for (var i = 0; i < _stars.Length; i++)
        {
            _starCoroutines[i] = StartCoroutine(SpawnStarRoutine(i));
        }

        _tutorial.OnLongPress += () =>
        {
            AccountInfo.Instance.Tutorial = false;
            App.Notification(NotifyType.TutorialSkip);
        };
        _prolog.OnLongPress += () =>
        {
            AccountInfo.Instance.Prolog = false;
            App.Notification(NotifyType.PrologSkip);
        };
    }

    public void ShowTitle(bool isNew = false)
    {
        _loadHandler.Setup(10);
      
        _loadHandler.FinishLoad();
        _startBtn.gameObject.SetActive(true);
    }

    private IEnumerator SpawnStarRoutine(int index)
    {
        var star = _stars[index];
        while (true)
        {
            var delay = Random.Range(_spawnInterval.x, _spawnInterval.y);
            yield return new WaitForSeconds(delay);
            StarAnimation(star);
        }
    }

    private void OnClickStart()
    {
        StartCoroutine(StopStarRoutine());

        _startTextRect.DOKill();
        _startBtn.gameObject.SetActive(false);
        App.LoadScene(AccountInfo.Instance.Prolog ? SceneName.Prolog : SceneName.Game);
    }

    private IEnumerator StopStarRoutine()
    {
        yield return new WaitForSeconds(1.99f);

        foreach (var star in _stars) star.DOKill();
        foreach (var t in _starCoroutines) StopCoroutine(t);
    }

    private void StarAnimation(RectTransform _rect)
    {
        _rect.DOKill();
        _rect.anchoredPosition = new Vector2(Random.Range(_width.x, _width.y), _height);
        _rect.localScale = new Vector3(0.5f, 0.5f, 1);
        var randomDiff = Random.Range(_diff.x, _diff.y);
        var randomSpeeed = Random.Range(0.25f, 0.75f);
        
        var sequence = DOTween.Sequence();
        sequence.Append(_rect.DOAnchorPos(
                new Vector2(_rect.anchoredPosition.x - randomDiff, _rect.anchoredPosition.y - randomDiff / 2), randomSpeeed))
            .Insert(randomSpeeed - 0.3f, _rect.DOScale(Vector3.zero, 0.3f))
            .Play();
    }
}
