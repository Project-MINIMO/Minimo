using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    [SerializeField] private List<TutorialStep> _steps;
    [SerializeField] private GameObject[] _hideUIs;
    [SerializeField] private MinimoSpawner _minimoSpawner;
    
    private int _currentStepIndex = -1;
    private TutorialStep _currentStep;
    public static bool IsTutorialing = true;

    private void Start()
    {
        foreach (var ui in _hideUIs)
        {
            ui.SetActive(false);
        }

        _minimoSpawner.SpawnTutorialMinimo();
        ProceedNextStep();
    }

    public void ProceedNextStep()
    {
        if (_currentStep != null)
            _currentStep.Cleanup();

        _currentStepIndex++;

        if (_currentStepIndex >= _steps.Count)
        {
            IsTutorialing = false;
            foreach (var ui in _hideUIs)
            {
                ui.SetActive(true);
            }
            return;
        }

        _currentStep = _steps[_currentStepIndex];
        _currentStep.StartStep(this);
    }
}
