using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialStep_09 : TutorialStep
{
    [SerializeField] private GameObject _chiefMinimo;
    [SerializeField] private GameObject _chiefMinimoTextObj;
    [SerializeField] private TextMeshProUGUI _chiefMinimoText;
    [SerializeField] private VisitMinimoSpawner _spawner;

    private VisitMinimoObject _visitMinimo;
    
    protected override void OnStart()
    {
        _chiefMinimoTextObj.SetActive(true);
        _chiefMinimoText.text = "미니모들에게 소원식을 만들어줄래?";
        _visitMinimo = _spawner.SpawnTutoriMinimo();

        StartCoroutine(ProgressQuest());
    }

    private IEnumerator ProgressQuest()
    {
        yield return new WaitUntil(() => _visitMinimo.CurrentState == VisitMinimoState.Hide);
        
        _chiefMinimoText.text = "고마워";

        yield return new WaitForSeconds(1);

        _chiefMinimoTextObj.SetActive(false);
        AccountInfo.Instance.Level.AddCount(200);
        CompleteStep();
    }
  
    public override void Cleanup()
    {

    }
}