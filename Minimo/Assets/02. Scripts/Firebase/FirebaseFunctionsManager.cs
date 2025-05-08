using System;
using System.Threading.Tasks;
using Firebase.Functions;
using UnityEngine;


    /// <summary>
    /// Firebase Functions 기능을 관리하는 매니저 클래스
    /// </summary>
    public class FirebaseFunctionsManager : ManagerBase
    {
        private FirebaseFunctions functions;

        protected override void Awake()
        {
            base.Awake();
            functions = FirebaseFunctions.DefaultInstance;
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
                var data = (System.Collections.Generic.Dictionary<string, object>)result.Data;
                
                Debug.Log($"Firestore 테스트 결과: {data["message"]}");
                return (bool)data["success"];
            }
            catch (Exception e)
            {
                Debug.LogError($"Firestore 테스트 실패: {e.Message}");
                return false;
            }
        }
    }
