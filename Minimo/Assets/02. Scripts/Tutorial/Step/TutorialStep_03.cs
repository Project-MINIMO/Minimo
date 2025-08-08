using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialStep_03 : TutorialStep
{
    [SerializeField] private GameObject _passOutMinimo;
    [SerializeField] private TutorialDialogue _dialogueBox;
    [SerializeField] private FocusPanel _focusPanel;

    [SerializeField] private PathManager _pathManager;
    private const float Speed = 0.3f;
    private Vector3 _targetPosition;
    private Vector3 _prevPosition;
    private int _currentIndex;

    protected override void OnStart()
    {
        _passOutMinimo.GetComponent<TutorialInteractable>().onClick += OnClickedChief;
        _dialogueBox.ShowQuest();
    }
    
    private void OnClickedChief()
    {
        _passOutMinimo.GetComponent<TutorialInteractable>().onClick -= OnClickedChief;
        
        _focusPanel.FocusOn(_passOutMinimo.transform.position,
            targetZoom: 1,
            duration: 1.5f,
            onComplete: () =>
            {
                StartCoroutine(ConversationSequence());
            },
            closeOnComplete: false);
    }

    private IEnumerator ConversationSequence()
    {
        _dialogueBox.Hide();
        
        yield return _dialogueBox.Show(
            "으으.. 어지러워",
            "어라.. 당신은 누구죠..?",
            "아.. 일단은 좀 쉬어야겠어요..");
        
        _focusPanel.FocusOn(Vector3.zero,
            targetZoom: 4,
            duration: 1.5f,
            onComplete: () =>
            {
                Cleanup();
                CompleteStep();
            },
            closeOnComplete: true);
        
        var path = _pathManager.GetPath(
            _passOutMinimo.transform.position, 
            _pathManager.GetTileWorldPosition(new Vector3Int(-1, -5, 0))
        );
        
        _currentIndex = 0;
        _targetPosition = _pathManager.GetTileWorldPosition(path[_currentIndex]);
        _prevPosition = _passOutMinimo.transform.position;

        while (_currentIndex < path.Count)
        {
            if ((_targetPosition - _passOutMinimo.transform.position).sqrMagnitude > 0f)
            {
               _passOutMinimo.transform.position = Vector3.MoveTowards(
                    _passOutMinimo.transform.position, 
                    _targetPosition, 
                    Speed * Time.deltaTime);

                if (_prevPosition == _passOutMinimo.transform.position)
                {
                    yield break;
                }
                _prevPosition = _passOutMinimo.transform.position;
            }
            else
            {
                if (++_currentIndex < path.Count)
                {
                    _targetPosition = _pathManager.GetTileWorldPosition(path[_currentIndex]);
                }
            }

            yield return null;
        }
    }

    public override void Cleanup()
    {

    }
}
