using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelEditorWindow : EditorWindow
{
	static LevelEditorWindow themeCreator;

	[MenuItem("Window/Tools/Level Editor", priority = 1)]
	public static void Init()
	{
		themeCreator = GetWindow<LevelEditorWindow>("Level Editor");
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

	static int currentTab = 0;
	static int registeredTab = 0;
	string[] toolbarTabsNames = { "Theme Creator", "Terrain Creator", "Terrain Editor" };

	ThemeManager themeManager;
	string newThemeName;
	string newTerrainName;

	private void OnGUI()
	{
		SetTextStyle();
		SetErrorStyle();
		SetButtonStyle();

		if(registeredTab != currentTab)
		{
			registeredTab = currentTab;
			newThemeName = null;
			newTerrainName = null;
			themeManager = null;
		}

		currentTab = GUILayout.Toolbar(currentTab, toolbarTabsNames);

		switch(currentTab)
		{
			case 0:
				DisplayThemeCreatorWindow();
				break;
			case 1:
				DisplayTerrainCreatorWindow();
				break;
			case 2:
				DisplayTerrainEditorWindow();
				break;
			default:
				break;
		}
	}

	void DisplayThemeCreatorWindow()
	{
		EditorGUILayout.LabelField("Theme Name:", textStyle);
		newThemeName = EditorGUILayout.TextField(newThemeName);

		EditorGUILayout.Space(10);

		EditorGUILayout.LabelField("Base Theme Manager:", textStyle);
		themeManager = (ThemeManager)EditorGUILayout.ObjectField(themeManager, typeof(ThemeManager), true);

		EditorGUILayout.Space(10);

		if (newThemeName == null || newThemeName == "")
		{
			EditorGUILayout.LabelField("Theme must have a name.", errorStyle);
		}
		else if (themeManager == null)
		{
			EditorGUILayout.LabelField("ThemeManager can not be null.", errorStyle);
		}
		else if (GUILayout.Button("Create Theme", buttonStyle))
		{
			CreateTheme();
		}
	}

	void DisplayTerrainCreatorWindow()
	{
		EditorGUILayout.LabelField("Terrain Name:", textStyle);
		newTerrainName = EditorGUILayout.TextField(newTerrainName);

		EditorGUILayout.Space(10);

		EditorGUILayout.LabelField("Theme Manager:", textStyle);
		themeManager = (ThemeManager)EditorGUILayout.ObjectField(themeManager, typeof(ThemeManager), true);

		EditorGUILayout.Space(10);

		if (newTerrainName == null || newTerrainName == "")
		{
			EditorGUILayout.LabelField("Terrain must have a name.", errorStyle);
		}
		else if (themeManager == null)
		{
			EditorGUILayout.LabelField("ThemeManager can not be null.", errorStyle);
		}
		else if (GUILayout.Button("Create Terrain", buttonStyle))
		{
			CreateTerrain();
		}
	}

	void DisplayTerrainEditorWindow()
	{
		EditorGUILayout.LabelField("Theme Manager:", textStyle);
		themeManager = (ThemeManager)EditorGUILayout.ObjectField(themeManager, typeof(ThemeManager), true);

		EditorGUILayout.Space(10);

		if (themeManager == null)
		{
			EditorGUILayout.LabelField("ThemeManager can not be null.", errorStyle);
			return;
		}

		EditorGUILayout.LabelField("Load Terrain:", textStyle);
		
		EditorGUILayout.BeginVertical();
		foreach(LevelTerrain _terrain in themeManager.terrains)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(50);
			if (GUILayout.Button($"Load {_terrain.terrainName}"))
			{
				themeManager.activeTerrain = _terrain;
				LoadTerrain();
			}
			GUILayout.Space(50);

			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space(10);

		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(50);
		if (GUILayout.Button("Save active Terrain"))
		{
			SaveTerrain();
		}
		GUILayout.Space(50);
		EditorGUILayout.EndHorizontal();
	}

	void CreateTheme()
	{
		themeManager.InitTerrainBlocks();

		PrefabUtility.SaveAsPrefabAsset(themeManager.gameObject, $"Assets/Prefabs/LevelElements/Themes/{newThemeName}.prefab");

		LevelTheme _newTheme = CreateInstance<LevelTheme>();

		_newTheme.themeName = newThemeName;

		_newTheme.prefab = (GameObject)AssetDatabase.LoadAssetAtPath($"Assets/Prefabs/LevelElements/Themes/{newThemeName}.prefab", typeof(GameObject));

		string _path = AssetDatabase.GenerateUniqueAssetPath($"Assets/LevelElements/Themes/{newThemeName}.asset");
		AssetDatabase.CreateAsset(_newTheme, _path);
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();
		Selection.activeObject = _newTheme;

		themeManager.blocks = null;

		Debug.Log($"Created new theme: {newThemeName}.");
	}

	void CreateTerrain()
	{
		LevelTerrain _newTerrain = CreateInstance<LevelTerrain>();

		_newTerrain.terrainName = newTerrainName;
		_newTerrain.positions = new Vector3[themeManager.blocks.Length];

		for (int i = 0; i < _newTerrain.positions.Length; i++)
		{
			////////////////////////////////// Could change to "= themeManager.blocks[i].tranform.child.position" if we use child object for the renderer
			_newTerrain.positions[i] = themeManager.blocks[i].transform.position;
		}

		string _path = AssetDatabase.GenerateUniqueAssetPath($"Assets/LevelElements/Terrains/{newTerrainName}.asset");
		AssetDatabase.CreateAsset(_newTerrain, _path);
		AssetDatabase.SaveAssets();

		themeManager.activeTerrain = (LevelTerrain)AssetDatabase.LoadAssetAtPath(_path, typeof(LevelTerrain));
		themeManager.terrains.Add((LevelTerrain)AssetDatabase.LoadAssetAtPath(_path, typeof(LevelTerrain)));

		EditorUtility.SetDirty(themeManager);
		PrefabUtility.RecordPrefabInstancePropertyModifications(themeManager);

		EditorUtility.FocusProjectWindow();
		Selection.activeObject = _newTerrain;

		Debug.Log($"Created new terrain: {newTerrainName}.");
	}

	void LoadTerrain()
	{
		for (int i = 0; i < themeManager.blocks.Length; i++)
		{
			themeManager.blocks[i].transform.position = themeManager.activeTerrain.positions[i];
		}

		Debug.Log($"Loaded terrain: {themeManager.activeTerrain.terrainName}.");
	}

	void SaveTerrain()
	{
		for (int i = 0; i < themeManager.blocks.Length; i++)
		{
			////////////////////////////////// Could change to themeManager.blocks[i].tranform.child.position if we use child object for the renderer
			themeManager.activeTerrain.positions[i] = themeManager.blocks[i].transform.position;
		}

		AssetDatabase.SaveAssets();

		Debug.Log($"Saved terrain: {themeManager.activeTerrain.terrainName}.");
	}
}
