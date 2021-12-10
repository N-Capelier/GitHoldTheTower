using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
	// Start is called before the first frame update

	public GameObject menuObject, lobbyObject;

	public TextMeshProUGUI ipInputText, usernameInputText, passwordInputText;

	private GameObject serverManager;
	MyNewNetworkManager networkManager;
	MyNewNetworkAuthenticator networkAuthenticator;

	private bool isHost = false;

	void Start()
	{
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		menuObject.SetActive(true);
		lobbyObject.SetActive(false);
		serverManager = GameObject.Find("ServerManager");
		networkManager = serverManager.GetComponent<MyNewNetworkManager>();
		networkAuthenticator = serverManager.GetComponent<MyNewNetworkAuthenticator>();
	}

	#region Button

	public void OnPressedHost(string _sceneName)
	{
		networkAuthenticator.lobbyPseudo = usernameInputText.text;
		networkAuthenticator.lobbyPassword = passwordInputText.text;
		networkManager.SetGameScene(_sceneName);

		changeMenu();
		isHost = true;
		networkManager.StartHost();
	}

	public void OnPressedJoin()
	{
		networkManager.networkAddress = ipInputText.text;
		networkAuthenticator.lobbyPseudo = usernameInputText.text;
		networkAuthenticator.lobbyPassword = passwordInputText.text;

		changeMenu();
		networkManager.StartClient();
	}

	public void OnPressedLeave()
	{
		changeMenu();
		if (isHost)
		{
			networkManager.StopHost();
			isHost = false;
		}
		else
		{
			networkManager.StopClient();
		}

	}

	#endregion

	public void changeMenu()
	{
		menuObject.SetActive(!menuObject.activeSelf);
		lobbyObject.SetActive(!lobbyObject.activeSelf);
	}
}
