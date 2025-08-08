using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialStep_06 : TutorialStep
{
    [SerializeField] private GameObject _chiefMinimo;
    [SerializeField] private TutorialDialogue _dialogueBox;
    [SerializeField] private FocusPanel _focusPanel;
    [SerializeField] private GameObject _chiefMinimoTextObj;
    [SerializeField] private TextMeshProUGUI _chiefMinimoText;
    
    [SerializeField] private PathManager _pathManager;
    private const float Speed = 0.3f;
    private Vector3 _targetPosition;
    private Vector3 _prevPosition;
    private int _currentIndex;
    
    private int _dialogueIndex = 0;
    private Coroutine _dialogueCoroutine;
    private Coroutine _moveCoroutine;
    
    private void Awake()
    {
        _chiefMinimoTextObj.SetActive(false);
    }

    public void MoveChief()
    {
        _moveCoroutine = StartCoroutine(MoveChiefRoutine());
    }

    private IEnumerator MoveChiefRoutine()
    {
        var path = _pathManager.GetPath(
            _chiefMinimo.transform.position,
            _pathManager.GetTileWorldPosition(new Vector3Int(0, -4, 0))
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
    
    protected override void OnStart()
    {
        _chiefMinimo.GetComponent<TutorialInteractable>().onClick += OnClickedChief;
        _chiefMinimoTextObj.SetActive(true);
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
                    _chiefMinimoText.text = "어라?";
                    break;
                case 1:
                    _chiefMinimoText.text = "어디갔지?";
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
                _pathManager.GetTileWorldPosition(_dialogueIndex == 0 
                    ? new Vector3Int(0, -4, 0) 
                    : new Vector3Int(1, -5, 0))
            );
            
            _currentIndex = 0;
            _targetPosition = _pathManager.GetTileWorldPosition(path[_currentIndex]);
            _prevPosition = _chiefMinimo.transform.position;
            
            _chiefMinimoText.text = "......";
            
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
            duration: 2,
            onComplete: () =>
            {
                StartCoroutine(ConversationSequence());
            },
            closeOnComplete: false);
    }
    
    private IEnumerator ConversationSequence()
    {
        _chiefMinimoTextObj.SetActive(false);
        
        yield return _dialogueBox.Show(
            "소원식(*음식)이 사라졌어",
            "소원식을 훔쳐가는 해적 미니모가 존재해",
            "소원식을 훔쳐갔나봐",
            "일단 소원식을 만들어야해",
            "밭에서 별곡을 심어줄래?");
        
        
        _focusPanel.FocusOn(Vector3.down,
            targetZoom: 4,
            duration: 2,
            onComplete: CompleteStep,
            closeOnComplete: true);
    }
  
    public override void Cleanup()
    {

    }
}
