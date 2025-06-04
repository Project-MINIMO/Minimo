using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;

/// <summary>
/// Firebase Firestore 데이터베이스 기능을 관리하는 매니저 클래스
/// </summary>
public class FirebaseFirestoreManager : ManagerBase
{
    private FirebaseFirestore db;
    private FirebaseFunctionsManager functionsManager;

    // protected override void Awake()
    // {
    //     base.Awake();
    //     db = FirebaseFirestore.DefaultInstance;
    //     functionsManager = GetComponent<FirebaseFunctionsManager>() ?? FindObjectOfType<FirebaseFunctionsManager>();
    //     
    //     if (functionsManager == null)
    //     {
    //         Debug.LogWarning("FirebaseFunctionsManager를 찾을 수 없습니다. Function 호출 관련 기능은 사용할 수 없습니다.");
    //     }
    // }
    //
    // /// <summary>
    // /// Firestore 테스트 함수를 호출합니다. (Functions 매니저를 통해 호출)
    // /// </summary>
    // /// <returns>테스트 결과</returns>
    // public async Task<bool> TestFirestore()
    // {
    //     if (functionsManager == null)
    //     {
    //         Debug.LogError("FirebaseFunctionsManager가 없어 테스트를 수행할 수 없습니다.");
    //         return false;
    //     }
    //     
    //     return await functionsManager.TestFirestore();
    // }
    //
    // /// <summary>
    // /// Firestore에서 문서를 가져옵니다.
    // /// </summary>
    // /// <param name="collection">컬렉션 이름</param>
    // /// <param name="documentId">문서 ID</param>
    // /// <returns>문서 데이터</returns>
    // public async Task<Dictionary<string, object>> GetDocument(string collection, string documentId)
    // {
    //     try
    //     {
    //         DocumentSnapshot snapshot = await db.Collection(collection).Document(documentId).GetSnapshotAsync();
    //         
    //         if (snapshot.Exists)
    //         {
    //             return snapshot.ToDictionary();
    //         }
    //         else
    //         {
    //             Debug.Log($"문서가 존재하지 않습니다: {collection}/{documentId}");
    //             return null;
    //         }
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.LogError($"Firestore 문서 가져오기 실패: {e.Message}");
    //         return null;
    //     }
    // }
    //
    // /// <summary>
    // /// Firestore에 문서를 저장합니다.
    // /// </summary>
    // /// <param name="collection">컬렉션 이름</param>
    // /// <param name="documentId">문서 ID (null이면 자동 생성)</param>
    // /// <param name="data">저장할 데이터</param>
    // /// <returns>성공 여부</returns>
    // public async Task<bool> SetDocument(string collection, string documentId, Dictionary<string, object> data)
    // {
    //     try
    //     {
    //         if (string.IsNullOrEmpty(documentId))
    //         {
    //             // 자동 ID 생성
    //             await db.Collection(collection).AddAsync(data);
    //         }
    //         else
    //         {
    //             // 지정된 ID 사용
    //             await db.Collection(collection).Document(documentId).SetAsync(data);
    //         }
    //         
    //         return true;
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.LogError($"Firestore 문서 저장 실패: {e.Message}");
    //         return false;
    //     }
    // }
    //
    // /// <summary>
    // /// Firestore 문서를 업데이트합니다.
    // /// </summary>
    // /// <param name="collection">컬렉션 이름</param>
    // /// <param name="documentId">문서 ID</param>
    // /// <param name="data">업데이트할 데이터</param>
    // /// <returns>성공 여부</returns>
    // public async Task<bool> UpdateDocument(string collection, string documentId, Dictionary<string, object> data)
    // {
    //     try
    //     {
    //         await db.Collection(collection).Document(documentId).UpdateAsync(data);
    //         return true;
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.LogError($"Firestore 문서 업데이트 실패: {e.Message}");
    //         return false;
    //     }
    // }
    //
    // /// <summary>
    // /// Firestore 문서를 삭제합니다.
    // /// </summary>
    // /// <param name="collection">컬렉션 이름</param>
    // /// <param name="documentId">문서 ID</param>
    // /// <returns>성공 여부</returns>
    // public async Task<bool> DeleteDocument(string collection, string documentId)
    // {
    //     try
    //     {
    //         await db.Collection(collection).Document(documentId).DeleteAsync();
    //         return true;
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.LogError($"Firestore 문서 삭제 실패: {e.Message}");
    //         return false;
    //     }
    // }
}
