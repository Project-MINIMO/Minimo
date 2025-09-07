public class MinimoFSM : FSM<MinimoObject, MinimoState>
{
    public MinimoFSM(MinimoObject owner)
    {
        AddState(MinimoState.None, new MinimoNoneState(owner));
        AddState(MinimoState.Swim, new MinimoSwimState(owner));
        AddState(MinimoState.Happy, new MinimoHappyState(owner));
        AddState(MinimoState.Acquire, new MinimoAcquireState(owner));
        AddState(MinimoState.Idle, new MinimoIdleState(owner));
        AddState(MinimoState.Work, new MinimoWorkState(owner));
    }
}
