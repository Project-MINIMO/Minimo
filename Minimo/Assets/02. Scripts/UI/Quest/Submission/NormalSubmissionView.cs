using System.Linq;

public class NormalSubmissionView : QuestSubmissionView
{
    public override void Setup(Quest quest)
    {
        base.Setup(quest);

        var i = 0;
        for (; i < quest.Clear.Length; i++)
        {
            _infoUpdaters[i].gameObject.SetActive(true);
            
            var clear = quest.Clear[i];
            var icon = clear.Target.Icon;
            var progress = $"{clear.CurrentProgress} / {clear.Amount}";
            _infoUpdaters[i].UpdateItem(icon, progress);
        }

        for (; i < _infoUpdaters.Length; i++)
        {
            _infoUpdaters[i].gameObject.SetActive(false);
        }
    }

    protected override void Submit()
    {
        if (Quest.Clear.Any(clear => !clear.IsCompleted)) return;
        
        QuestManager.SubmitQuest();
    }
}