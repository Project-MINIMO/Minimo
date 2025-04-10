using UnityEngine;

public class WorldYThresholdUpdater : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;  
    [SerializeField] private Material _targetMaterial; 
    
    private void Update()
    {
        Vector3 screenPoint = new Vector3(Screen.width / 2, Screen.height * 0.5f, 0);
        
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPoint);
   
        float worldYThreshold = worldPoint.y;

        targetMaterial.SetFloat("_WorldYThreshold", worldYThreshold);
    }
}