public class MinimoFSM : FSM<MinimoObject, MinimoState>
{
    public MinimoFSM(MinimoObject owner)
    {
        AddState(MinimoState.Idle, new MinimoIdleState(owner));
        AddState(MinimoState.Work, new MinimoWorkState(owner));
        AddState(MinimoState.Hide, new MinimoHideState(owner));
        AddState(MinimoState.Assign, new MinimoAssignState(owner));
    }
}
