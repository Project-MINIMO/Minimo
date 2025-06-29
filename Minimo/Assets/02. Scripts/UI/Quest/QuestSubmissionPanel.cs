using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class QuestSubmissionPanel : UIBase
{
    [SerializeField] private Button _closeBtn;

    private TitleData _titleData;

    public override void Initialize()
    {
        _titleData = App.GetData<TitleData>();
  
        _closeBtn.onClick.AddListener(ClosePanel);
    }

    public void OpenPanel(DetailQuestData questData)
    {
        base.OpenPanel();
    }
}
