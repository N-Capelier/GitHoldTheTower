using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ThemeCreatorWindow : EditorWindow
{
	static ThemeCreatorWindow themeCreator;

	[MenuItem("Window/Tools/Theme Creator", priority = 1)]
	public static void Init()
	{
		themeCreator = GetWindow<ThemeCreatorWindow>("Theme Creator");
	}

	GUIStyle textStyle;
	GUIStyle errorStyle;
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

	void SetErrorStyle()
	{
		//if (errorStyle != null)
		//	return;

		errorStyle = new GUIStyle();

		errorStyle.fontStyle = FontStyle.Bold;
		errorStyle.fontSize = 16;
		errorStyle.normal.textColor = Color.red;
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

	string newThemeName;
	GameObject blocsParent;

	private void OnGUI()
	{
		SetTextStyle();
		SetErrorStyle();
		SetButtonStyle();

		EditorGUILayout.LabelField("Theme Name:", textStyle);
		newThemeName = EditorGUILayout.TextField(newThemeName);

		GUILayout.Label("");

		EditorGUILayout.LabelField("Blocs Parent:", textStyle);
		blocsParent = (GameObject)EditorGUILayout.ObjectField(blocsParent, typeof(GameObject), true);

		GUILayout.Label("");

		if(newThemeName == null || newThemeName == "")
		{
			EditorGUILayout.LabelField("Theme must have a name", errorStyle);
		}
		else if (blocsParent == null)
		{
			EditorGUILayout.LabelField("Blocs parent can not be empty", errorStyle);
		}
		else if(GUILayout.Button("Create Theme", buttonStyle))
		{
			CreateTheme();
		}
	}

	void CreateTheme()
	{
		try
		{
			PrefabUtility.SaveAsPrefabAsset(blocsParent, $"Assets/Prefabs/Level Elements/{newThemeName}");

			LevelTheme _newTheme = CreateInstance<LevelTheme>();

			_newTheme.name = newThemeName;
			_newTheme.prefab = (GameObject)AssetDatabase.LoadAssetAtPath($"Assets/Prefabs/Level Elements/{newThemeName}", typeof(GameObject));

			string _path = AssetDatabase.GenerateUniqueAssetPath($"Assets/Level Elements/Themes/{newThemeName}.asset");
			AssetDatabase.CreateAsset(_newTheme, _path);
			AssetDatabase.SaveAssets();

			EditorUtility.FocusProjectWindow();
			Selection.activeObject = _newTheme;
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
			return;
		}

		Debug.Log($"Created new theme: {newThemeName}");
	}
}
