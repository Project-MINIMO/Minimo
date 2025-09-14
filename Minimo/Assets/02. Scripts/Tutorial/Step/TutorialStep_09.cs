using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialStep_09 : TutorialStep
{
    [SerializeField] private GameObject _chiefMinimo;
    [SerializeField] private TutorialInteractable _hungryMinimo;
    [SerializeField] private TutorialDialogue _dialogueBox;
    [SerializeField] private Button _hungryBtn;

    private bool _isGiveItem;
    private Item _requiredItem;
    
    protected override void OnStart()
    {
        _dialogueBox.Show("이제 백미로 도정해서 배고픈 미니모에게 갖다주게!");
        _hungryBtn.onClick.AddListener(GiveItem);
        _hungryBtn.gameObject.SetActive(true);
        _hungryMinimo.onClick += GiveItem;
        _requiredItem = AccountInfo.Instance.Items[6];
        StartCoroutine(ProgressQuest());
    }

    private IEnumerator ProgressQuest()
    {
        yield return new WaitUntil(() => _isGiveItem);
        
        _dialogueBox.Show("잘했네! 당분간 굶어 죽을 일은 없겠구만.");

        yield return new WaitForSeconds(5);

        _dialogueBox.Hide();
        CompleteStep();
    }
  
    public override void Cleanup()
    {
        AccountInfo.Instance.Level.AddCount(100);
    }

    private void GiveItem()
    {
        if (_requiredItem.Count >= 1)
        {
            _requiredItem.AddCount(-1);
            AccountInfo.Instance.Level.AddCount(_requiredItem.Exp * 5);
            AccountInfo.Instance.Gold.AddCount(_requiredItem.SellCost * 5);
            _hungryBtn.gameObject.SetActive(false);
            _hungryMinimo.onClick -= GiveItem;
            _isGiveItem = true;
        }
        else
        {
            App.Notification(NotifyType.ItemLack);
        }
    }
}