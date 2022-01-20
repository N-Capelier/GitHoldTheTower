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

	[Header("Join Menu Object")]
	public InputField nameToSaveTheIp;
	public GameObject buttonCustomType;

	private List<GameObject> allCustomButton = new List<GameObject>();
	[SerializeField]
	private List<RectTransform> allPositionCustomButton;

	[HideInInspector]
	[SerializeField] private Dictionary<string, string> ipLinkName = new Dictionary<string, string>();

	[Header("GameObject Menu")]
	public GameObject menuObject;
	public GameObject lobbyObject;
	public GameObject mainMenu;
	public GameObject joinMenu;
	public GameObject settingsMenu;
	public GameObject creditMenu;

	[Header("Effect component")]

	[SerializeField]
	private Text EnterToStart;

	private WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame(); //MERCI NICO

	void Start()
	{

		//Enable the start scene
		menuObject.SetActive(false);
		creditMenu.SetActive(false);
		mainMenu.SetActive(true);
		lobbyObject.SetActive(false);
		joinMenu.SetActive(false);
		settingsMenu.SetActive(false);

		//Load all the ip that will be create into button
		SaveManager.LoadParams(ref ipLinkName);

		//Create all button
		LoadAllLinkButton();

		//Load param from json
		SaveManager.LoadParams(ref menuParams);
		inputFieldPseudoText.text = menuParams.playerPseudo;
		ipInputText.text = menuParams.ipToJoin;
		usernameStartMenu.text = menuParams.playerPseudo;

        //Curso management
        UnityEngine.Cursor.visible = true;
		UnityEngine.Cursor.lockState = CursorLockMode.None;

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


				BackToMainMenu();

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

	public void OnPressedJoinCustom(string ipToInput)
	{
		menuParams.playerPseudo = usernameInputText.text;
		menuParams.ipToJoin = ipToInput;
		SaveManager.SaveParams(menuParams);

		networkManager.networkAddress = ipToInput;
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
		BackToMainMenu();
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
		creditMenu.SetActive(true);
		mainMenu.SetActive(false);
		lobbyObject.SetActive(true);
		joinMenu.SetActive(false);
		settingsMenu.SetActive(false);

		if (menuObject.activeSelf)
        {
			usernameInputText.text = menuParams.playerPseudo;
			ipInputText.text = menuParams.ipToJoin;
		}
	}

	public void OnPressedSettings() {
		//Enable the start scene
		menuObject.SetActive(false);
		creditMenu.SetActive(false);
		mainMenu.SetActive(true);
		lobbyObject.SetActive(false);
		joinMenu.SetActive(false);
		settingsMenu.SetActive(true);
	}

	public void OnPressedCredits()
    {
		//Enable the start scene
		menuObject.SetActive(false);
		creditMenu.SetActive(true);
		mainMenu.SetActive(false);
		lobbyObject.SetActive(false);
		joinMenu.SetActive(false);
		settingsMenu.SetActive(false);
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
		SaveManager.LoadParams(ref menuParams);
		inputFieldPseudoText.text = menuParams.playerPseudo;
		ipInputText.text = menuParams.ipToJoin;

		menuObject.SetActive(true);
		creditMenu.SetActive(false);
		mainMenu.SetActive(false);
		lobbyObject.SetActive(false);
		joinMenu.SetActive(false);
		settingsMenu.SetActive(false);
	}

	public void OnPressedCreateprefabButton()
    {
		ipLinkName.Add(nameToSaveTheIp.text, ipInputText.text);
		SaveManager.SaveParams(ref ipLinkName);
		LoadAllLinkButton();

	}

	public void OnPressedDestroyPrefabButton(string ipLinkKey)
    {
		ipLinkName.Remove(ipLinkKey);
		LoadAllLinkButton();
		SaveManager.SaveParams(ref ipLinkName);
	}

	public void LoadAllLinkButton()
    {
		int i = 0;

		foreach(GameObject but in allCustomButton)
        {
			Destroy(but);
		}

		foreach (string keyText in ipLinkName.Keys)
		{
			GameObject button = Instantiate(buttonCustomType, allPositionCustomButton[i]);
			button.GetComponentInChildren<Text>().text = keyText;
			button.GetComponent<Button>().onClick.AddListener(() => { OnPressedJoinCustom(ipLinkName[keyText]); });
			button.transform.Find("ButtonDestroy").GetComponent<Button>().onClick.AddListener(() => { OnPressedDestroyPrefabButton(keyText);});
			allCustomButton.Add(button);
			i++;
		}
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
