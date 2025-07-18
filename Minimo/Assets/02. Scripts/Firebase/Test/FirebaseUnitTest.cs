using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase;

/// <summary>
/// Firebase 서비스들을 테스트하는 통합 단위 테스트 클래스
/// </summary>
public class FirebaseUnitTest : MonoBehaviour
{
    private FirebaseManager _manager;
    private FirebaseFirestoreManager firestoreManager;
    private FirebaseFunctionsManager functionsManager;
    private FirebaseAuth auth;
    private bool isFirebaseInitialized = false;
    
    private const string TEST_EMAIL = "test@example.com";
    private const string TEST_PASSWORD = "test123456";
    private const string TEST_DISPLAY_NAME = "Test User";
    
    [SerializeField] bool runAuthTest = true;
    [SerializeField] bool runFunctionsTest = true;
    [SerializeField] bool runFirestoreTest = true;

    /// <summary>
    /// 테스트 시작 시 호출되는 메서드
    /// Firebase 매니저들을 찾고 초기화한 후 테스트를 시작합니다.
    /// </summary>
    async void Start()
    {
        Debug.Log("=== Firebase 통합 테스트 시작 ===");
        
        // Firebase 초기화
        await InitializeFirebaseAsync();
        if (!isFirebaseInitialized)
        {
            Debug.LogError("Firebase 초기화 실패!");
            return;
        }
        
        // 매니저들 찾기
        _manager = FindObjectOfType<FirebaseManager>();
        firestoreManager = FindObjectOfType<FirebaseFirestoreManager>();
        functionsManager = FindObjectOfType<FirebaseFunctionsManager>();
        
        if (_manager == null)
        {
            Debug.LogError("씬에서 FirebaseAuthManager를 찾을 수 없습니다!");
            return;
        }
        
        if (firestoreManager == null)
        {
            Debug.LogError("씬에서 FirebaseFirestoreManager를 찾을 수 없습니다!");
            return;
        }
        
        if (functionsManager == null)
        {
            Debug.LogError("씬에서 FirebaseFunctionsManager를 찾을 수 없습니다!");
            return;
        }

        // 이벤트 구독
        _manager.OnUserSignedIn += (user) => Debug.Log($"사용자 로그인: {user.DisplayName}");
        _manager.OnUserSignedOut += () => Debug.Log("사용자 로그아웃");
        _manager.OnError += (error) => Debug.LogError($"인증 오류: {error}");

        if (runAuthTest)
        {
            await UnitTestFirebaseAuth();
            await Task.Delay(1000);
        }

        if (runFunctionsTest)
        {
            await UnitTestFirebaseFunctions();
            await Task.Delay(1000);
        }
        
        if (runFirestoreTest)
        {
            await UnitTestFirebaseFirestore();
        }
        
        Debug.Log("=== Firebase 통합 테스트 완료 ===");
    }

    /// <summary>
    /// Firebase 초기화 및 인증 설정을 수행합니다.
    /// </summary>
    private async Task InitializeFirebaseAsync()
    {
        Debug.Log("Firebase 초기화 중...");
        
        // Firebase 초기화 확인
        await Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Firebase 초기화 성공
                auth = FirebaseAuth.DefaultInstance;
                isFirebaseInitialized = true;
                Debug.Log("Firebase 초기화 성공!");
                
                // 익명 로그인 시도
                if (auth.CurrentUser == null)
                {
                    Debug.Log("익명 인증 시도 중...");
                    auth.SignInAnonymouslyAsync().ContinueWith(authTask => {
                        if (authTask.IsCanceled || authTask.IsFaulted)
                        {
                            Debug.LogError($"익명 인증 실패: {authTask.Exception}");
                            return;
                        }
                        
                        Debug.Log($"익명 인증 성공! User ID: {auth.CurrentUser.UserId}");
                    });
                }
                else
                {
                    Debug.Log($"이미 인증된 사용자: {auth.CurrentUser.UserId}");
                }
            }
            else
            {
                Debug.LogError($"Firebase 초기화 실패: {dependencyStatus}");
                isFirebaseInitialized = false;
            }
        });
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
        bool createResult = await _manager.CreateUserWithEmailAndPasswordAsync(TEST_EMAIL, TEST_PASSWORD);
        Debug.Log($"사용자 생성 결과: {createResult}");
        await Task.Delay(1000);

        // 2. 로그인 테스트
        Debug.Log("2. 이메일/비밀번호로 로그인 테스트...");
        bool signInResult = await _manager.SignInWithEmailAndPasswordAsync(TEST_EMAIL, TEST_PASSWORD);
        Debug.Log($"로그인 결과: {signInResult}");
        await Task.Delay(1000);
        
        // 2.5 GetAccountInfo FireFunction Test
        Debug.Log("2.5. 사용자 정보 가져오기 테스트...");
        

        // 3. 프로필 업데이트 테스트
        Debug.Log("3. 사용자 프로필 업데이트 테스트...");
        bool updateProfileResult = await _manager.UpdateUserProfileAsync(TEST_DISPLAY_NAME);
        Debug.Log($"프로필 업데이트 결과: {updateProfileResult}");
        await Task.Delay(1000);

        // 4. 이메일 인증 메일 발송 테스트
        Debug.Log("4. 이메일 인증 메일 발송 테스트...");
        bool emailVerificationResult = await _manager.SendEmailVerificationAsync();
        Debug.Log($"이메일 인증 메일 발송 결과: {emailVerificationResult}");
        await Task.Delay(1000);

        // 5. 비밀번호 재설정 메일 발송 테스트
        Debug.Log("5. 비밀번호 재설정 메일 발송 테스트...");
        bool passwordResetResult = await _manager.SendPasswordResetEmailAsync(TEST_EMAIL);
        Debug.Log($"비밀번호 재설정 메일 발송 결과: {passwordResetResult}");
        await Task.Delay(1000);

        // 6. 재인증 테스트
        Debug.Log("6. 사용자 재인증 테스트...");
        bool reauthResult = await _manager.ReauthenticateAsync(TEST_EMAIL, TEST_PASSWORD);
        Debug.Log($"재인증 결과: {reauthResult}");
        await Task.Delay(1000);

        // 7. 로그아웃 테스트
        Debug.Log("7. 로그아웃 테스트...");
        _manager.SignOut();
        Debug.Log("로그아웃 완료");
        await Task.Delay(1000);

        // 8. 다시 로그인
        Debug.Log("8. 다시 로그인 테스트...");
        signInResult = await _manager.SignInWithEmailAndPasswordAsync(TEST_EMAIL, TEST_PASSWORD);
        Debug.Log($"로그인 결과: {signInResult}");
        await Task.Delay(1000);

        // 9. 계정 삭제 테스트
        Debug.Log("9. 사용자 계정 삭제 테스트...");
        bool deleteResult = await _manager.DeleteUserAsync();
        Debug.Log($"계정 삭제 결과: {deleteResult}");

        Debug.Log("=== Firebase 인증 단위 테스트 완료 ===");
    }
    
    /// <summary>
    /// Firebase Functions의 기능을 테스트합니다.
    /// </summary>
    private async Task UnitTestFirebaseFunctions()
    {
        Debug.Log("=== Firebase Functions 테스트 시작 ===");
        
        // 사용자가 로그인될 때까지 대기
        if (auth.CurrentUser == null)
        {
            Debug.Log("Functions 테스트를 위해 익명 로그인이 필요합니다.");
            
            // 익명 로그인 시도
            try
            {
                await auth.SignInAnonymouslyAsync();
                Debug.Log($"익명 인증 성공! User ID: {auth.CurrentUser.UserId}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"익명 인증 실패: {ex.Message}");
                return;
            }
        }
        
        while (auth.CurrentUser == null)
        {
            await Task.Delay(500);
            Debug.Log("사용자 로그인 대기 중...");
        }
        
        if (FirebaseAuth.DefaultInstance.CurrentUser == null)
        {
            Debug.LogError("로그인이 필요합니다.");
            return;
        }
        
        // Firebase Functions 테스트
        Debug.Log("Firestore Function 테스트 시작...");
        bool result = await functionsManager.TestFirestore();
        
        if (result)
        {
            Debug.Log("Firestore Function 테스트 성공!");
        }
        else
        {
            Debug.LogError("Firestore Function 테스트 실패!");
        }
        
        // 익명 계정 삭제
        if (auth.CurrentUser != null)
        {
            try
            {
                await auth.CurrentUser.DeleteAsync();
                Debug.Log("익명 계정 삭제 성공");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"익명 계정 삭제 실패: {ex.Message}");
            }
        }
        
        Debug.Log("=== Firebase Functions 테스트 완료 ===");
    }
    
    /// <summary>
    /// Firebase Firestore의 CRUD 기능을 테스트합니다.
    /// </summary>
    private async Task UnitTestFirebaseFirestore()
    {
        Debug.Log("=== Firebase Firestore 테스트 시작 ===");
        
        // 사용자가 로그인될 때까지 대기
        if (auth.CurrentUser == null)
        {
            Debug.Log("Firestore 테스트를 위해 익명 로그인이 필요합니다.");
            
            try
            {
                await auth.SignInAnonymouslyAsync();
                Debug.Log($"익명 인증 성공! User ID: {auth.CurrentUser.UserId}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"익명 인증 실패: {ex.Message}");
                return;
            }
        }
        
        string userId = auth.CurrentUser.UserId;
        string testCollection = "unitTest";
        string testDocId = $"test_{userId}";
        
        // 1. 문서 생성 테스트
        Debug.Log("1. Firestore 문서 생성 테스트...");
        Dictionary<string, object> testData = new Dictionary<string, object>
        {
            { "name", "테스트 데이터" },
            { "value", 42 },
            { "isTest", true },
            { "timestamp", Firebase.Firestore.FieldValue.ServerTimestamp }
        };
        
        bool createResult = await firestoreManager.SetDocument(testCollection, testDocId, testData);
        Debug.Log($"문서 생성 결과: {createResult}");
        await Task.Delay(1000);
        
        // 2. 문서 읽기 테스트
        Debug.Log("2. Firestore 문서 읽기 테스트...");
        Dictionary<string, object> readData = await firestoreManager.GetDocument(testCollection, testDocId);
        
        if (readData != null)
        {
            Debug.Log($"문서 읽기 성공: 이름={readData["name"]}, 값={readData["value"]}");
        }
        else
        {
            Debug.LogError("문서 읽기 실패!");
        }
        await Task.Delay(1000);
        
        // 3. 문서 업데이트 테스트
        Debug.Log("3. Firestore 문서 업데이트 테스트...");
        Dictionary<string, object> updateData = new Dictionary<string, object>
        {
            { "value", 100 },
            { "updated", true }
        };
        
        bool updateResult = await firestoreManager.UpdateDocument(testCollection, testDocId, updateData);
        Debug.Log($"문서 업데이트 결과: {updateResult}");
        await Task.Delay(1000);
        
        // 4. 업데이트된 문서 읽기
        Debug.Log("4. 업데이트된 Firestore 문서 읽기 테스트...");
        Dictionary<string, object> updatedData = await firestoreManager.GetDocument(testCollection, testDocId);
        
        if (updatedData != null)
        {
            Debug.Log($"업데이트된 문서 읽기 성공: 이름={updatedData["name"]}, 값={updatedData["value"]}, 업데이트됨={updatedData["updated"]}");
        }
        else
        {
            Debug.LogError("업데이트된 문서 읽기 실패!");
        }
        await Task.Delay(1000);
        
        // 5. 문서 삭제 테스트
        Debug.Log("5. Firestore 문서 삭제 테스트...");
        bool deleteResult = await firestoreManager.DeleteDocument(testCollection, testDocId);
        Debug.Log($"문서 삭제 결과: {deleteResult}");
        
        Debug.Log("=== Firebase Firestore 테스트 완료 ===");
    }
}
