public class VisitMinimoFSM : FSM<VisitMinimoObject, VisitMinimoState>
{
    public VisitMinimoFSM(VisitMinimoObject owner)
    {
        AddState(VisitMinimoState.Idle, new VisitMinimoIdleState(owner));
        AddState(VisitMinimoState.Hide, new VisitMinimoHideState(owner));
        AddState(VisitMinimoState.None, new VisitMinimoNoneState(owner));
    }
}
