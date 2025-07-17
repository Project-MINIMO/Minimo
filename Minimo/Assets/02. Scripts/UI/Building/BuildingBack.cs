using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class BuildingBack : MonoBehaviour
{
    private enum EBuildingType
    {
        Production = 0,
        Decoration = 3,
        Utility,
        Minimo
    }

    [SerializeField] private EBuildingType _buildingType;
    [SerializeField] private Transform _buildingBtnParent;
    [SerializeField] private GameObject _buildingBtnPrefab;

    private Dictionary<int, BuildingBtn> _btnDictionary = new();

    private void Awake()
    {
        InitBuildingBtns();
    }

    private void InitBuildingBtns()
    {
        BuildingBtn buildingBtn;

        var existingButtons = GetComponentsInChildren<BuildingBtn>(true);

        var index = 0;

        foreach (var data in App.GetData<TitleData>().Building.Values)
        {
            var isValid = true;//_buildingType == EBuildingType.Production
                //? data.Type is 0 or 1 or 2 or 3
                //: data.Type == (int)_buildingType;

            if (!isValid)
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

            //buildingBtn.Initialize(data);
            _btnDictionary.Add(data.ID, buildingBtn);

            index++;
        }

        for (; index < existingButtons.Length; index++)
        {
            existingButtons[index].gameObject.SetActive(false);
        }
    }
}