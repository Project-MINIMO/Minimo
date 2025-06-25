using UnityEngine;

public class Minimo : MonoBehaviour
{
    public MinimoFSM FSM { get; private set; }

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        FSM = new MinimoFSM(this);
        SetChillState();
    }

    private void Update()
    {
        FSM.Update();
    }
    
    public void SetSpriteFilp(bool isFlip)
    {
        _spriteRenderer.flipX = isFlip;
    }
    
    public void SetSpriteOrder(int order)
    {
        _spriteRenderer.sortingOrder = order;
    }

    public void SetAnimation(string trigger, bool isActive)
    {
        _animator.SetBool(trigger, isActive);
    }

    public void SetChillState()
    {
        var randomIndex = Random.Range(0, 2);
        
        FSM.ChangeState(randomIndex == 0 ? MinimoState.Idle : MinimoState.Walk);
    }

    public void SetWorkState(ProduceAdvanced produceObject)
    {
        FSM.ChangeState(MinimoState.Work);
        _animator.SetTrigger(produceObject.AnimTrigger);
        transform.SetParent(produceObject.MinimoWorkingPosition);
        transform.localPosition = Vector3.zero;
        SetSpriteFilp(false);
    }
}
