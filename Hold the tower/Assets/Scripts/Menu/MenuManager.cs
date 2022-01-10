using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using AnotherFileBrowser.Windows; //Librairy fais par un docteur en informatique indien, pas des lol, il a du devenir fou pour faire cett merde, merci à lui

public class MenuManager : MonoBehaviour
{
	// Start is called before the first frame update

	public GameObject menuObject, lobbyObject;

	public TextMeshProUGUI usernameInputText, passwordInputText;

	public Text ipInputText;

	public Text analyticsPath;
	[SerializeField]
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
		//serverManager = GameObject.Find("ServerManager");
		networkManager = serverManager.GetComponent<MyNewNetworkManager>();
		networkAuthenticator = serverManager.GetComponent<MyNewNetworkAuthenticator>();
	}

	#region Button

	public void OnPressedHost(string _sceneName)
	{
		networkAuthenticator.lobbyPseudo = usernameInputText.text;
		networkAuthenticator.lobbyPassword = passwordInputText.text;
		networkManager.SetGameScene(_sceneName);

		ChangeMenu();
		isHost = true;
		networkManager.StartHost();
	}

	public void OnPressedJoin()
	{
		networkManager.networkAddress = ipInputText.text; // ipText;//ipInputText.text;
		networkAuthenticator.lobbyPseudo = usernameInputText.text;
		networkAuthenticator.lobbyPassword = passwordInputText.text;

		ChangeMenu();
		networkManager.StartClient();
		
	}

	public void OnPressedLeave()
	{
		ChangeMenu();
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

	public void ChangeMenu()
	{
		menuObject.SetActive(!menuObject.activeSelf);
		lobbyObject.SetActive(!lobbyObject.activeSelf);
	}

	public void AnalyticsExplorer()
    {

		var bp = new BrowserProperties();
		bp.filter = "txt files (*.txt)|*.txt|All Files (*.*)|*.*";
		bp.filterIndex = 0;

		new FileBrowser().OpenFileBrowser(bp, path =>
		{
			analyticsPath.text = path;
			serverManager.GetComponent<MyNewNetworkManager>().analyticsPath = path;
		});


	}
}
