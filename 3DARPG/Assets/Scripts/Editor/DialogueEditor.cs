using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;

[CustomEditor(typeof(DialogueData_SO))]
public class DialogueCustomEditor:Editor
{
    public override void OnInspectorGUI()
    {
        //如果按下按钮，就打开窗口
        if(GUILayout.Button("Open in Editor"))
        {
            DialogueEditor.InitWindow((DialogueData_SO)target);
        }
        base.OnInspectorGUI();
    }
}

public class DialogueEditor : EditorWindow
{
    DialogueData_SO currentData;

    ReorderableList pieceList = null;

    [MenuItem("Tong/Dialogue Editor")]
    public static void Init()
    {
        DialogueEditor editorWindow = GetWindow<DialogueEditor>("Dialogue Editor");

        //重新绘制窗口
        editorWindow.autoRepaintOnSceneChange = true;
    }

    public static void InitWindow(DialogueData_SO data)
    {
        DialogueEditor editorWindow = GetWindow<DialogueEditor>("Dialogue Editor");
        editorWindow.currentData = data;
    }

    [OnOpenAsset]
    public static bool OpenAsset(int instanceID,int line)
    {
        DialogueData_SO data = EditorUtility.InstanceIDToObject(instanceID) as DialogueData_SO;

        if(data != null)
        {
            DialogueEditor.InitWindow(data);
            return true;
        }
        return false;
    }

    private void OnGUI()
    {
        if(currentData != null)
        {
            EditorGUILayout.LabelField(currentData.name, EditorStyles.boldLabel);
            GUILayout.Space(10);
            if (pieceList == null)
                SetupReorderableList();
            pieceList.DoLayoutList();
        }
        else
        {
            GUILayout.Label("No data", EditorStyles.boldLabel);
        }
    }

    private void SetupReorderableList()
    {
        pieceList = new ReorderableList(currentData.dialoguePieces, typeof(DialoguePiece), true, true, true, true);
    }
}
