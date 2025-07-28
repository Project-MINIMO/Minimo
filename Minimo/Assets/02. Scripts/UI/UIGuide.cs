using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGuide : MonoBehaviour
{
    [SerializeField] private GameObject[] _guideObjs;

    [SerializeField] private Button _prevBtn;
    [SerializeField] private Button _nextBtn;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private TextMeshProUGUI _indexTMP;
    
    private int _currentIndex = 0;
    private int _totalPages;
    
    private void Awake()
    {
        _totalPages = _guideObjs.Length;

        _prevBtn.onClick.AddListener(Prev);
        _nextBtn.onClick.AddListener(Next);
        _closeBtn.onClick.AddListener(Close);
    }

    private void OnEnable()
    {
        ShowPage(0);
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
}
