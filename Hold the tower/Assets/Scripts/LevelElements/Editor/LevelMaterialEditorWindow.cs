using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

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
	string materialToReplace;
	Material newMaterial;

	private void OnGUI()
	{
		SetTextStyle();
		SetButtonStyle();

		EditorGUILayout.LabelField("Theme Manager:", textStyle);
		themeManager = (ThemeManager)EditorGUILayout.ObjectField(themeManager, typeof(ThemeManager), true);

		GUILayout.Space(10);

		EditorGUILayout.LabelField("Block to affect:", textStyle);
		materialToReplace = EditorGUILayout.TextField(materialToReplace);

		GUILayout.Space(10);

		EditorGUILayout.LabelField("New material:", textStyle);
		newMaterial = (Material)EditorGUILayout.ObjectField(newMaterial, typeof(Material), false);

		GUILayout.Space(10);

		if (GUILayout.Button("Replace materials", buttonStyle))
		{
			Replace();
		}

		GUILayout.Space(10);

		if(GUILayout.Button("SetRandomRotation", buttonStyle))
		{
			Rotate();
		}
	}

	private void Rotate()
	{
		int rotation;
		foreach (BlockBehaviour block in themeManager.blocks)
		{
			if (block.gameObject.GetComponent<MeshRenderer>().material.name.Contains(materialToReplace))
			{
				rotation = Random.Range(0, 4) * 90;
				block.gameObject.transform.Rotate(new Vector3(0, rotation, 0));
			}
		}

		EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

		Debug.Log("Rotated blocks");
	}

	private void Replace()
	{
		foreach(BlockBehaviour block in themeManager.blocks)
		{
			if(block.gameObject.GetComponent<MeshRenderer>().material.name.Contains(materialToReplace))
			{
				block.gameObject.GetComponent<MeshRenderer>().material = newMaterial;
			}
		}

		EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

		Debug.Log("Replaced materials");
	}
}
