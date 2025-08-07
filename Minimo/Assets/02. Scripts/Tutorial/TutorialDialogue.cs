using System.Collections;

using UnityEngine;
using TMPro;
using DG.Tweening;

public class TutorialDialogue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private GameObject _panel;
    [SerializeField] private float _charInterval = 0.05f;

    public IEnumerator Show(params string[] lines)
    {
        _panel.SetActive(true);

        foreach (var line in lines)
        {
            _text.text = string.Empty;
            _text.maxVisibleCharacters = 0;
            _text.DOKill();
            
            var tween = _text.DOText(line, line.Length * _charInterval)
                .SetEase(Ease.Linear);

            yield return tween.WaitForCompletion();
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }

        _panel.SetActive(false);
    }
}