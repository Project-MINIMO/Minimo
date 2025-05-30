using System;
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

        var spritePath = $"Building/Icon/{data.Name}";
        _iconImg.sprite = Resources.Load<Sprite>(spritePath);
        _iconImg.SetNativeSize();
        
        var prefabPath = $"Building/{data.Name}";
        _objectPrefab = Resources.Load<GameObject>(prefabPath);

        SetString();
        SetBuildingState();
    }

    private void SetString()
    {
        var titleData = App.GetData<TitleData>();

        _nameTMP.text = titleData.GetString(Data.Name);
        _lockNoticeTMP.text = titleData.GetFormatString("STR_BUILDING_UI_LOCK", Data.UnlockLevel.ToString());
        _unlockNoticeTMP.text = titleData.GetString("STR_BUILDING_UI_UNLOCKABLE");
    }

    private void SetBuildingState()
    {
        if (AccountInfo.Instance.level < Data.UnlockLevel)
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
        }
        else
        {
            Debug.LogError("GridObject component not found in instantiated prefab.");
            Destroy(gridObject.gameObject);
        }
    }
}
