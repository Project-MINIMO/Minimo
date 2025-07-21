using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
// (Optional) Inspector에 버튼을 추가하고 싶다면 이걸 같이 추가
[CustomEditor(typeof(QuestTester))]
public class QuestTesterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(6);
        if (GUILayout.Button("Add Quest")) 
            ((QuestTester)target).AddQuest();
        if (GUILayout.Button("Remove Quest")) 
            ((QuestTester)target).RemoveQuest();
    }
}
#endif