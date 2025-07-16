using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuaternaryPanel : UIBase
{
    [Serializable]
    public struct WishOption
    {
        public WishSlot Slot;
        public Button CloseBtn;
        public GameObject OptionsBack;
    }

    [Serializable]
    public struct WishSlot
    {
        public Button Button;
        public Image Icon;
        public Item Item;
    }
    
    [SerializeField] private Button _closeBtn;
    [SerializeField] private TextMeshProUGUI _titleTMP;
    
    private ProduceSlot[] _taskBtns;
    [SerializeField] private Button _expandBtn;
    [SerializeField] private GameObject _back;
    
    [SerializeField] private WishOption _food;
    [SerializeField] private WishOption _flower;
    
    [SerializeField] private Button _placeMinimoBtn;

    private ProduceManager _produceManager;
    private PlaceMinimoPanel _placeMinimoPanel;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _produceManager = App.GetManager<ProduceManager>();
        _placeMinimoPanel = manager.GetPanel<PlaceMinimoPanel>();
        
        _taskBtns = GetComponentsInChildren<ProduceSlot>(true);
        _closeBtn.onClick.AddListener(_produceManager.Deselect);
        _expandBtn.onClick.AddListener(() =>
        {
            ((ProduceElevated)_produceManager.CurrentObject).AddSlotCount();
            InitializeTaskBtns();
        });
        
        _food.Slot.Button.onClick.AddListener(() =>
        {
            _back.SetActive(false);
            _food.OptionsBack.SetActive(true);
        });
        
        _flower.Slot.Button.onClick.AddListener(() =>
        {
            _back.SetActive(false);
            _flower.OptionsBack.SetActive(true);
        });
        
        _food.CloseBtn.onClick.AddListener(() =>
        {
            _back.SetActive(true);
            _food.OptionsBack.SetActive(false);
        });
        
        _flower.CloseBtn.onClick.AddListener(() =>
        {
            _back.SetActive(true);
            _flower.OptionsBack.SetActive(false);
        });
        
        _placeMinimoBtn.onClick.AddListener(_placeMinimoPanel.OpenPanel);
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
        
        _titleTMP.text = App.GetData<TitleData>()
            .GetString($"STR_BUILDING_{_produceManager.CurrentObject.BuildingData.Name.ToUpper()}_NAME");

        InitializeTaskBtns();
    }

    private void InitializeTaskBtns()
    {
        var maxCount = ((ProduceElevated)_produceManager.CurrentObject).MaxSlotCount;
        var i = 0;

        for (; i < maxCount; i++)
        {
            _taskBtns[i].gameObject.SetActive(true);
        }

        for (; i < _taskBtns.Length; i++)
        {
            _taskBtns[i].gameObject.SetActive(false);
        }
        
        _expandBtn.gameObject.SetActive(maxCount < 5);
    }

    public void SetItemOnSlot(Item item)
    {
        if (item.Data.Type == (int)ItemType.Food)
        {
            _back.SetActive(true);
            _food.OptionsBack.SetActive(false);
            
            _food.Slot.Icon.sprite = Resources.Load<Sprite>($"Item/{item.Name}");
            _food.Slot.Item = item;

            if (_flower.Slot.Item != null)
            {
                ((ProduceQuaternary)_produceManager.CurrentObject).StartPlant(_flower.Slot.Item.Data.ID,  _food.Slot.Item.Data.ID);
                
                _food.Slot.Item = null;
                _food.Slot.Icon.sprite = null;
                
                _flower.Slot.Item = null;
                _flower.Slot.Icon.sprite = null;
            }
        }
        else
        {
            _back.SetActive(true);
            _flower.OptionsBack.SetActive(false);
            
            _flower.Slot.Icon.sprite = Resources.Load<Sprite>($"Item/{item.Name}");
            _flower.Slot.Item = item;

            if (_food.Slot.Item != null)
            {
                ((ProduceQuaternary)_produceManager.CurrentObject).StartPlant(_flower.Slot.Item.Data.ID,  _food.Slot.Item.Data.ID);
                
                _food.Slot.Item = null;
                _food.Slot.Icon.sprite = null;
                
                _flower.Slot.Item = null;
                _flower.Slot.Icon.sprite = null;
            }
        }
    }
}