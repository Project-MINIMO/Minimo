public class StrayMinimoFSM : FSM<StrayMinimoObject, StrayMinimoState>
{
    public StrayMinimoFSM(StrayMinimoObject owner)
    {
        AddState(StrayMinimoState.Idle, new StrayMinimoIdleState(owner));
        AddState(StrayMinimoState.Plunder, new StrayMinimoPlunderState(owner));
        AddState(StrayMinimoState.Run, new StrayMinimoRunState(owner));
        AddState(StrayMinimoState.Hide, new StrayMinimoHideState(owner));
    }
}
