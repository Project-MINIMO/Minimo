using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    [SerializeField] private List<TutorialStep> _steps;
    [SerializeField] private GameObject[] _hideUIs;
    [SerializeField] private MinimoSpawner _minimoSpawner;
    [SerializeField] private GameObject _mainGuide;
    
    private int _currentStepIndex = -1;
    private TutorialStep _currentStep;
    public static bool IsTutorialing = false;
    public static bool IsUseInput = false;

    protected override void Awake()
    {
        base.Awake();
        
        if (AccountInfo.Instance.Tutorial)
        {
            IsTutorialing = true;
            foreach (var ui in _hideUIs)
            {
                ui.SetActive(false);
            }
        }
        else
        {
            _mainGuide.SetActive(true);
        }
    }

    private void Start()
    {
        if (AccountInfo.Instance.Tutorial)
        {
            _minimoSpawner.SpawnTutorialMinimo();
            ProceedNextStep();
        }
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
            _mainGuide.SetActive(true);
            return;
        }

        _currentStep = _steps[_currentStepIndex];
        _currentStep.StartStep(this);
    }
}
