using System;
using MinimoShared;
using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingBtn : MonoBehaviour
{
    private enum BuildingState
    {
        Use,
        Unlock,
        Lock,
    }

    [Serializable]
    public struct RequireItem
    {
        public GameObject ItemBack;
        public TextMeshProUGUI CountTMP;
        public Image IconImg;
    }

    [SerializeField] private Button _editBtn;
    [SerializeField] private TextMeshProUGUI _nameTMP;
    [SerializeField] private TextMeshProUGUI _countTMP;
    [SerializeField] private TextMeshProUGUI _hpiTMP;
    [SerializeField] private Image _iconImg;

    [SerializeField] private GameObject[] _stateBacks;

    [SerializeField] private RequireItem _requireItem1;
    [SerializeField] private RequireItem _requireItem2;

    [SerializeField] private TextMeshProUGUI _lockNoticeTMP;
    [SerializeField] private TextMeshProUGUI _unlockNoticeTMP;
    
    public BuildingData Data { get; private set; }
    private GameObject _objectPrefab;
    private Transform _buildingGroup;

    private BuildingPanel _buildingPanel;
    
    private BuildingState _currentState;

    private void Awake()
    {
        _editBtn.onClick.AddListener(CreateObject);
        _buildingPanel = App.GetManager<UIManager>().GetPanel<BuildingPanel>();
    }

    public void Initialize(BuildingData data, Transform gridObjectGroup)
    {
        Data = data;

        _buildingGroup = gridObjectGroup;

        var spritePath = $"Building/Icon/{data.Icon}";
        _iconImg.sprite = Resources.Load<Sprite>(spritePath);
        _iconImg.SetNativeSize();
        
        var prefabPath = $"Building/{data.ID}";
        _objectPrefab = Resources.Load<GameObject>(prefabPath);

        SetString();
        SetRequireItem();
        SetBuildingState();
        
        //if (data.ID is "Building_Farm" or "Building_Orchard")
        //{
        //    gameObject.SetActive(true);
        //}
        //else
        //{
        //    gameObject.SetActive(false);
        //}
    }

    private void SetString()
    {
        var titleData = App.GetData<TitleData>();

        _nameTMP.text = titleData.GetString(Data.Name);
        _lockNoticeTMP.text = titleData.GetFormatString("STR_BUILDING_UI_LOCK", Data.UnlockLevel.ToString());
        _unlockNoticeTMP.text = titleData.GetString("STR_BUILDING_UI_UNLOCKABLE");

        _hpiTMP.text = Data.HPI.ToString();
    }

    private void SetRequireItem()
    {
        if (!App.GetData<TitleData>().Construct.TryGetValue(Data.ID, out var constructData))
        {
            Debug.LogError($"Can't Find ConstructData with ID : {Data.ID}");
            return;
        }

        if (string.IsNullOrEmpty(constructData.MatCode1))
        {
            _requireItem1.ItemBack.SetActive(false);
        }
        else
        {
            _requireItem1.ItemBack.SetActive(true);

            string spritePath = $"Item/{constructData.MatCode1}";
            _requireItem1.IconImg.sprite = Resources.Load<Sprite>(spritePath);

            _requireItem1.CountTMP.text = constructData.MatAmount1.ToString();
        }

        if (string.IsNullOrEmpty(constructData.MatCode2))
        {
            _requireItem2.ItemBack.SetActive(false);
        }
        else
        {
            _requireItem2.ItemBack.SetActive(true);

            string spritePath = $"Item/Icon/{constructData.MatCode2}";
            _requireItem2.IconImg.sprite = Resources.Load<Sprite>(spritePath);

            _requireItem2.CountTMP.text = constructData.MatAmount2.ToString();
        }
    }

    private void SetBuildingState()
    {
        if (App.GetManager<AccountInfoManager>().Level.Value < Data.UnlockLevel)
        {
            _currentState = BuildingState.Lock;
        }
        else
        {
            _currentState = BuildingState.Use;
        }

        for (int i = 0; i < _stateBacks.Length; i++)
        {
            if (i == (int)_currentState)
            {
                _stateBacks[i].SetActive(true);
            }
            else
            {
                _stateBacks[i].SetActive(false);
            }
        }
    }

    private void CreateObject()
    {
        _buildingPanel.ClosePanel();

        var cameraCenterPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane));
        cameraCenterPosition.z = 0;
        
        var gridObject = Instantiate(_objectPrefab, cameraCenterPosition, Quaternion.identity, _buildingGroup)
            .GetComponent<BuildingObject>();

        if (gridObject != null)
        {
            gridObject.Initialize(Data);
            App.GetManager<EditManager>().StartEdit(gridObject);
        }
        else
        {
            Debug.LogError("GridObject component not found in instantiated prefab.");
            Destroy(gridObject.gameObject);
        }
    }
}
