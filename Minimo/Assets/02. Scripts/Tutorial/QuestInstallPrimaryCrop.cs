using UnityEngine;
using TMPro;

public class QuestInstallPrimaryCrop : QuestBase
{
    public override string ID => "InstallPrimary_Crop";
    protected override bool IsClear => CheckClear();

    [SerializeField] private Transform _builidngParent;
    [SerializeField] private TextMeshProUGUI _detailText;
    
    private BuildingObject _buildingObject;
    
    protected override void ClearQuest()
    {
        base.ClearQuest();
        
        _detailText.text = "밭을 설치하세요. (1/1)";
    }
    
    private bool CheckClear()
    {
        if (_buildingObject == null) 
        {
            if (_builidngParent.childCount > 0)
            {
                for (var i = 0; i < _builidngParent.childCount; i++)
                {
                    if (string.Equals(_builidngParent.GetChild(i).gameObject.name, "Building_Farm(Clone)"))
                    {
                        _buildingObject = _builidngParent.GetChild(i).gameObject.GetComponent<BuildingObject>();
                    }
                }
            }
            
            return false;
        }
        else
        {
            if (_buildingObject.IsPlaced) 
            {
                return true;
            }
        }
        
        return false;
    }
}