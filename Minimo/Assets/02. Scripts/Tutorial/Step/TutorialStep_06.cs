using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialStep_06 : TutorialStep
{
    [SerializeField] private GameObject _chiefMinimo;
    [SerializeField] private TutorialDialogue _dialogueBox;
    [SerializeField] private FocusPanel _focusPanel;
    
    [SerializeField] private PathManager _pathManager;
    private const float Speed = 0.3f;
    private Vector3 _targetPosition;
    private Vector3 _prevPosition;
    private int _currentIndex;
    
    private int _dialogueIndex = 0;
    private Coroutine _dialogueCoroutine;
    private Coroutine _moveCoroutine;

    public void MoveChief()
    {
        _moveCoroutine = StartCoroutine(MoveChiefRoutine());
    }

    private IEnumerator MoveChiefRoutine()
    {
        var path = _pathManager.GetPath(
            _chiefMinimo.transform.position,
            new Vector3(1.5f, -0.5f, 0)
        );
            
        _currentIndex = 0;
        _targetPosition = _pathManager.GetTileWorldPosition(path[_currentIndex]);
        _prevPosition = _chiefMinimo.transform.position;
        
        while (_currentIndex < path.Count)
        {
            if ((_targetPosition - _chiefMinimo.transform.position).sqrMagnitude > 0f)
            {
                _chiefMinimo.transform.position = Vector3.MoveTowards(
                    _chiefMinimo.transform.position,
                    _targetPosition,
                    2 * Speed * Time.deltaTime
                );
                    
                if (_prevPosition == _chiefMinimo.transform.position)
                    yield break;

                _prevPosition = _chiefMinimo.transform.position;
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
    
    protected override void OnStart()
    {
        _chiefMinimo.GetComponent<TutorialInteractable>().onClick += OnClickedChief;
        StopCoroutine(_moveCoroutine);
        _dialogueCoroutine = StartCoroutine(ShowDialogueSequence());
    }
    
    private IEnumerator ShowDialogueSequence()
    {
        while (true)
        {
            switch (_dialogueIndex)
            {
                case 0:
                    _dialogueBox.Show("어라?");
                    break;
                case 1:
                    _dialogueBox.Show("어디갔지?");
                    break;
            }

            // 대사 표시 후 기다림
            yield return new WaitForSeconds(1);
            
            _dialogueIndex++;
            if (_dialogueIndex > 1) 
            {
                _dialogueIndex = 0;
            }
            
            var path = _pathManager.GetPath(
                _chiefMinimo.transform.position,
                _dialogueIndex == 0 ? new Vector3(1.5f, -0.5f, 0) : new Vector3(2.5f, -0.5f, 0)
            );
            
            _currentIndex = 0;
            _targetPosition = _pathManager.GetTileWorldPosition(path[_currentIndex]);
            _prevPosition = _chiefMinimo.transform.position;
            
            _dialogueBox.Show("...");
            
            while (_currentIndex < path.Count)
            {
                if ((_targetPosition - _chiefMinimo.transform.position).sqrMagnitude > 0f)
                {
                    _chiefMinimo.transform.position = Vector3.MoveTowards(
                        _chiefMinimo.transform.position,
                        _targetPosition,
                        Speed * Time.deltaTime
                    );
                    
                    if (_prevPosition == _chiefMinimo.transform.position)
                        yield break;

                    _prevPosition = _chiefMinimo.transform.position;
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
    }
    
    private void OnClickedChief()
    {
        _chiefMinimo.GetComponent<TutorialInteractable>().onClick -= OnClickedChief;
        StopCoroutine(_dialogueCoroutine);
        _focusPanel.FocusOn(_chiefMinimo.transform.position,
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
            "방금 수확한 별곡이 사라졌네.",
            "대체 어디로 갔는지 모르겠군.",
            "어쩌면 해적들이 훔쳐갔을 지도 모르네!",
            "(입가에 곡물 가루같은 것이 묻어있다.)",
            "수고스럽겠지만... 밭에 별곡을 더 심어줄 수 있겠나?");
        
        
        _focusPanel.FocusOn(Vector3.down,
            targetZoom: 4,
            duration: 1.5f,
            onComplete: CompleteStep,
            closeOnComplete: true);
    }
  
    public override void Cleanup()
    {

    }
}
