using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialStep_06 : TutorialStep
{
    private const string TopRight = "Walk_TR";
    private const string TopLeft = "Walk_TL";
    private const string BottomRight = "Walk_BR";
    private const string BottomLeft = "Walk_BL";
    private const string Default = "Default";
    
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
    private Animator _animator;
    
    private int _lastXSign = 1;

    private void Start()
    {
        _animator = _chiefMinimo.GetComponentInChildren<Animator>(true);
    }
    
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
        
        _animator.SetBool("IsWalk", true);
        _animator.speed = 3;
        
        while (_currentIndex < path.Count)
        {
            if ((_targetPosition - _chiefMinimo.transform.position).sqrMagnitude > 0f)
            {
                SetAnimationDirection();
                
                _chiefMinimo.transform.position = Vector3.MoveTowards(
                    _chiefMinimo.transform.position,
                    _targetPosition,
                    3 * Speed * Time.deltaTime
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
        
        _animator.SetBool("IsWalk", false);
        _animator.speed = 1;
    }
    
    protected override void OnStart()
    {
        _chiefMinimo.GetComponent<TutorialInteractable>().onClick += OnClickedChief;
        _dialogueBox.onClick += OnClickedChief;
        StopCoroutine(_moveCoroutine);
        _animator.SetBool("IsWalk", false);
        _animator.speed = 1;
        _dialogueCoroutine = StartCoroutine(ShowDialogueSequence());
    }
    
    private IEnumerator ShowDialogueSequence()
    {
        while (true)
        {
            _dialogueBox.Hide();
            
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
            
            _dialogueBox.Hide();
            _dialogueBox.ShowQuest();
            
            _animator.SetBool("IsWalk", true);
            
            while (_currentIndex < path.Count)
            {
                if ((_targetPosition - _chiefMinimo.transform.position).sqrMagnitude > 0f)
                {
                    SetAnimationDirection();
                    
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
            
            _animator.SetBool("IsWalk", false);
        }
    }
    
    private void OnClickedChief()
    {
        _chiefMinimo.GetComponent<TutorialInteractable>().onClick -= OnClickedChief;
        _dialogueBox.onClick -= OnClickedChief;
        StopCoroutine(_dialogueCoroutine);
        _dialogueBox.Hide();
        _animator.SetBool("IsWalk", false);
        _focusPanel.FocusOn(_chiefMinimo.transform.position + Vector3.up * 0.5f,
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
        yield return _dialogueBox.Show(
            "방금 수확한 별곡이 사라졌다네.",
            "대체 어디로 갔는지 모르겠군.",
            "어쩌면 해적들이 훔쳐갔을 지도 모르네!",
            "(입가에 곡물 가루같은 것이 묻어있다.)",
            "수고스럽겠지만... 밭에 별곡을 더 심어줄 수 있겠나?",
            "별곡들을 수확해서 나에게 갖다주게.");
        
        
        _focusPanel.FocusOn(Vector3.down,
            targetZoom: 4,
            duration: 1.5f,
            onComplete: CompleteStep,
            closeOnComplete: true);
    }
  
    public override void Cleanup()
    {

    }
    
    private void SetAnimationDirection()
    {
        var deltaX = _targetPosition.x - _chiefMinimo.transform.position.x;
        var deltaY = _targetPosition.y - _chiefMinimo.transform.position.y;

        if (deltaX > 0f) _lastXSign = 1;
        else if (deltaX < 0f) _lastXSign = -1;
        
        var trigger = (deltaX, deltaY) switch
        {
            (> 0, > 0) => TopRight,
            (> 0, < 0) => BottomRight,
            (> 0, 0) => BottomRight,

            (< 0, > 0) => TopLeft,
            (< 0, < 0) => BottomLeft,
            (< 0, 0) => BottomLeft,

            (0, > 0) => _lastXSign > 0 ? TopRight  : TopLeft,    // ↑
            (0, < 0) => _lastXSign > 0 ? BottomRight : BottomLeft, // ↓

            (0, 0) => TopRight,
            _ => TopRight
        };
                
        _animator.SetTrigger(trigger);
    }
    
    private bool AnimatorIsPlaying(string stateName)
    {
        return _animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
}
