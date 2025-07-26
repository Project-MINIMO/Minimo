using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class QuestInfoPanel : UIBase
{
    public override bool IsUseBlur => true;
    
    [SerializeField] private Button _closeBtn;
    [SerializeField] private QuestTransitioner _transitioner;
    [SerializeField] protected QuestInfoUpdater _infoUpdater;
    
    protected QuestManager QuestManager;
    protected Quest SelectedQuest;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        
        QuestManager = App.GetManager<QuestManager>();
        QuestManager.CurrentQuest
            .Subscribe(quest =>
            {
                if (quest != null) SelectedQuest = quest;
            })
            .AddTo(this);
        
        _closeBtn.onClick.AddListener(ClosePanel);
    }

    public override void Show(bool isNew)
    {
        base.Show(isNew);

        _transitioner.Open(isNew);
    }
}
