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
        _chiefMinimoText.text = "이제 백미로 도정하면 되네!";
        _visitMinimo = _spawner.SpawnTutoriMinimo();

        StartCoroutine(ProgressQuest());
    }

    private IEnumerator ProgressQuest()
    {
        yield return new WaitUntil(() => _visitMinimo.CurrentState == VisitMinimoState.Hide);
        
        _chiefMinimoText.text = "고맙네";

        yield return new WaitForSeconds(1);

        _chiefMinimoTextObj.SetActive(false);
        AccountInfo.Instance.Level.AddCount(200);
        CompleteStep();
    }
  
    public override void Cleanup()
    {

    }
}