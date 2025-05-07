using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Firebase.Auth;

/// <summary>
/// Firebase Authentication 기능을 테스트하는 단위 테스트 클래스
/// </summary>
public class FirebaseUnitTest : MonoBehaviour
{
    private FirebaseAuthManager authManager;
    private const string TEST_EMAIL = "test@example.com";
    private const string TEST_PASSWORD = "test123456";
    private const string TEST_DISPLAY_NAME = "Test User";

    /// <summary>
    /// 테스트 시작 시 호출되는 메서드
    /// FirebaseAuthManager를 찾고 이벤트를 구독한 후 테스트를 시작합니다.
    /// </summary>
    async void Start()
    {
        authManager = FindObjectOfType<FirebaseAuthManager>();
        if (authManager == null)
        {
            Debug.LogError("씬에서 FirebaseAuthManager를 찾을 수 없습니다!");
            return;
        }

        // 이벤트 구독
        authManager.OnUserSignedIn += (user) => Debug.Log($"사용자 로그인: {user.DisplayName}");
        authManager.OnUserSignedOut += () => Debug.Log("사용자 로그아웃");
        authManager.OnError += (error) => Debug.LogError($"인증 오류: {error}");

        // Firebase 초기화 대기
        await Task.Delay(2000);
        
        // 테스트 시작
        await UnitTestFirebaseAuth();
    }

    /// <summary>
    /// Firebase Authentication의 모든 주요 기능을 순차적으로 테스트합니다.
    /// 각 테스트 단계 사이에 1초의 지연시간을 둡니다.
    /// </summary>
    private async Task UnitTestFirebaseAuth()
    {
        Debug.Log("=== Firebase 인증 단위 테스트 시작 ===");

        // 1. 새 사용자 생성 테스트
        Debug.Log("1. 이메일/비밀번호로 새 사용자 생성 테스트...");
        bool createResult = await authManager.CreateUserWithEmailAndPasswordAsync(TEST_EMAIL, TEST_PASSWORD);
        Debug.Log($"사용자 생성 결과: {createResult}");
        await Task.Delay(1000);

        // 2. 로그인 테스트
        Debug.Log("2. 이메일/비밀번호로 로그인 테스트...");
        bool signInResult = await authManager.SignInWithEmailAndPasswordAsync(TEST_EMAIL, TEST_PASSWORD);
        Debug.Log($"로그인 결과: {signInResult}");
        await Task.Delay(1000);

        // 3. 프로필 업데이트 테스트
        Debug.Log("3. 사용자 프로필 업데이트 테스트...");
        bool updateProfileResult = await authManager.UpdateUserProfileAsync(TEST_DISPLAY_NAME);
        Debug.Log($"프로필 업데이트 결과: {updateProfileResult}");
        await Task.Delay(1000);

        // 4. 이메일 인증 메일 발송 테스트
        Debug.Log("4. 이메일 인증 메일 발송 테스트...");
        bool emailVerificationResult = await authManager.SendEmailVerificationAsync();
        Debug.Log($"이메일 인증 메일 발송 결과: {emailVerificationResult}");
        await Task.Delay(1000);

        // 5. 비밀번호 재설정 메일 발송 테스트
        Debug.Log("5. 비밀번호 재설정 메일 발송 테스트...");
        bool passwordResetResult = await authManager.SendPasswordResetEmailAsync(TEST_EMAIL);
        Debug.Log($"비밀번호 재설정 메일 발송 결과: {passwordResetResult}");
        await Task.Delay(1000);

        // 6. 재인증 테스트
        Debug.Log("6. 사용자 재인증 테스트...");
        bool reauthResult = await authManager.ReauthenticateAsync(TEST_EMAIL, TEST_PASSWORD);
        Debug.Log($"재인증 결과: {reauthResult}");
        await Task.Delay(1000);

        // 7. 로그아웃 테스트
        Debug.Log("7. 로그아웃 테스트...");
        authManager.SignOut();
        Debug.Log("로그아웃 완료");
        await Task.Delay(1000);

        // 8. 다시 로그인
        Debug.Log("8. 다시 로그인 테스트...");
        signInResult = await authManager.SignInWithEmailAndPasswordAsync(TEST_EMAIL, TEST_PASSWORD);
        Debug.Log($"로그인 결과: {signInResult}");
        await Task.Delay(1000);

        // 9. 계정 삭제 테스트
        Debug.Log("9. 사용자 계정 삭제 테스트...");
        bool deleteResult = await authManager.DeleteUserAsync();
        Debug.Log($"계정 삭제 결과: {deleteResult}");

        Debug.Log("=== Firebase 인증 단위 테스트 완료 ===");
    }

    // Update is called once per frame
    void Update()
    {
        // Update 메서드는 비워둡니다
    }
}
