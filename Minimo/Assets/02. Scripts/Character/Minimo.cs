using System.Collections.Generic;

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
    
    public List<IMinimoAbility> Abilities { get; private set; }
    
    public MinimoManager _minimoManager;
    
    private void Awake()
    {
        var titleData = App.GetData<TitleData>();
        _parent = transform.parent;   //temp
        Data = titleData.UserMinimo[transform.GetSiblingIndex()];    //temp
        float potentialValue = Data.Potential;
        var rawPotential = 1 + (potentialValue - 1) * (((float)titleData.Common["PotentialGap"] - 1) / 9);
        var potential = Mathf.Round(rawPotential * 100f) / 100f;
        _minimoManager = App.GetManager<MinimoManager>();

        Abilities = new List<IMinimoAbility>
        {
            CreateAbility(titleData.UMStat[Data.StatType1].StatType, Data.StatType1, potential),
            CreateAbility(titleData.UMStat[Data.StatType2].StatType, Data.StatType2, potential),
            CreateAbility(titleData.UMStat[Data.StatType3].StatType, Data.StatType3, potential),
        };
        
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        FSM = new MinimoFSM(this);
        SetChillState();

        AddLevel(1);
    }

    private IMinimoAbility CreateAbility(int abilityType, int id, float potential) => (AbilityType)abilityType switch
    {
        AbilityType.None => null,
        AbilityType.ProdTime_Second => new ProduceTimeSecondAbility(id, potential),
        AbilityType.ProdTime_Percent => new ProduceTimePercentAbility(id, potential),
        AbilityType.ProdAmount => new ProduceAmountAbility(id, potential),
        AbilityType.ProdEXP => new ProduceExperienceAbility(id, potential),
        AbilityType.QuestEXP => new QuestExperienceAbility(id, potential),
        AbilityType.SellValue => new SellValueAbility(id, potential),
        AbilityType.TimeSkipCost => new TimeSkipCostAbility(id, potential),
        AbilityType.MissionTime => new MissionTimeAbility(id, potential)
    };

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

        if (AssignedBuilding != null)
        {
            AssignedBuilding = null;
        
            _minimoManager.OnMinimoUnassigned(this);
        }
    }

    public void SetWorkState(ProduceAdvanced produceObject)
    {
        FSM.ChangeState(MinimoState.Work);
        //_animator.SetTrigger(produceObject.AnimTrigger);
        
        transform.SetParent(produceObject.MinimoWorkingPosition);
        transform.localPosition = Vector3.zero;
        
        SetSpriteFilp(false);

        AssignedBuilding = produceObject;
        produceObject.PlaceMinimo(this);
        
        _minimoManager.OnMinimoAssigned(this);
        
        foreach (var ability in Abilities)
        {
            if (ability.IsApplicableTo(produceObject))
            {
                ability.Apply(produceObject);
            }
        }
    }

    public void AddLevel(int amount)
    {
        if (AssignedBuilding != null)
        {
            _minimoManager.OnMinimoUnassigned(this);
        }
        
        Level += amount;
        Level = Mathf.Clamp(Level, 1, 30);

        foreach (var ability in Abilities)
        {
            ability.CalculateAbility(Level);
        }

        if (AssignedBuilding != null)
        {
            foreach (var ability in Abilities)
            {
                if (ability.IsApplicableTo(AssignedBuilding))
                {
                    ability.Apply(AssignedBuilding);
                }
            }
            
            _minimoManager.OnMinimoAssigned(this);
        }
    }
}
