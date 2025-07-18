using UnityEditor;
using UnityEngine;

public class FirebaseUserEditorWindow : EditorWindow
{
    [MenuItem("Firebase/UserInfo")]
    public static void ShowWindow()
    {
        FirebaseUserEditorWindow window = GetWindow<FirebaseUserEditorWindow>("Firebase User Editor");
        window.minSize = new UnityEngine.Vector2(400, 300);
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Firebase User Editor", EditorStyles.boldLabel);
        GUILayout.Label("This is a placeholder for Firebase user management features.");
        // Add your GUI elements here for managing Firebase users
        
        // CurrentUser
        var firebaseManager = App.GetManager<FirebaseManager>();
        if (firebaseManager != null && firebaseManager.CurrentUser != null)
        {
            // UserData
            var userData = firebaseManager.CurrentUserData;
            if (userData != null)
            {
                GUILayout.Label($"uid: {userData.uid}");
                GUILayout.Label($"Nickname: {userData.nickname}");
                GUILayout.Label($"CreatedAt: {userData.createdAt}");
                GUILayout.Label($"LastLoginAt: {userData.lastLoginAt}");
                GUILayout.Label($"Currencies");
                if (userData.currencies != null)
                {
                    GUILayout.Label($"별똥전: {userData.currencies.SDC}");
                    GUILayout.Label($"별빛가루: {userData.currencies.SLP}");
                    GUILayout.Label($"소원씨앗: {userData.currencies.WSD}");
                    GUILayout.Label($"행복방울: {userData.currencies.HDP}");
                }
            }
            else
            {
                GUILayout.Label("No user data available.");
            }
        }
        else
        {
            GUILayout.Label("No user is currently signed in.");
        }
    }
}