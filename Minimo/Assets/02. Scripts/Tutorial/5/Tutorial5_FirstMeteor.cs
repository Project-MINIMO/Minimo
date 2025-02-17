using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Tutorial5_FirstMeteor : TutorialBase
{
    [SerializeField] private CanvasGroup _questCanvasGroup;
    [SerializeField] private TileStateModifier _tileStateModifier;
    [SerializeField] private BoundsInt _area;
    private SidePanel _sidePanel;

    private void Start()
    {
        _sidePanel = App.GetManager<UIManager>().GetPanel<SidePanel>();
    }
    
    public override void StartTutorial()
    {
        gameObject.SetActive(true);
        _questCanvasGroup.DOFade(1, 0.5f);

        App.GetManager<QuestManager>().StartQuest("FirstMeteor");
        
        StartCoroutine(WaitUntilSideOpen());
    }

    private IEnumerator WaitUntilSideOpen()
    {
        yield return new WaitUntil(() => _sidePanel.IsOpen);
        
        _questCanvasGroup.DOFade(0, 0.5f)
            .OnComplete(() => _questCanvasGroup.gameObject.SetActive(false));
    }
    
    public override void EndTutorial()
    {
        App.GetManager<AccountInfoManager>().UpdateLevel(3);
        _tileStateModifier.ModifyTileState(_area, TileState.Empty);
        
        _sidePanel.Close(); 
        gameObject.SetActive(false);
    }
}
