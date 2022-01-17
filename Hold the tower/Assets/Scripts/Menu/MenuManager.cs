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
	public TMP_InputField inputFieldPseudoText;

	public Text analyticsPath;
	[SerializeField]
	private GameObject serverManager;
	MyNewNetworkManager networkManager;
	MyNewNetworkAuthenticator networkAuthenticator;

	[SerializeField]
	private ScriptableMenuParams menuParams;

	public string customIp;
	public string customIp2;

	private bool isHost = false;

	void Start()
	{
		inputFieldPseudoText.text = menuParams.playerPseudo;
		ipInputText.text = menuParams.ipToJoin;
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
		menuParams.playerPseudo = usernameInputText.text;
		menuParams.ipToJoin = ipInputText.text;
		networkAuthenticator.lobbyPseudo = usernameInputText.text;
		networkAuthenticator.lobbyPassword = passwordInputText.text;
		networkManager.SetGameScene(_sceneName);

		ChangeMenu();
		isHost = true;
		networkManager.StartHost();
	}

	public void OnPressedJoin()
	{
		menuParams.playerPseudo = usernameInputText.text;
		menuParams.ipToJoin = ipInputText.text;
		networkManager.networkAddress = ipInputText.text; // ipText;//ipInputText.text;
		networkAuthenticator.lobbyPseudo = usernameInputText.text;
		networkAuthenticator.lobbyPassword = passwordInputText.text;

		ChangeMenu();
		networkManager.StartClient();
		
	}

	public void OnPressedCustom()
    {
		networkManager.networkAddress = customIp;
		networkAuthenticator.lobbyPseudo = usernameInputText.text;
		networkAuthenticator.lobbyPassword = passwordInputText.text;

		ChangeMenu();
		networkManager.StartClient();
	}

	public void OnPressedCustom2()
	{
		networkManager.networkAddress = customIp2;
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

        if (menuObject.activeSelf)
        {
			usernameInputText.text = menuParams.playerPseudo;
			ipInputText.text = menuParams.ipToJoin;
		}
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
