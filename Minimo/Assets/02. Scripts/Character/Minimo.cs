using UnityEngine;

public class Minimo : MonoBehaviour
{
    public UMData Data { get; private set; }
    public MinimoFSM FSM { get; private set; }
    public ProduceAdvanced AssignedBuilding { get; private set; }
    public int Level { get; private set; }

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private Transform _parent;    //temp
    private UMStatGrowthData _statGrowthData1;
    private UMStatGrowthData _statGrowthData2;
    private UMStatGrowthData _statGrowthData3;
    public float AbilityValue1 { get; private set; }
    public float AbilityValue2 { get; private set; }
    public float AbilityValue3 { get; private set; }

    private float _potential;
    
    private void Awake()
    {
        var titleData = App.GetData<TitleData>();
        _parent = transform.parent;   //temp
        Data = titleData.UserMinimo[transform.GetSiblingIndex()];    //temp
        float potentialValue = Data.Potential;
        var rawPotential = (potentialValue / 9) * titleData.Common["PotentialGap"];
        _potential = Mathf.Round(rawPotential * 100f) / 100f;

        _statGrowthData1 = titleData.UMStatGrowth[Data.StatType1];
        _statGrowthData2 = titleData.UMStatGrowth[Data.StatType2];
        _statGrowthData3 = titleData.UMStatGrowth[Data.StatType3];
        
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        FSM = new MinimoFSM(this);
        SetChillState();

        AddLevel(1);
    }

    private void Update()
    {
        FSM.Update();
    }
    
    public void SetSpriteFilp(bool isFlip)
    {
        _spriteRenderer.flipX = isFlip;
    }
    
    public void SetSpriteOrder(int order)
    {
        _spriteRenderer.sortingOrder = order;
    }

    public void SetAnimation(string trigger, bool isActive)
    {
        _animator.SetBool(trigger, isActive);
    }

    public void SetChillState()
    {
        //var randomIndex = Random.Range(0, 2);
        //FSM.ChangeState(randomIndex == 0 ? MinimoState.Idle : MinimoState.Walk);
        FSM.ChangeState(MinimoState.Idle);
        
        transform.SetParent(_parent);   //temp
        transform.localPosition = Vector3.zero;   //temp

        AssignedBuilding = null;
    }

    public void SetWorkState(ProduceAdvanced produceObject)
    {
        FSM.ChangeState(MinimoState.Work);
        //_animator.SetTrigger(produceObject.AnimTrigger);
        
        transform.SetParent(produceObject.MinimoWorkingPosition);
        transform.localPosition = Vector3.zero;
        
        SetSpriteFilp(false);

        AssignedBuilding = produceObject;
    }

    public void AddLevel(int amount)
    {
        Level += amount;
        Level = Mathf.Clamp(Level, 1, 30);

        CalculateAbility();
    }

    private void CalculateAbility()
    {
        if (_statGrowthData1 != null)
        {
            var value = _statGrowthData1.BaseValue + Level * _statGrowthData1.Step;
            AbilityValue1 = Mathf.Round(value * _potential * 100f) / 100f;
        }
        
        if (_statGrowthData2 != null)
        {
            var value = _statGrowthData2.BaseValue + Level * _statGrowthData2.Step;
            AbilityValue2 = Mathf.Round(value * _potential * 100f) / 100f;
        }
        
        if (_statGrowthData3 != null)
        {
            var value = _statGrowthData3.BaseValue + Level * _statGrowthData3.Step;
            AbilityValue3 = Mathf.Round(value * _potential * 100f) / 100f;
        }
    }
}
