using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Functions;
using UnityEngine;


    /// <summary>
    /// Firebase Firestore 기능을 관리하는 매니저 클래스
    /// </summary>
    public class FirebaseFirestoreManager : ManagerBase
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
                }
                
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"Firestore 테스트 실패: {e.Message}");
                return false;
            }
        }
    }
