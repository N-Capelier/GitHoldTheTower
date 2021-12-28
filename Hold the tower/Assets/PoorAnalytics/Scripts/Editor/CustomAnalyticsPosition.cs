using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(PA_Position))]
public class CustomAnalyticsPosition : Editor
{
    SerializedProperty pathFileToStorePosition;
    SerializedProperty timeEachBreak;
    SerializedProperty analyticGameObjectPosition;
    SerializedProperty gameManager;
    public void OnEnable()
    {
        gameManager = serializedObject.FindProperty("gameManager");
        pathFileToStorePosition = serializedObject.FindProperty("fileToStorePosition");
        timeEachBreak = serializedObject.FindProperty("timeEachBreak");
        analyticGameObjectPosition = serializedObject.FindProperty("analyticGameObjectPosition");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //Game Manger
        EditorGUILayout.PropertyField(gameManager);
        //Text file were are saved position
        EditorGUILayout.PropertyField(pathFileToStorePosition);
        //Show time break point
        EditorGUILayout.PropertyField(timeEachBreak);
        timeEachBreak.floatValue = Mathf.Clamp(timeEachBreak.floatValue,0.01f,float.MaxValue);

        GUILayout.BeginHorizontal();

        //Create empty file to store position
        if(GUILayout.Button("New Text File"))
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Position", "positions", "txt","");
            pathFileToStorePosition.stringValue = path;
            try
            {
                FileStream fs = File.Create(path);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
            
        }
        //Load File to Store position
        if (GUILayout.Button("Load Text File"))
        {
            string path = EditorUtility.OpenFilePanel("Load Text positions", "", "txt");
            pathFileToStorePosition.stringValue = path;
        }

        GUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(analyticGameObjectPosition);

        EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }

}
