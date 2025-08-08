using UnityEngine;

public abstract class TutorialStep : MonoBehaviour
{
    protected TutorialManager _manager;

    public void StartStep(TutorialManager manager)
    {
        _manager = manager;
        OnStart();
    }

    protected abstract void OnStart();

    public abstract void Cleanup();

    protected void CompleteStep()
    {
        _manager.ProceedNextStep();
        AccountInfo.Instance.Gold.AddCount(100);
    }
}
