
using Cysharp.Threading.Tasks;
using MinimoShared;
using UnityEditor;
using UnityEngine;

public class CheatUI : EditorWindow
{
    private CurrencyDTO currency = new CurrencyDTO();
    private ItemDTO item = new ItemDTO();
    private BuildingInfoDTO buildingInfo = new BuildingInfoDTO();
    private string targetDateTime = "2021-09-01 00:00:00";
    private int meteorId;
    
    private Vector2 scrollPosition;
    
    [MenuItem("Tools/Cheat UI")]
    public static void ShowWindow()
    {
        GetWindow<CheatUI>("Cheat UI");
    }

    /// <summary>
    /// 1. Currency 치트
    /// 2. 아이템 치트
    /// 3. 건물 정보 치트
    /// </summary>
    private void OnGUI()
    {
        if(App.Services == null)
        {
            EditorGUILayout.HelpBox("Service container is not initialized.", MessageType.Error);
            return;
        }
        
        // 스크롤뷰 시작
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        // Currency를 SerializeField로 만들어서 이곳에 노출시키면 좋을 것 같다.
        // Currency 치트
        currency.Star = EditorGUILayout.IntField("Star", currency.Star);
        currency.Heart = EditorGUILayout.IntField("Heart", currency.Heart);
        currency.BlueStar = EditorGUILayout.IntField("BlueStar", currency.BlueStar);
        currency.HPI = EditorGUILayout.IntField("HPI", currency.HPI);
        
        if (GUILayout.Button("재화 업데이트"))
        {
            App.GetManager<CheatManager>().UpdateCurrency(currency).Forget();
        }
        
        // 공백
        EditorGUILayout.Space();
        
        // 아이템 치트
        item.ItemType = EditorGUILayout.TextField("ItemType", item.ItemType);
        item.Count = EditorGUILayout.IntField("Count", item.Count);
        if (GUILayout.Button("자원 업데이트"))
        {
            App.GetManager<CheatManager>().UpdateItem(item).Forget();
        }
        
        // 공백
        EditorGUILayout.Space();
        
        // 건물 정보 치트
        buildingInfo.BuildingType = EditorGUILayout.TextField("BuildingType", buildingInfo.BuildingType);
        buildingInfo.OwnCount = EditorGUILayout.IntField("OwnCount", buildingInfo.OwnCount);
        buildingInfo.MaxCount = EditorGUILayout.IntField("MaxCount", buildingInfo.MaxCount);
        buildingInfo.ProduceSlotCount = EditorGUILayout.IntField("ProduceSlotCount", buildingInfo.ProduceSlotCount);
        if (GUILayout.Button("건물 정보 업데이트"))
        {
            App.GetManager<CheatManager>().UpdateBuildingInfo(buildingInfo).Forget();
        }
        
        // 공백
        EditorGUILayout.Space();
        
        // 시간 치트
        targetDateTime = EditorGUILayout.TextField("TargetDateTime(\"yyyy-mm-dd hh:mm:ss\")", targetDateTime);
        if (GUILayout.Button("서버 시간 변경"))
        {
            App.GetManager<CheatManager>().SetServerTimeForce(targetDateTime);
        }
        
        // 유성 치트
        // 유성 생성
        if (GUILayout.Button("유성 생성"))
        {
            App.GetManager<CheatManager>().TryCreateMeteor();
        }
        // 유성 획득
        meteorId = EditorGUILayout.IntField("MeteorId", meteorId);
        if (GUILayout.Button("유성 획득"))
        {
            App.GetManager<CheatManager>().TryGetMeteorResult(meteorId);
        }
        
        // 스크롤뷰 끝
        EditorGUILayout.EndScrollView();
    }
}
