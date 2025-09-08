using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StarTestUI : MonoBehaviour
{
    [SerializeField] private Button _toggleBtn;
    [SerializeField] private Button _activeBtn;
    [SerializeField] private Button _resetBtn;

    [SerializeField] private GameObject _buttonBack;
    [SerializeField] private TextMeshProUGUI _activeTMP;

    private bool _isOpen;
    private bool _isActive;

    private const string _activeString = "활성 [O]";
    private const string _inactiveString = "활성 [X]";
    
    private StarManager _starManager;
    
    private void Start()
    {
        _starManager = App.GetManager<StarManager>();
        
        _toggleBtn.onClick.AddListener(Toggle);
        _activeBtn.onClick.AddListener(Active);
        _resetBtn.onClick.AddListener(Reset);
        
        _buttonBack.SetActive(_isOpen);
        _activeTMP.text = _isActive ? _activeString : _inactiveString;
    }

    private void Toggle()
    {
        _isOpen = !_isOpen;
        _buttonBack.SetActive(_isOpen);
    }

    private void Active()
    {
        _isActive = !_isActive;
        _activeTMP.text = _isActive ? _activeString : _inactiveString;
        
        _starManager.ToggleAddStarMode(_isActive);
    }

    private void Reset()
    {
        _starManager.ResetStars();
    }
}
