using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class LevelScalerWindow : EditorWindow
{
	static LevelScalerWindow levelScaler;

	[MenuItem("Tools/Debug/Level Corrector", priority = 1)]
	public static void Init()
	{
		levelScaler = GetWindow<LevelScalerWindow>("Level Corrector");
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
	int oldSize, newSize;

	private void OnGUI()
	{
		SetTextStyle();
		SetButtonStyle();

		EditorGUILayout.LabelField("Theme Manager:", textStyle);
		themeManager = (ThemeManager)EditorGUILayout.ObjectField(themeManager, typeof(ThemeManager), true);

		EditorGUILayout.Space(10);

		EditorGUILayout.LabelField("Old grid size:", textStyle);
		oldSize = EditorGUILayout.IntField(oldSize);

		EditorGUILayout.Space(10);

		EditorGUILayout.LabelField("New grid size:", textStyle);
		newSize = EditorGUILayout.IntField(newSize);

		EditorGUILayout.Space(10);

		if(GUILayout.Button("Resize blocks", buttonStyle))
		{
			Rescale();
		}
	}

	private void Rescale()
	{
		for (int i = 0; i < themeManager.blocks.Length; i++)
		{
			themeManager.blocks[i].transform.position = new Vector3(
				themeManager.blocks[i].transform.position.x * (newSize / oldSize),
				themeManager.blocks[i].transform.position.y,
				themeManager.blocks[i].transform.position.z * (newSize / oldSize));
		}
	}
}
