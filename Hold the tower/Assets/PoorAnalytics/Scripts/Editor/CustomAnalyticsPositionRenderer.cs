using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PA_RenderPosition))]
public class CustomAnalyticsPositionRenderer : Editor
{

    SerializedProperty pathFilePositions;
    SerializedProperty drawnGizmos;

    SerializedProperty keyPositions;
    SerializedProperty colors;
    SerializedProperty round;

    public void OnEnable()
    {
        pathFilePositions = serializedObject.FindProperty("pathFilePositions");
        drawnGizmos = serializedObject.FindProperty("drawnGizmos");
        //keyPositions = serializedObject.FindProperty("playersKeyPosition");
        colors = serializedObject.FindProperty("colors");
        round = serializedObject.FindProperty("round");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //Path to get the file
        EditorGUILayout.PropertyField(pathFilePositions);
        //Round to show
        EditorGUILayout.PropertyField(round);
        round.intValue = Mathf.Clamp(round.intValue, 0, 5);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Drawn In World"))
        {
            PA_RenderPosition renderPos = (PA_RenderPosition)target;
            renderPos.LoadKeyPositions();
        }
        if (GUILayout.Button("Load file Position"))
        {
            string path = EditorUtility.OpenFilePanel("Load Text positions", "", "txt");
            pathFilePositions.stringValue = path;
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(colors);
        //EditorGUILayout.PropertyField(keyPositions);

        EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }

}
