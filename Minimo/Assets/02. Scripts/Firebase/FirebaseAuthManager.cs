using System;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;

/// <summary>
/// Firebase Authentication을 관리하는 매니저 클래스
/// 사용자 인증, 프로필 관리, 계정 관리 등의 기능을 제공합니다.
/// </summary>
public class FirebaseAuthManager : ManagerBase
{
    private FirebaseAuth auth;
    private FirebaseUser user;
    private bool isInitialized = false;

    // 이벤트
    public event Action<FirebaseUser> OnUserSignedIn;
    public event Action OnUserSignedOut;
    public event Action<string> OnError;

    public FirebaseUser CurrentUser => user;
    public bool IsSignedIn => user != null;

    protected override void Awake()
    {
        base.Awake();
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);

        if (auth.CurrentUser == null && !isInitialized)
        {
            SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("익명 로그인 실패: " + task.Exception);
                }
                else
                {
                    isInitialized = true;
                    Debug.Log("익명 로그인 성공");
                }
            });
        }
        else
        {
            Debug.Log($"자동 로그인 성공: {auth.CurrentUser?.Email ?? "익명 사용자"}");
            user = auth.CurrentUser;
            isInitialized = true;
        }
    }

    private void AuthStateChanged(object sender, EventArgs args)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = auth.CurrentUser != null;
            if (signedIn)
            {
                user = auth.CurrentUser;
                OnUserSignedIn?.Invoke(user);
            }
            else
            {
                user = null;
                OnUserSignedOut?.Invoke();
            }
        }
    }

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

    // ===================== //
    //     사용자 인증 관련    //
    // ===================== //

    public Task<bool> SignInAnonymouslyAsync()
    {
        return Try(() => auth.SignInAnonymouslyAsync(), result =>
        {
            user = result.User;
            OnUserSignedIn?.Invoke(user);
        });
    }

    public Task<bool> CreateUserWithEmailAndPasswordAsync(string email, string password)
    {
        return Try(() => auth.CreateUserWithEmailAndPasswordAsync(email, password), result =>
        {
            user = result.User;
            OnUserSignedIn?.Invoke(user);
        });
    }

    public Task<bool> SignInWithEmailAndPasswordAsync(string email, string password)
    {
        return Try(() => auth.SignInWithEmailAndPasswordAsync(email, password), result =>
        {
            user = result.User;
            OnUserSignedIn?.Invoke(user);
        });
    }

    public Task<bool> SignInWithGoogleAsync(string idToken, string accessToken)
    {
        var credential = GoogleAuthProvider.GetCredential(idToken, accessToken);
        return Try(() => auth.SignInWithCredentialAsync(credential), result =>
        {
            user = result;
            OnUserSignedIn?.Invoke(user);
        });
    }

    public void SignOut()
    {
        auth.SignOut();
    }

    // ===================== //
    //     계정 정보 관리     //
    // ===================== //

    public Task<bool> UpdateUserProfileAsync(string displayName, string photoUrl = null)
    {
        if (user == null) return Task.FromResult(false);

        var profile = new UserProfile
        {
            DisplayName = displayName,
            PhotoUrl = photoUrl != null ? new Uri(photoUrl) : null
        };

        return Try(() => user.UpdateUserProfileAsync(profile));
    }

    public Task<bool> UpdateEmailAsync(string newEmail)
    {
        if (user == null) return Task.FromResult(false);
        return Try(() => user.UpdateEmailAsync(newEmail));
    }

    public Task<bool> UpdatePasswordAsync(string newPassword)
    {
        if (user == null) return Task.FromResult(false);
        return Try(() => user.UpdatePasswordAsync(newPassword));
    }

    public Task<bool> SendEmailVerificationAsync()
    {
        if (user == null) return Task.FromResult(false);
        return Try(() => user.SendEmailVerificationAsync());
    }

    public Task<bool> SendPasswordResetEmailAsync(string email)
    {
        return Try(() => auth.SendPasswordResetEmailAsync(email));
    }

    public Task<bool> ReauthenticateAsync(string email, string password)
    {
        if (user == null) return Task.FromResult(false);
        var credential = EmailAuthProvider.GetCredential(email, password);
        return Try(() => user.ReauthenticateAsync(credential));
    }

    public Task<bool> DeleteUserAsync()
    {
        if (user == null) return Task.FromResult(false);
        return Try(() => user.DeleteAsync());
    }

    private void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
        }
    }
}
