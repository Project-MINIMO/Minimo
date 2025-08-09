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
        _dialogueBox.Show("이제 백미로 도정해서 배고픈 미니모에게 갖다주게!");
        _visitMinimo = _spawner.SpawnTutoriMinimo();

        StartCoroutine(ProgressQuest());
    }

    private IEnumerator ProgressQuest()
    {
        yield return new WaitUntil(() => _visitMinimo.CurrentState == VisitMinimoState.Hide);
        
        _dialogueBox.Show("잘했네! 당분간 굶어 죽을 일은 없겠구만.");

        yield return new WaitForSeconds(3);

        _dialogueBox.Hide();
        CompleteStep();
    }
  
    public override void Cleanup()
    {
        AccountInfo.Instance.Level.AddCount(100);
    }
}