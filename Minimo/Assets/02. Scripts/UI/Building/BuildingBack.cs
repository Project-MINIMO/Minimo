using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class BuildingBack : MonoBehaviour
{
    private enum EBuildingType
    {
        Production,
        Decoration,
        Utility,
        Minimo
    }

    [SerializeField] private EBuildingType _buildingType;
    [SerializeField] private Transform _buildingBtnParent;
    [SerializeField] private GameObject _buildingBtnPrefab;

    private Dictionary<int, BuildingBtn> _btnDictionary = new();
    private Transform _buildingObjectParent;

    private void Awake()
    {
        _buildingObjectParent = GameObject.FindWithTag("BuildingObjectParent").transform;

        InitBuildingBtns();
    }

    private void InitBuildingBtns()
    {
        BuildingBtn buildingBtn;

        var existingButtons = GetComponentsInChildren<BuildingBtn>(true);

        int index = 0;

        foreach (var data in App.GetData<TitleData>().Building.Values)
        {
            if (data.Type != (int)_buildingType)
            {
                continue;
            }

            if (index < existingButtons.Length)
            {
                buildingBtn = existingButtons[index];
            }
            else
            {
                buildingBtn = Instantiate(_buildingBtnPrefab, _buildingBtnParent).GetComponent<BuildingBtn>();
            }

            buildingBtn.Initialize(data, _buildingObjectParent);
            _btnDictionary.Add(data.ID, buildingBtn);

            index++;
        }

        for (; index < existingButtons.Length; index++)
        {
            existingButtons[index].gameObject.SetActive(false);
        }
    }
}