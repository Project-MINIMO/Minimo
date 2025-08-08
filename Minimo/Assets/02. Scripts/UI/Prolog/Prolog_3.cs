using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Prolog_3 : PrologBase
{
    [SerializeField] private Image _background;
    [SerializeField] private Image _textBox;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private string[] _strings;
    [SerializeField] private float _charInterval = 0.05f;
    [SerializeField] private Image[] _pages;
    [SerializeField] private GameObject _textSkipObj;
    
    protected override IEnumerator ShowProlog()
    {
        FadeIn(_background, 1f);
        yield return new WaitForSeconds(3f);

        foreach (var page in _pages)
        {
            FadeIn(page, 0.5f);
            yield return new WaitForSeconds(1f);
        }
        
        FadeIn(_textBox, 0.3f);
        yield return new WaitForSeconds(0.3f);
        
        foreach (var line in _strings)
        {
            _text.text = string.Empty;
            _text.DOKill();

            var duration = line.Length * _charInterval;
            _text.DOText(line, duration).SetEase(Ease.Linear);

            yield return new WaitForSeconds(duration);
            _textSkipObj.SetActive(true);
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            _textSkipObj.SetActive(false);
        }
        
        EndProlog();
    }
}
