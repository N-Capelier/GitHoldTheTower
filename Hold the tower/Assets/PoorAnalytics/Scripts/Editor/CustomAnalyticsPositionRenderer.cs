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
    public void OnEnable()
    {
        pathFilePositions = serializedObject.FindProperty("pathFilePositions");
        drawnGizmos = serializedObject.FindProperty("drawnGizmos");
        //keyPositions = serializedObject.FindProperty("playersKeyPosition");
        colors = serializedObject.FindProperty("colors");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //Path to get the file
        EditorGUILayout.PropertyField(pathFilePositions);
        if(GUILayout.Button("Drawn In World"))
        {
            PA_RenderPosition renderPos = (PA_RenderPosition)target;
            renderPos.LoadKeyPositions();
        }
        EditorGUILayout.PropertyField(colors);
        //EditorGUILayout.PropertyField(keyPositions);

        EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }

}
