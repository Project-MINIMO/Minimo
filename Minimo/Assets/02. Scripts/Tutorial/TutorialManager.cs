using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    [SerializeField] private List<TutorialStep> _steps;

    private int _currentStepIndex = -1;
    private TutorialStep _currentStep;

    private void Start()
    {
        ProceedNextStep();
    }

    public void ProceedNextStep()
    {
        if (_currentStep != null)
            _currentStep.Cleanup();

        _currentStepIndex++;

        if (_currentStepIndex >= _steps.Count)
        {
            Debug.Log("Tutorial Completed");
            return;
        }

        _currentStep = _steps[_currentStepIndex];
        _currentStep.StartStep(this);
    }
}
