using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleTransition : MonoBehaviour
{
    [SerializeField] private Color _onColor = Color.black;
    [SerializeField] private Color _offColor = Color.white;
    
    [SerializeField] private Sprite _onSprite;
    [SerializeField] private Sprite _offSprite;

    private Toggle _toggle;
    private Image _image;
    private TextMeshProUGUI _text;
    
    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
        _image = GetComponentInChildren<Image>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
        
        _toggle.onValueChanged.AddListener(UpdateTransition);
        UpdateTransition(_toggle.isOn);
    }

    private void UpdateTransition(bool isOn)
    {
        _text.color = isOn ? _onColor : _offColor;
        _image.sprite = isOn ? _onSprite : _offSprite;
    }
}
