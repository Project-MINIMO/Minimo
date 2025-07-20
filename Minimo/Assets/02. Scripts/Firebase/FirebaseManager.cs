using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.Functions;

/// <summary>
/// Firebase Authentication을 관리하는 매니저 클래스
/// 사용자 인증, 프로필 관리, 계정 관리 등의 기능을 제공합니다.
/// </summary>
public class FirebaseManager : ManagerBase
{
    private FirebaseAuth _auth;
    private FirebaseFirestore _db;
    private FirebaseFunctions _functions;
    private FirebaseUser _user;
    private bool _isInitialized = false;
    
    // 문서
    private UserData _currentUserData;
    public UserData CurrentUserData
    {
        get => _currentUserData;
        private set
        {
            _currentUserData = value;
            Debug.Log($"UserData updated: {_currentUserData}");
        }
    }

    // 이벤트
    public event Action<FirebaseUser> OnUserSignedIn;
    public event Action OnUserSignedOut;
    public event Action<string> OnError;

    public FirebaseUser CurrentUser => _user;
    public bool IsSignedIn => _user != null;

    protected override void Awake()
    {
        base.Awake();
        InitializeFirebase();
    }
    
    private void OnDestroy()
    {
        if (_auth != null)
        {
            _auth.StateChanged -= AuthStateChanged;
        }
    }

    private async void InitializeFirebase()
    {
        _functions = FirebaseFunctions.GetInstance("asia-northeast3");
        _db = FirebaseFirestore.DefaultInstance;
        _auth = FirebaseAuth.DefaultInstance;
        
        _auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);

        // 저장된 정보로부터 자동 로그인 시도 => 실패 시 익명 로그인
        if (_auth.CurrentUser == null && !_isInitialized)
        {
            await SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("익명 로그인 실패: " + task.Exception);
                }
                else
                {
                    if (_auth.CurrentUser == null)
                    {
                        Debug.LogError("로그인 실패: 현재 사용자 정보가 없습니다.");
                        return;
                    }
                    
                    _isInitialized = true;
                    Debug.Log("익명 로그인 성공");
                }
            });
        }
        else
        {
            Debug.Log($"자동 로그인 성공: {_auth.CurrentUser?.Email ?? "익명 사용자"}");
            _user = _auth.CurrentUser;
            _isInitialized = true;
        }
        
        // user 문서 가져오기
        _currentUserData = await FetchUserData(_user?.UserId);
        if (_currentUserData == null)
        {
            // create users document
            var callable = _functions.GetHttpsCallable("createUserAccount");
            await callable.CallAsync(new Dictionary<string,object>()).ContinueWithOnMainThread(createTask =>
            {
                if (createTask.IsFaulted)
                {
                    Debug.LogError("사용자 계정 생성 실패: " + createTask.Exception);
                }
                else
                {
                    Debug.Log("사용자 계정 생성 성공");
                }
            });
            _currentUserData = await FetchUserData(_user?.UserId);
        }
    }

#region Util Method
    // ===================== //
    //      유틸 메서드      //
    // ===================== //

    private async Task<bool> Try(Func<Task> action)
    {
        try
        {
            await action();
            return true;
        }
        catch (Exception e)
        {
            OnError?.Invoke(e.Message);
            return false;
        }
    }

    private async Task<bool> Try<T>(Func<Task<T>> action, Action<T> onSuccess = null)
    {
        try
        {
            var result = await action();
            onSuccess?.Invoke(result);
            return true;
        }
        catch (Exception e)
        {
            OnError?.Invoke(e.Message);
            return false;
        }
    }
    
    private async Task<(bool success, T result)> TryGet<T>(Func<Task<T>> action)
    {
        try
        {
            var result = await action();
            return (true, result);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e.ToString()); // 더 자세히 출력
            return (false, default);
        }
    }

#endregion

#region Auth
// ===================== //
//     사용자 인증 관련    //
// ===================== //
    public Task<bool> SignInAnonymouslyAsync()
    {
        return Try(() => _auth.SignInAnonymouslyAsync(), result =>
        {
            _user = result.User;
            OnUserSignedIn?.Invoke(_user);
        });
    }

    public Task<bool> CreateUserWithEmailAndPasswordAsync(string email, string password)
    {
        return Try(() => _auth.CreateUserWithEmailAndPasswordAsync(email, password), result =>
        {
            _user = result.User;
            OnUserSignedIn?.Invoke(_user);
        });
    }

    public Task<bool> SignInWithEmailAndPasswordAsync(string email, string password)
    {
        return Try(() => _auth.SignInWithEmailAndPasswordAsync(email, password), result =>
        {
            _user = result.User;
            OnUserSignedIn?.Invoke(_user);
        });
    }

    public Task<bool> SignInWithGoogleAsync(string idToken, string accessToken)
    {
        var credential = GoogleAuthProvider.GetCredential(idToken, accessToken);
        return Try(() => _auth.SignInWithCredentialAsync(credential), result =>
        {
            _user = result;
            OnUserSignedIn?.Invoke(_user);
        });
    }

    public void SignOut()
    {
        _auth.SignOut();
    }

    // ===================== //
    //     계정 정보 관리     //
    // ===================== //

    public Task<bool> UpdateUserProfileAsync(string displayName, string photoUrl = null)
    {
        if (_user == null) return Task.FromResult(false);

        var profile = new UserProfile
        {
            DisplayName = displayName,
            PhotoUrl = photoUrl != null ? new Uri(photoUrl) : null
        };

        return Try(() => _user.UpdateUserProfileAsync(profile));
    }

    public Task<bool> UpdateEmailAsync(string newEmail)
    {
        if (_user == null) return Task.FromResult(false);
        return Try(() => _user.UpdateEmailAsync(newEmail));
    }

    public Task<bool> UpdatePasswordAsync(string newPassword)
    {
        if (_user == null) return Task.FromResult(false);
        return Try(() => _user.UpdatePasswordAsync(newPassword));
    }

    public Task<bool> SendEmailVerificationAsync()
    {
        if (_user == null) return Task.FromResult(false);
        return Try(() => _user.SendEmailVerificationAsync());
    }

    public Task<bool> SendPasswordResetEmailAsync(string email)
    {
        return Try(() => _auth.SendPasswordResetEmailAsync(email));
    }

    public Task<bool> ReauthenticateAsync(string email, string password)
    {
        if (_user == null) return Task.FromResult(false);
        var credential = EmailAuthProvider.GetCredential(email, password);
        return Try(() => _user.ReauthenticateAsync(credential));
    }

    public Task<bool> DeleteUserAsync()
    {
        if (_user == null) return Task.FromResult(false);
        return Try(() => _user.DeleteAsync());
    }
#endregion

#region Building
    public async Task<List<BuildingDTO>> LoadUserBuildings()
    {
        var uid = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
        if (string.IsNullOrEmpty(uid))
        {
            Debug.LogError("No user logged in.");
            return new List<BuildingDTO>();
        }

        var snapshot = await FirebaseFirestore.DefaultInstance
            .Collection("users").Document(uid)
            .Collection("buildings").GetSnapshotAsync();

        var buildings = new List<BuildingDTO>();
        foreach (var doc in snapshot.Documents)
        {
            var data = doc.ToDictionary();

            var dto = new BuildingDTO
            {
                BuildingId = doc.Id,
                BuildingDataId = Convert.ToInt32(data["buildingDataId"]),
                Position = new Vector3Int(
                    Convert.ToInt32(((Dictionary<string, object>)data["position"])["x"]),
                    Convert.ToInt32(((Dictionary<string, object>)data["position"])["y"]),
                    Convert.ToInt32(((Dictionary<string, object>)data["position"])["z"])
                )
            };

            buildings.Add(dto);
        }

        return buildings;
    }

    public async Task<string?> InstallBuilding(int buildingDataId, Vector3Int position)
    {
        
        var callable = _functions.GetHttpsCallable("installBuilding");
        var data = new Dictionary<string, object>
        {
            { "buildingDataId", buildingDataId },
            { "position", new[] { position.x, position.y, position.z } }
        };

        var (success, result) = await TryGet(() => callable.CallAsync(data));
        if (!success)
        {
            return null;
        }

        var dict = result.Data as Dictionary<object, object>;
        string buildingId = dict?["buildingId"] as string;

        Debug.Log($"Building installed at {position}, ID: {buildingId}");
        return buildingId;
    }

    public Task<bool> MoveBuilding(string buildingId, Vector3Int position)
    {
        var callable = _functions.GetHttpsCallable("moveBuilding");
        var data = new Dictionary<string, object>
        {
            { "buildingId", buildingId },
            { "position", new[] { position.x, position.y, position.z } }
        };
        return Try(() => callable.CallAsync(data), result =>
        {
            Debug.Log($"Building moved to {position}: {result.Data}");
        });
    }
#endregion

#region Private
    private void AuthStateChanged(object sender, EventArgs args)
    {
        if (_auth.CurrentUser != _user)
        {
            bool signedIn = _auth.CurrentUser != null;
            if (signedIn)
            {
                _user = _auth.CurrentUser;
                OnUserSignedIn?.Invoke(_user);
            }
            else
            {
                _user = null;
                OnUserSignedOut?.Invoke();
            }
        }
    }

    private async Task<UserData> FetchUserData(string uid)
    {
        var docRef = _db.Collection("users").Document(uid);
        UserData result = null;

        await docRef.GetSnapshotAsync().ContinueWithOnMainThread<DocumentSnapshot>(snapshotTask =>
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
                result = snapshot.ConvertTo<UserData>();
                Debug.Log($"User data fetched: {result}");
            }
            else
            {
                Debug.LogWarning("User document does not exist.");
            }
        });
        return result;
    }
#endregion
}
