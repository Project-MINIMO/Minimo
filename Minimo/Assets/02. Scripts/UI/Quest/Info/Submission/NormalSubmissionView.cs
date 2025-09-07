using System.Linq;

public class NormalSubmissionView : QuestSubmissionView
{
    public override void Setup(Quest quest)
    {
        base.Setup(quest);

        var i = 0;
        for (; i < quest.Clear.Length; i++)
        {
            SubmissionSlots[i].gameObject.SetActive(true);
            
            var clear = quest.Clear[i];
            SubmissionSlots[i].Initialize(clear.Target, clear.CurrentProgress, clear.Amount);
        }

        for (; i < SubmissionSlots.Length; i++)
        {
            SubmissionSlots[i].gameObject.SetActive(false);
        }

        SubmitBtn.interactable = Quest.Clear.All(clear => clear.IsCompleted);
    }

    protected override void Submit()
    {
        if (Quest.Clear.Any(clear => !clear.IsCompleted)) return;
        
        QuestManager.SubmitQuest();
        base.Submit();
    }
}