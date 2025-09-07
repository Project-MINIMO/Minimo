using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGuide : MonoBehaviour
{
    [SerializeField] private GameObject[] _guideObjs;

    [SerializeField] private Button _prevBtn;
    [SerializeField] private Button _nextBtn;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _closeEntireBtn;
    [SerializeField] private TextMeshProUGUI _indexTMP;
    
    private int _currentIndex = 0;
    private int _totalPages;
    
    private float _closeEntireElapsed = 0f;
    private bool _waitingToEnableCloseEntire = false;
    
    private void Awake()
    {
        _totalPages = _guideObjs.Length;

        _prevBtn.onClick.AddListener(Prev);
        _nextBtn.onClick.AddListener(Next);
        _closeBtn.onClick.AddListener(Close);
        _closeEntireBtn.onClick.AddListener(Entire);
    }

    private void OnEnable()
    {
        ShowPage(0);
        
        _closeEntireBtn.enabled = false;
        _closeEntireElapsed = 0f;
        _waitingToEnableCloseEntire = true;
    }
    
    private void Update()
    {
        if (_waitingToEnableCloseEntire)
        {
            _closeEntireElapsed += Time.deltaTime;
            if (_closeEntireElapsed >= 1f)
            {
                _closeEntireBtn.enabled = true;
                _waitingToEnableCloseEntire = false;
            }
        }
    }
    
    private void ShowPage(int index)
    {
        if (index < 0 || index >= _totalPages) return;
        
        _currentIndex = index;

        for (var i = 0; i < _totalPages; i++)
        {
            _guideObjs[i].SetActive(i == _currentIndex);
        }

        _prevBtn.gameObject.SetActive(_currentIndex > 0);
        _nextBtn.gameObject.SetActive(_currentIndex < _totalPages - 1);
        if (_totalPages == 1)
        {
            _indexTMP.gameObject.SetActive(false);
        }

        _indexTMP.text = $"{_currentIndex + 1}/{_totalPages}";
    }
    
    private void Prev()
    {
        ShowPage(_currentIndex - 1);
    }

    private void Next()
    {
        ShowPage(_currentIndex + 1);
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

    private void Entire()
    {
        if (_currentIndex + 1 == _totalPages)
        {
            Close();
        }
        else
        {
            _closeEntireBtn.enabled = false;
            _closeEntireElapsed = 0f;
            _waitingToEnableCloseEntire = true;
            ShowPage(_currentIndex + 1);
        }
    }
}
