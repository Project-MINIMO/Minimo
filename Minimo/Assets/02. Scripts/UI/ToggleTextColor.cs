using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleTextColor : MonoBehaviour
{
    [SerializeField] private Color _onColor = Color.black;
    [SerializeField] private Color _offColor = Color.white;

    private Toggle _toggle;
    private TextMeshProUGUI _text;
    
    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
        
        _toggle.onValueChanged.AddListener(UpdateTextColor);
        UpdateTextColor(_toggle.isOn);
    }

    private void UpdateTextColor(bool isOn)
    {
        _text.color = isOn ? _onColor : _offColor;
    }
}
