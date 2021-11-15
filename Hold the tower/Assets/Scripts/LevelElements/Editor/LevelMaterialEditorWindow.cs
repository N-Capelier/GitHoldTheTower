using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class LevelMaterialEditorWindow : EditorWindow
{
	static LevelMaterialEditorWindow levelMaterialEditor;

	[MenuItem("Tools/Debug/Level Material Editor", priority = 2)]
	public static void Init()
	{
		levelMaterialEditor = GetWindow<LevelMaterialEditorWindow>("Level Material Editor");
	}

	GUIStyle textStyle;
	GUIStyle buttonStyle;

	void SetTextStyle()
	{
		//if (textStyle != null)
		//	return;

		textStyle = new GUIStyle();

		textStyle.fontStyle = FontStyle.Bold;
		textStyle.fontSize = 16;
		textStyle.normal.textColor = Color.white;
	}

	void SetButtonStyle()
	{
		//if (buttonStyle != null)
		//	return;

		buttonStyle = new GUIStyle(GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 16;
		buttonStyle.normal.textColor = Color.white;
	}

	ThemeManager themeManager;
	Material materialToReplace, newMaterial;

	private void OnGUI()
	{
		SetTextStyle();
		SetButtonStyle();

		EditorGUILayout.LabelField("Theme Manager:", textStyle);
		themeManager = (ThemeManager)EditorGUILayout.ObjectField(themeManager, typeof(ThemeManager), true);

		GUILayout.Space(10);

		EditorGUILayout.LabelField("Material to replace:", textStyle);
		materialToReplace = (Material)EditorGUILayout.ObjectField(materialToReplace, typeof(Material), false);

		GUILayout.Space(10);

		EditorGUILayout.LabelField("New material:", textStyle);
		newMaterial = (Material)EditorGUILayout.ObjectField(newMaterial, typeof(Material), false);

		GUILayout.Space(10);

		if (GUILayout.Button("Replace materials", buttonStyle))
		{
			Replace();
		}
	}

	private void Replace()
	{
		foreach(BlockBehaviour block in themeManager.blocks)
		{
			if(block.gameObject.GetComponent<MeshRenderer>().material.name.Contains("BaseB"))
			{
				block.gameObject.GetComponent<MeshRenderer>().material = newMaterial;
			}
		}
		Debug.Log("Replaced materials");
	}
}
