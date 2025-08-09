using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class TutorialStep_05 : TutorialStep
{
    [SerializeField] private EditManager _editManager;
    [SerializeField] private GameObject _primaryPanel;
    [SerializeField] private RectTransform _handIcon;
    [SerializeField] private SpriteRenderer _highlight;
    private Vector2 _startPos = new(5, 201);
    private Vector2 _endPos = new(40, -82);
    
    private ProduceObject _farmObject;
    
    public async void SpawnFarm()
    {
        if (_editManager.ActiveProduces.Count > 0)
        {
            _farmObject = _editManager.ActiveProduces.FirstOrDefault(x => x.BuildingData.ID == 0);
            _farmObject?.CreateTask(_farmObject.ProduceData[0]);
            return;
        }
        
        var farm = App.GetData<TitleData>().Building[0];
        _farmObject = await _editManager.SpawnBuildingAsync(farm, _editManager.AlignToCell(new Vector3(2.5f, -0.5f, 0)));
        var farmObject2 = await _editManager.SpawnBuildingAsync(farm, _editManager.AlignToCell(new Vector3(3f, -0.75f, 0)));
        await _editManager.Install(_farmObject);
        await _editManager.Install(farmObject2);
        
        _farmObject.CreateTask(_farmObject.ProduceData[0]);
    }
    
    protected override void OnStart()
    {
        StartCoroutine(ProgressQuest());
    }
   
    private IEnumerator ProgressQuest()
    {
        _highlight.gameObject.SetActive(true);
        _highlight.DOFade(0.7f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        
        yield return new WaitUntil(() => _primaryPanel.activeSelf);

        _highlight.DOKill();
        _highlight.gameObject.SetActive(false);
        _handIcon.gameObject.SetActive(true);
        _handIcon
            .DOAnchorPos(_endPos, 1)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Restart);
        
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        
        _handIcon.DOKill();
        _handIcon.gameObject.SetActive(false);
        
        yield return new WaitUntil(() => _farmObject.CurrentState == ProduceState.Idle);
        
        CompleteStep();
    }

    public override void Cleanup()
    {
        AccountInfo.Instance.Level.AddCount(100);
    }
}
