using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Functions;
using UnityEngine;

/// <summary>
/// Firebase Cloud Functions 기능을 관리하는 매니저 클래스
/// </summary>
public class FirebaseFunctionsManager : ManagerBase
{
    private FirebaseFunctions functions;

    protected override void Awake()
    {
        base.Awake();
        functions = FirebaseFunctions.DefaultInstance;
#if USE_EMULATOR
        functions.UseFunctionsEmulator("http://localhost:4000");
#endif
    }

    /// <summary>
    /// Firestore 테스트 함수를 호출합니다.
    /// </summary>
    /// <returns>테스트 결과</returns>
    public async Task<bool> TestFirestore()
    {
        try
        {
            var function = functions.GetHttpsCallable("testFirestore");
            var result = await function.CallAsync();
            
            if (result.Data is IDictionary data)
            {
                var message = data["message"]?.ToString();
                var success = Convert.ToBoolean(data["success"]);

                Debug.Log($"Firestore 테스트 결과: {message}");
                return success;
            }
            else
            {
                Debug.LogError("Firestore 테스트 결과가 유효하지 않습니다.");
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Firestore 테스트 실패: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 일반적인 HTTPS 호출 가능 함수를 실행합니다.
    /// </summary>
    /// <param name="functionName">호출할 함수 이름</param>
    /// <param name="data">함수에 전달할 데이터 (선택사항)</param>
    /// <returns>함수 실행 결과</returns>
    public async Task<object> CallFunction(string functionName, object data = null)
    {
        try
        {
            var function = functions.GetHttpsCallable(functionName);
            var result = await function.CallAsync(data);
            return result.Data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Firebase Function 호출 실패 ({functionName}): {e.Message}");
            throw; // 호출자가 오류를 처리할 수 있도록 예외를 다시 throw
        }
    }
}
