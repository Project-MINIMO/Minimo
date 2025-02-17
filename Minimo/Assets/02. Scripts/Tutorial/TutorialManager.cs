using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : ManagerBase
{
    [SerializeField] private List<TutorialBase> _tutorials;
    [SerializeField] private Transform[] _circleTransforms;
    [SerializeField] private Transform _endTransform;
    [SerializeField] private Image _lastImg;
    [SerializeField] private TextMeshProUGUI _wishText1;
    [SerializeField] private TextMeshProUGUI _wishText2;
    [SerializeField] private GameObject _notTutorialBack;
    [SerializeField] private CameraInput _cameraInput;

    public string Wish;
    private bool _isZooming = false;
    private DateTime _startTime;
    private TimeManager _timeManager;

    private void Start()
    {
        _timeManager = App.GetManager<TimeManager>();
        
        _startTime = _timeManager.Time;
        
        _tutorials[0].StartTutorial();
    }
    
    private void Update()
    {
        if (_isZooming)
        {
            // 카메라 위치 이동
            Vector3 targetPosition = new Vector3(_endTransform.position.x, _endTransform.position.y, -10);
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPosition, 3 * Time.deltaTime);

            // 카메라 줌인 (Orthographic 기준)
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 5, 2 * Time.deltaTime);

            // 줌과 이동 완료 여부 확인
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f &&
                Mathf.Abs(Camera.main.orthographicSize - 5) < 0.1f)
            {
                _isZooming = false; // 줌인 완료
            }
        }
    }
    
    public void NextTutorial()
    {
        _tutorials[0].EndTutorial();
        _tutorials.RemoveAt(0);

        StartCoroutine(NextTutorialCoroutine());
    }
    
    private IEnumerator NextTutorialCoroutine()
    {
        if (_timeManager.Time - _startTime < TimeSpan.FromSeconds(1))
            yield return null;
        else
            yield return new WaitForSeconds(0.5f);
        
        if (_tutorials.Count > 0)
        {
            _tutorials[0].StartTutorial();
            _startTime = _timeManager.Time;
        }
        else
        {
            StartCoroutine(EndTutorial());
        }
    }

    private IEnumerator EndTutorial()
    {
        _isZooming = true;
        _notTutorialBack.SetActive(false);
        _cameraInput.enabled = false;
        _wishText1.text = $"{Wish}....";
        foreach (var circle in _circleTransforms)
        {
            circle.DOMove(_endTransform.position, 1f);

            yield return new WaitForSeconds(0.3f);
        }
        
        var sequence = DOTween.Sequence();
        sequence.Append(_lastImg.DOFade(1, 1f))
            .AppendInterval(1f)
            .Append(_wishText1.DOFade(1, 1f))
            .Append(_wishText2.DOFade(1, 1))
            .AppendInterval(2f)
            .OnComplete(() =>
            {
                App.GetManager<UIManager>().FadeIn(() =>
                {
                    App.GetManager<LoginManager>().Logout();
                    App.LoadScene(SceneName.Title);
                });
            });
    }
}
