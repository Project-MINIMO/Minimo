using UnityEngine;
using System.Threading.Tasks;

public class UserGold : IQuestRewardTarget
{
    public Sprite Icon { get; }
    public int Count { get; private set; } = 0;

    private FirebaseManager _firebaseManager;

    public UserGold(Sprite icon)
    {
        Icon = icon;
        _firebaseManager = App.GetManager<FirebaseManager>();
        
        // Firebase OnSDCUpdate Action에 이벤트 핸들러 등록
        if (_firebaseManager != null)
        {
            _firebaseManager.OnSDCUpdate += OnSDCUpdate;
            Count = _firebaseManager.GetCurrentSDC();
        }
        else
        {
            Debug.LogWarning("FirebaseManager is not initialized. Using local count only.");
        }
    }
    
    private void OnSDCUpdate(int newCount)
    {
        Count = newCount;
    }
    
    public async void AddCount(int amount)
    {
        if (_firebaseManager != null)
        {
            // Firebase에 SDC 추가 요청
            bool success = await _firebaseManager.UpdateSDC(amount, "add");
            if (!success)
            { 
                Debug.LogError("Failed to update SDC in Firebase");
            }
        }
        else
        {
            // Firebase 매니저가 없는 경우 로컬에서만 업데이트
            Count += amount;
            Count = Mathf.Clamp(Count, 0, int.MaxValue);
        }
    }

    // Firebase에서 최신 SDC 값 동기화
    public void SyncWithFirebase()
    {
        if (_firebaseManager != null)
        {
            Count = _firebaseManager.GetCurrentSDC();
        }
    }
}
