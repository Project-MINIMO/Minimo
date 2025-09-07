using System.Collections;

using UnityEngine;
using TMPro;
using DG.Tweening;

public class TutorialDialogue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _text2;
    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject _questPanel;
    [SerializeField] private float _charInterval = 0.05f;
    
    private void Awake()
    {
        _panel.SetActive(false);
        _questPanel.SetActive(false);
    }

    public void ShowQuest()
    {
        _questPanel.SetActive(true);
    }

    public void Show(string text)
    {
        _panel.SetActive(true);
        _text.text = string.Empty;
        _text2.text = string.Empty;
        var duration = text.Length * _charInterval;
        _text.DOText(text, duration).SetEase(Ease.Linear);
        _text2.DOText(text, duration).SetEase(Ease.Linear);
    }

    public IEnumerator Show(params string[] lines)
    {
        _panel.SetActive(true);

        foreach (var line in lines)
        {
            _text.text = string.Empty;
            _text2.text = string.Empty;
            _text.DOKill();
            _text2.DOKill();

            var duration = line.Length * _charInterval;
            _text.DOText(line, duration).SetEase(Ease.Linear);
            _text2.DOText(line, duration).SetEase(Ease.Linear);

            yield return new WaitForSeconds(duration);
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }
        
        _panel.SetActive(false);
    }

    public void Hide()
    {
        _panel.SetActive(false);
        _questPanel.SetActive(false);
    }
}