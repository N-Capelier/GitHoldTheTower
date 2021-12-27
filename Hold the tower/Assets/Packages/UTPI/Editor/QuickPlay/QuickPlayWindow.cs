using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class QuickPlayWindow : EditorWindow
{
    static QuickPlayWindow window;

    [MenuItem("Window/General/Quick Play", priority = 1)]
    public static void Init()
    {
        window = GetWindow<QuickPlayWindow>("Quick Play");
        window.GetReferences();
    }

    Texture2D playImage, stopImage, pauseImage, unpauseImage;
    Texture2D currentPlayImage, currentPauseImage;

    const string lobbyScenePath = "Assets/Scenes/Game/LobbyScene.unity";
    string referenceScenePath;

    void GetReferences()
	{
        playImage = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Packages/UTPI/Editor/QuickPlay/Textures/playTexture.png");
        stopImage = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Packages/UTPI/Editor/QuickPlay/Textures/stopTexture.png");
        pauseImage = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Packages/UTPI/Editor/QuickPlay/Textures/pauseTexture.png");
        unpauseImage = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Packages/UTPI/Editor/QuickPlay/Textures/unpauseTexture.png");

        if(!EditorApplication.isPlaying)
		{
            currentPlayImage = playImage;
        }
        else
        {
            currentPlayImage = stopImage;
        }

        if(!EditorApplication.isPaused)
		{
            currentPauseImage = pauseImage;
        }
        else
		{
            currentPauseImage = unpauseImage;
		}
    }

	private void OnGUI()
	{
        if(playImage == null)
		{
            GetReferences();
		}

        GUILayout.BeginArea(new Rect((Screen.width / 2) - 40f, 5f, 80f, 40f));
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button(currentPlayImage, GUILayout.Width(40f), GUILayout.Height(40f)))
		{
            if(!EditorApplication.isPlaying)
			{
                EnterPlayMode();
			}
            else
			{
                ExitPlayMode();
			}
		}

        if(GUILayout.Button(currentPauseImage, GUILayout.Width(40f), GUILayout.Height(40f)))
		{
            if(EditorApplication.isPlaying)
			{
                if(!EditorApplication.isPaused)
				{
                    PausePlayMode();
				}
                else
				{
                    UnpausePlayMode();
				}
			}
		}

        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();
	}

    void EnterPlayMode()
	{
        currentPlayImage = stopImage;
		currentPauseImage = pauseImage;

		referenceScenePath = EditorSceneManager.GetActiveScene().path;

		if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
		{
			EditorSceneManager.OpenScene(lobbyScenePath);

            EditorApplication.EnterPlaymode();
        }
    }

	void ExitPlayMode()
	{
        currentPlayImage = playImage;
        currentPauseImage = pauseImage;

        EditorApplication.playModeStateChanged += OnPlayModeExited;

        EditorApplication.ExitPlaymode();
    }

    void OnPlayModeExited(PlayModeStateChange _newState)
	{
        if(_newState == PlayModeStateChange.EnteredEditMode)
		{
            EditorSceneManager.OpenScene(referenceScenePath);

            EditorApplication.playModeStateChanged -= OnPlayModeExited;
        }
    }

    void PausePlayMode()
	{
        currentPauseImage = unpauseImage;
        EditorApplication.isPaused = true;
	}

    void UnpausePlayMode()
    {
        currentPauseImage = pauseImage;
        EditorApplication.isPaused = false;
    }
}
