using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using AnotherFileBrowser.Windows; //Librairy fais par un docteur en informatique indien, pas des lol, il a du devenir fou pour faire cett merde, merci à lui

public class MenuManager : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField]
	private GameObject serverManager;
	MyNewNetworkManager networkManager;
	MyNewNetworkAuthenticator networkAuthenticator;

	[SerializeField]
	private ScriptableMenuParams menuParams;

	// Start is called before the first frame update
	[Header("StartMenu")]
	public InputField usernameStartMenu;

	[Header("Main Menu Object")]
	public Text ipInputText;
	public TextMeshProUGUI usernameInputText, passwordInputText;
	public TMP_InputField inputFieldPseudoText;

	public Text analyticsPath;

	public string customIp;
	public string customIp2;

	private bool isHost = false;

	[Header("GameObject Menu")]
	public GameObject menuObject;
	public GameObject lobbyObject;
	public GameObject mainMenu;
	public GameObject joinMenu;

	[Header("Effect component")]

	[SerializeField]
	private Text EnterToStart;

	private WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame(); //MERCI NICO

	void Start()
	{
		menuObject.SetActive(false);
		lobbyObject.SetActive(false);
		joinMenu.SetActive(false);
		mainMenu.SetActive(true);

		//Load param from json
		SaveManager.LoadParams(ref menuParams);
		inputFieldPseudoText.text = menuParams.playerPseudo;
		ipInputText.text = menuParams.ipToJoin;
		usernameStartMenu.text = menuParams.playerPseudo;

		//Curso management
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;

		//Find server component
		networkManager = serverManager.GetComponent<MyNewNetworkManager>();
		networkAuthenticator = serverManager.GetComponent<MyNewNetworkAuthenticator>();

		//Start all my effect
		TextBlink(1f, EnterToStart);
	}

    private void Update()
    {
        if (mainMenu.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
				menuParams.playerPseudo = usernameStartMenu.text;
				SaveManager.SaveParams(menuParams);
				SaveManager.LoadParams(ref menuParams);
				Debug.Log(usernameStartMenu.text);

				menuObject.SetActive(true);
				mainMenu.SetActive(false);
				lobbyObject.SetActive(false);
				joinMenu.SetActive(false);

			}
        }

        if (menuObject.activeSelf)
        {

        }

        if (lobbyObject.activeSelf)
        {

        }

        if (joinMenu.activeSelf)
        {

        }
    }

    #region Button

    public void OnPressedHost(string _sceneName)
	{
		menuParams.playerPseudo = usernameInputText.text;
		menuParams.ipToJoin = ipInputText.text;
		SaveManager.SaveParams(menuParams);

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
		SaveManager.SaveParams(menuParams);

		networkManager.networkAddress = ipInputText.text;
		networkAuthenticator.lobbyPseudo = usernameInputText.text;
		networkAuthenticator.lobbyPassword = passwordInputText.text;

		ChangeMenu();
		networkManager.StartClient();
		
	}

	public void OnPressedJoinMenu()
    {
		menuObject.SetActive(false);
		lobbyObject.SetActive(false);
		joinMenu.SetActive(true);
		mainMenu.SetActive(false);
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



	public void ChangeMenu()
	{
		menuObject.SetActive(false);
		joinMenu.SetActive(false);
		lobbyObject.SetActive(true);

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

	public void ExitGame()
    {
		Application.Quit();
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#endif

		Application.Quit();

	}

	public void BackToMainMenu()
    {
		menuObject.SetActive(true);
		mainMenu.SetActive(false);
		lobbyObject.SetActive(false);
		joinMenu.SetActive(false);
    }
	#endregion

	#region effect

	//Use this to create a blink during all time
	public void TextBlink(float timeOfOneBlink, Text textToBlink)
    {
		StartCoroutine(ManageTextBlink(timeOfOneBlink, textToBlink));
	}

	public IEnumerator ManageTextBlink(float timeOfOneBlink,Text textToBlink)
    {
		float fadeValue = 1f;
		while (true)
        {
			
			while (fadeValue > 0f)
            {
				fadeValue = textToBlink.color.a - Time.deltaTime / timeOfOneBlink;
				textToBlink.color = new Color(textToBlink.color.r, textToBlink.color.g, textToBlink.color.b, fadeValue);//textToBlink.color.a - Time.deltaTime/ timeOfOneBlink);
				yield return endOfFrame;
			}

			while (fadeValue < 1f)
            {
				fadeValue = textToBlink.color.a + Time.deltaTime / timeOfOneBlink;
				textToBlink.color = new Color(textToBlink.color.r, textToBlink.color.g, textToBlink.color.b, fadeValue);
				yield return endOfFrame;
			}
			yield return endOfFrame;
        }
    }

#endregion

}
