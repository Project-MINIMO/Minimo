using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Functions;
using UnityEngine;
using UnityEditor;

public class FirebaseCheat
{
    [MenuItem("FirebaseCheat/현재 계정 삭제")]
    private static async void DeleteCurrentAccount()
    {
        // 플레이 중일 경우 불가
        if (Application.isPlaying)
        {
            Debug.LogWarning("게임이 실행 중일 때는 계정을 삭제할 수 없습니다.");
            return;
        }
        
        if (EditorUtility.DisplayDialog("계정 삭제", "현재 계정을 삭제하시겠습니까?", "삭제", "취소"))
        {
            await FirebaseAuth.DefaultInstance.CurrentUser.DeleteAsync();
        }
    }
}
