using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialStep_09 : TutorialStep
{
    [SerializeField] private GameObject _chiefMinimo;
    [SerializeField] private VisitMinimoSpawner _spawner;
    [SerializeField] private TutorialDialogue _dialogueBox;

    private VisitMinimoObject _visitMinimo;
    
    protected override void OnStart()
    {
        _dialogueBox.Show("이제 백미로 도정하면 되네!");
        _visitMinimo = _spawner.SpawnTutoriMinimo();

        StartCoroutine(ProgressQuest());
    }

    private IEnumerator ProgressQuest()
    {
        yield return new WaitUntil(() => _visitMinimo.CurrentState == VisitMinimoState.Hide);
        
        _dialogueBox.Show("고맙네");

        yield return new WaitForSeconds(1);

        _dialogueBox.Hide();
        AccountInfo.Instance.Level.AddCount(200);
        CompleteStep();
    }
  
    public override void Cleanup()
    {

    }
}