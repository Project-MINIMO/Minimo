using System.Collections;

using UnityEngine;
using TMPro;
using DG.Tweening;

public class TutorialDialogue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private GameObject _panel;
    [SerializeField] private float _charInterval = 0.05f;
    
    private ConstellationPanel _constellationPanel;

    private void Awake()
    {
        _constellationPanel = App.GetManager<UIManager>().GetPanel<ConstellationPanel>();
    }

    public IEnumerator Show(params string[] lines)
    {
        _panel.SetActive(true);
        _constellationPanel.OpenPanel();

        foreach (var line in lines)
        {
            _text.text = string.Empty;
            _text.DOKill();

            var duration = line.Length * _charInterval;
            _text.DOText(line, duration).SetEase(Ease.Linear);

            yield return new WaitForSeconds(duration);
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }

        _constellationPanel.ClosePanel();
        _panel.SetActive(false);
    }
}