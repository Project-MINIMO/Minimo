using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.Functions;
using UnityEngine;
using Newtonsoft.Json; // 만약 Newtonsoft JSON을 쓴다면

public class FirebaseFunctionTest : MonoBehaviour
{
    private FirebaseFunctions _functions;
    private FirebaseAuth _auth;
    
    async void Awake()
    {
        _functions = FirebaseFunctions.DefaultInstance;
#if USE_EMULATOR
        _functions.UseFunctionsEmulator("http://localhost:5001");
#endif
        _auth = FirebaseAuth.DefaultInstance;
        await _auth.SignInAnonymouslyAsync();
        // 1초 대기
        await Task.Delay(1000);
        
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null) {
            Debug.LogError("로그인 안됨");
            return;
        }
        
        var callable = _functions.GetHttpsCallable("getUserAccountInfo");
        
        var data = new Dictionary<string, object>
        {
            //{ "text", "Hello World" }
        };

        await callable.CallAsync(data).ContinueWith(task => {
            if (task.IsFaulted) {
                Debug.LogError("Function call failed: " + task.Exception);
            } else {
                var result = task.Result.Data;
                Debug.Log("Raw JSON result: " + JsonConvert.SerializeObject(result));
            }
        });
        
        var uid = _auth.CurrentUser.UserId;
        Debug.Log("Current User ID: " + uid);
        FetchUserData(uid);
        
        // Delete Current User
        await _auth.CurrentUser.DeleteAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted) {
                Debug.LogError("Error deleting user: " + task.Exception);
            } else {
                Debug.Log("User deleted successfully.");
            }
        });
    }

    private void FetchUserData(string uid)
    {
        var db = FirebaseFirestore.DefaultInstance;
        var docRef = db.Collection("users").Document(uid);

        docRef.GetSnapshotAsync().ContinueWithOnMainThread<DocumentSnapshot>(snapshotTask =>
        {
            if (snapshotTask.IsFaulted)
            {
                Debug.LogError("Error getting user doc: " + snapshotTask.Exception);
                return;
            }

            var snapshot = snapshotTask.Result;
            if (snapshot.Exists)
            {
                // T로 변환
                var user = snapshot.ConvertTo<UserData>();
                Debug.Log($"{user.ToString()}");
            }
            else
            {
                Debug.LogWarning("User document does not exist.");
            }
        });
    }
}
