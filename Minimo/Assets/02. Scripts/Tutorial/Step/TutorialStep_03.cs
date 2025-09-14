using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialStep_03 : TutorialStep
{
    private const string TopRight = "Walk_TR";
    private const string TopLeft = "Walk_TL";
    private const string BottomRight = "Walk_BR";
    private const string BottomLeft = "Walk_BL";
    private const string Default = "Default";
    
    [SerializeField] private GameObject _passOutMinimo;
    [SerializeField] private TutorialDialogue _dialogueBox;
    [SerializeField] private FocusPanel _focusPanel;

    [SerializeField] private PathManager _pathManager;
    private const float Speed = 0.3f;
    private Vector3 _targetPosition;
    private Vector3 _prevPosition;
    private int _currentIndex;
    
    private Animator _animator;

    private void Start()
    {
        _animator = _passOutMinimo.GetComponentInChildren<Animator>(true);
        _animator.SetBool("IsLay", true);
    }

    protected override void OnStart()
    {
        _passOutMinimo.GetComponent<TutorialInteractable>().onClick += OnClickedChief;
        _dialogueBox.onClick += OnClickedChief;
        _dialogueBox.ShowQuest();
    }
    
    private void OnClickedChief()
    {
        _passOutMinimo.GetComponent<TutorialInteractable>().onClick -= OnClickedChief;
        _dialogueBox.onClick -= OnClickedChief;
        _dialogueBox.Hide();
        
        _focusPanel.FocusOn(_passOutMinimo.transform.position + Vector3.up * 0.5f,
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
        _animator.SetBool("IsLay", false);
        
        yield return new WaitForSeconds(2.3f);
        
        _animator.SetBool("IsTalk", true);
        yield return _dialogueBox.Show(
            "으으.. 어지러워",
            "어라.. 당신은 누구죠..?",
            "아.. 일단은 좀 쉬어야겠어요..");
        _animator.SetBool("IsTalk", false);
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

        _animator.SetBool("IsWalk", true);
        
        while (_currentIndex < path.Count)
        {
            if ((_targetPosition - _passOutMinimo.transform.position).sqrMagnitude > 0f)
            {
                SetAnimationDirection();
                
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
        
        _animator.SetBool("IsWalk", false);
        _animator.SetBool("IsLay", true);
    }

    public override void Cleanup()
    {

    }
    
    private void SetAnimationDirection()
    {
        var deltaX = _targetPosition.x - _passOutMinimo.transform.position.x;
        var deltaY = _targetPosition.y - _passOutMinimo.transform.position.y;

        var trigger = (deltaX, deltaY) switch
        {
            (> 0, > 0) => TopRight,
            (> 0, < 0) => BottomRight,
            (> 0, 0) => BottomRight,

            (< 0, > 0) => TopLeft,
            (< 0, < 0) => BottomLeft,
            (< 0, 0) => BottomLeft,

            (0, > 0) => // 수직 ↑
                AnimatorIsPlaying(BottomLeft) 
                || AnimatorIsPlaying(TopLeft) 
                || AnimatorIsPlaying(Default)
                    ? TopLeft : TopRight,

            (0, < 0) => // 수직 ↓
                AnimatorIsPlaying(BottomLeft) 
                || AnimatorIsPlaying(TopLeft)
                || AnimatorIsPlaying(Default)
                    ? BottomLeft : BottomRight,

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
