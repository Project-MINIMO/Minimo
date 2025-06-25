using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
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
    /// <summary>
    /// 사용자가 로그인했을 때 발생하는 이벤트
    /// </summary>
    public event Action<FirebaseUser> OnUserSignedIn;
    /// <summary>
    /// 사용자가 로그아웃했을 때 발생하는 이벤트
    /// </summary>
    public event Action OnUserSignedOut;
    /// <summary>
    /// 인증 과정에서 오류가 발생했을 때 발생하는 이벤트
    /// </summary>
    public event Action<string> OnError;

    /// <summary>
    /// 현재 로그인된 사용자 정보를 반환합니다.
    /// </summary>
    public FirebaseUser CurrentUser => user;
    /// <summary>
    /// 현재 로그인 상태를 반환합니다.
    /// </summary>
    public bool IsSignedIn => user != null;

    protected override void Awake()
    {
        base.Awake();
        InitializeFirebase();
    }

    /// <summary>
    /// Firebase Authentication을 초기화합니다.
    /// </summary>
    private void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    /// <summary>
    /// 인증 상태가 변경될 때 호출되는 콜백 메서드
    /// </summary>
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

    // 사용자 관리 기능
    /// <summary>
    /// 이메일과 비밀번호로 새 사용자를 생성합니다.
    /// </summary>
    /// <param name="email">사용자 이메일</param>
    /// <param name="password">사용자 비밀번호</param>
    /// <returns>생성 성공 여부</returns>
    public async Task<bool> CreateUserWithEmailAndPasswordAsync(string email, string password)
    {
        try
        {
            var result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            user = result.User;
            OnUserSignedIn?.Invoke(user);
            return true;
        }
        catch (Exception e)
        {
            OnError?.Invoke(e.Message);
            return false;
        }
    }

    /// <summary>
    /// 이메일과 비밀번호로 로그인합니다.
    /// </summary>
    /// <param name="email">사용자 이메일</param>
    /// <param name="password">사용자 비밀번호</param>
    /// <returns>로그인 성공 여부</returns>
    public async Task<bool> SignInWithEmailAndPasswordAsync(string email, string password)
    {
        try
        {
            var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            user = result.User;
            OnUserSignedIn?.Invoke(user);
            return true;
        }
        catch (Exception e)
        {
            OnError?.Invoke(e.Message);
            return false;
        }
    }

    /// <summary>
    /// 현재 로그인된 사용자를 로그아웃합니다.
    /// </summary>
    public void SignOut()
    {
        auth.SignOut();
    }

    // 프로필 관리
    /// <summary>
    /// 사용자의 프로필 정보를 업데이트합니다.
    /// </summary>
    /// <param name="displayName">표시 이름</param>
    /// <param name="photoUrl">프로필 사진 URL (선택사항)</param>
    /// <returns>업데이트 성공 여부</returns>
    public async Task<bool> UpdateUserProfileAsync(string displayName, string photoUrl = null)
    {
        try
        {
            var profile = new UserProfile
            {
                DisplayName = displayName,
                PhotoUrl = photoUrl != null ? new Uri(photoUrl) : null
            };
            await user.UpdateUserProfileAsync(profile);
            return true;
        }
        catch (Exception e)
        {
            OnError?.Invoke(e.Message);
            return false;
        }
    }

    // 계정 관리
    /// <summary>
    /// 사용자의 이메일 주소를 업데이트합니다.
    /// </summary>
    /// <param name="newEmail">새 이메일 주소</param>
    /// <returns>업데이트 성공 여부</returns>
    public async Task<bool> UpdateEmailAsync(string newEmail)
    {
        try
        {
            await user.UpdateEmailAsync(newEmail);
            return true;
        }
        catch (Exception e)
        {
            OnError?.Invoke(e.Message);
            return false;
        }
    }

    /// <summary>
    /// 사용자의 이메일 주소로 인증 메일을 발송합니다.
    /// </summary>
    /// <returns>메일 발송 성공 여부</returns>
    public async Task<bool> SendEmailVerificationAsync()
    {
        try
        {
            await user.SendEmailVerificationAsync();
            return true;
        }
        catch (Exception e)
        {
            OnError?.Invoke(e.Message);
            return false;
        }
    }

    /// <summary>
    /// 사용자의 비밀번호를 업데이트합니다.
    /// </summary>
    /// <param name="newPassword">새 비밀번호</param>
    /// <returns>업데이트 성공 여부</returns>
    public async Task<bool> UpdatePasswordAsync(string newPassword)
    {
        try
        {
            await user.UpdatePasswordAsync(newPassword);
            return true;
        }
        catch (Exception e)
        {
            OnError?.Invoke(e.Message);
            return false;
        }
    }

    /// <summary>
    /// 비밀번호 재설정 메일을 발송합니다.
    /// </summary>
    /// <param name="email">사용자 이메일</param>
    /// <returns>메일 발송 성공 여부</returns>
    public async Task<bool> SendPasswordResetEmailAsync(string email)
    {
        try
        {
            await auth.SendPasswordResetEmailAsync(email);
            return true;
        }
        catch (Exception e)
        {
            OnError?.Invoke(e.Message);
            return false;
        }
    }

    /// <summary>
    /// 사용자를 재인증합니다. 보안에 민감한 작업 전에 필요합니다.
    /// </summary>
    /// <param name="email">사용자 이메일</param>
    /// <param name="password">사용자 비밀번호</param>
    /// <returns>재인증 성공 여부</returns>
    public async Task<bool> ReauthenticateAsync(string email, string password)
    {
        try
        {
            var credential = EmailAuthProvider.GetCredential(email, password);
            await user.ReauthenticateAsync(credential);
            return true;
        }
        catch (Exception e)
        {
            OnError?.Invoke(e.Message);
            return false;
        }
    }

    /// <summary>
    /// 현재 로그인된 사용자의 계정을 삭제합니다.
    /// </summary>
    /// <returns>삭제 성공 여부</returns>
    public async Task<bool> DeleteUserAsync()
    {
        try
        {
            await user.DeleteAsync();
            return true;
        }
        catch (Exception e)
        {
            OnError?.Invoke(e.Message);
            return false;
        }
    }

    // 제공업체별 인증
    /// <summary>
    /// Google 계정으로 로그인합니다.
    /// </summary>
    /// <param name="idToken">Google ID 토큰</param>
    /// <param name="accessToken">Google 액세스 토큰</param>
    /// <returns>로그인 성공 여부</returns>
    public async Task<bool> SignInWithGoogleAsync(string idToken, string accessToken)
    {
        try
        {
            var credential = GoogleAuthProvider.GetCredential(idToken, accessToken);
            user = await auth.SignInWithCredentialAsync(credential);
            OnUserSignedIn?.Invoke(user);
            return true;
        }
        catch (Exception e)
        {
            OnError?.Invoke(e.Message);
            return false;
        }
    }

    /// <summary>
    /// 컴포넌트가 파괴될 때 이벤트 구독을 해제합니다.
    /// </summary>
    private void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
        }
    }
}
