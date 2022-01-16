using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System.Collections;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/components/network-manager
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/
public class MyNewNetworkManager : NetworkManager
{
    [Header("Params")]
    public GameObject lobbyPlayer;
    public GameObject Player;
    public GameObject spectator;
    public GameObject MenuManagerObject;
    public GameObject StartButton;
    public GameObject TextInputIp;
    public GameObject textInputSceneName;
    public GameObject[] lobbyPlayerServer = new GameObject[5];
    public GameObject[] SpawnPlayerPosition = new GameObject[5];

    [HideInInspector]
    public LobbyPlayerLogic.TeamName playerTeamName;
    [HideInInspector]
    public string playerPseudo;
    private int nbRedTeam = 0;
    private int nbBlueTeam = 0;

    [SerializeField]
    private string gameScene;

    [HideInInspector]
    public string analyticsPath;

    [HideInInspector]
    public SoundManager soundManager;
    

    #region Unity Callbacks

    public override void OnValidate()
    {
        base.OnValidate();
    }

    /// <summary>
    /// Runs on both Server and Client
    /// Networking is NOT initialized when this fires
    /// </summary>
    public override void Awake()
    {
        base.Awake();

    }

    /// <summary>
    /// Runs on both Server and Client
    /// Networking is NOT initialized when this fires
    /// </summary>
    public override void Start()
    {
        base.Start();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        //Stop music when player is back to Menu
        soundManager.StopMusic();
    }

    /// <summary>
    /// Runs on both Server and Client
    /// </summary>
    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    /// <summary>
    /// Runs on both Server and Client
    /// </summary>
    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    #endregion

    #region Start & Stop


    /// <summary>
    /// called when quitting the application by closing the window / pressing stop in the editor
    /// </summary>
    public override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
    }

    #endregion

    #region Scene Management

    /// <summary>
    /// This causes the server to switch scenes and sets the networkSceneName.
    /// <para>Clients that connect to this server will automatically switch to this scene. This is called automatically if onlineScene or offlineScene are set, but it can be called from user code to switch scenes again while the game is in progress. This automatically sets clients to be not-ready. The clients must call NetworkClient.Ready() again to participate in the new scene.</para>
    /// </summary>
    /// <param name="newSceneName"></param>
    public override void ServerChangeScene(string newSceneName)
    {
        base.ServerChangeScene(newSceneName);
    }

    /// <summary>
    /// Called from ServerChangeScene immediately before SceneManager.LoadSceneAsync is executed
    /// <para>This allows server to do work / cleanup / prep before the scene changes.</para>
    /// </summary>
    /// <param name="newSceneName">Name of the scene that's about to be loaded</param>
    public override void OnServerChangeScene(string newSceneName) { }

    /// <summary>
    /// Called on the server when a scene is completed loaded, when the scene load was initiated by the server with ServerChangeScene().
    /// </summary>
    /// <param name="sceneName">The name of the new scene.</param>
    public override void OnServerSceneChanged(string sceneName) {
        
    }

    /// <summary>
    /// Called from ClientChangeScene immediately before SceneManager.LoadSceneAsync is executed
    /// <para>This allows client to do work / cleanup / prep before the scene changes.</para>
    /// </summary>
    /// <param name="newSceneName">Name of the scene that's about to be loaded</param>
    /// <param name="sceneOperation">Scene operation that's about to happen</param>
    /// <param name="customHandling">true to indicate that scene loading will be handled through overrides</param>
    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling) { }

    /// <summary>
    /// Called on clients when a scene has completed loaded, when the scene load was initiated by the server.
    /// <para>Scene changes can cause player objects to be destroyed. The default implementation of OnClientSceneChanged in the NetworkManager is to add a player object for the connection if no player object exists.</para>
    /// </summary>
    /// <param name="conn">The network connection that the scene change message arrived on.</param>
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
        if (SceneManager.GetActiveScene().name != "LobbyScene")
        {
            soundManager.StopMusic();
            soundManager.PlayMusic("GameMusic");

            //Reset number
            nbBlueTeam = 0;
            nbRedTeam = 0;

            MyNewNetworkAuthenticator.CreateClientPlayer msg = new MyNewNetworkAuthenticator.CreateClientPlayer
            {
                teamName = playerTeamName,
                pseudo = playerPseudo
            };
            
            conn.Send(msg);
        }
        else
        {
            soundManager.StopMusic();
        }
       
    }

    #endregion

    #region Server System Callbacks

    /// <summary>
    /// Called on the server when a new client connects.
    /// <para>Unity calls this on the Server when a Client connects to the Server. Use an override to tell the NetworkManager what to do when a client connects to the server.</para>
    /// </summary>
    /// <param name="conn">Connection from client.</param>
    public override void OnServerConnect(NetworkConnection conn) {
        
    }

    /// <summary>
    /// Called on the server when a client is ready.
    /// <para>The default implementation of this function calls NetworkServer.SetClientReady() to continue the network setup process.</para>
    /// </summary>
    /// <param name="conn">Connection from client.</param>
    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);
    }

    /// <summary>
    /// Called on the server when a client adds a new player with ClientScene.AddPlayer.
    /// <para>The default implementation for this function creates a new player object from the playerPrefab.</para>
    /// </summary>
    /// <param name="conn">Connection from client.</param>
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
    }

    /// <summary>
    /// Called on the server when a client disconnects.
    /// <para>This is called on the Server when a Client disconnects from the Server. Use an override to decide what should happen when a disconnection is detected.</para>
    /// </summary>
    /// <param name="conn">Connection from client.</param>
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
    }

    /// <summary>
    /// Called on server when transport raises an exception.
    /// <para>NetworkConnection may be null.</para>
    /// </summary>
    /// <param name="conn">Connection of the client...may be null</param>
    /// <param name="exception">Exception thrown from the Transport.</param>
    public override void OnServerError(NetworkConnection conn, Exception exception) { }

    #endregion

    #region Client System Callbacks

    /// <summary>
    /// Called on the client when connected to a server.
    /// <para>The default implementation of this function sets the client as ready and adds a player. Override the function to dictate what happens when the client connects.</para>
    /// </summary>
    /// <param name="conn">Connection to the server.</param>
    public override void OnClientConnect(NetworkConnection conn)//Quand le client se connecte envoit un message contenant le pseudo
    {
        base.OnClientConnect(conn);
        Debug.Log(GetComponent<MyNewNetworkAuthenticator>().lobbyPseudo);

        //Keep player pseudo
        playerPseudo = GetComponent<MyNewNetworkAuthenticator>().lobbyPseudo;

        //Send message to host with client pseudo
        MyNewNetworkAuthenticator.ClientConnectionMessage clientMsg = new MyNewNetworkAuthenticator.ClientConnectionMessage 
        {
            pseudo = GetComponent<MyNewNetworkAuthenticator>().lobbyPseudo
        };
        NetworkClient.Send(clientMsg);
    }

    /// <summary>
    /// Called on clients when disconnected from a server.
    /// <para>This is called on the client when it disconnects from the server. Override this function to decide what happens when the client disconnects.</para>
    /// </summary>
    /// <param name="conn">Connection to the server.</param>
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        Shutdown();
        if (SceneManager.GetActiveScene().name != "LobbyScene")
        {
            Shutdown();
            Destroy(gameObject);
            SceneManager.LoadScene("LobbyScene");
        }
        
    }

    /// <summary>
    /// Called on clients when a servers tells the client it is no longer ready.
    /// <para>This is commonly used when switching scenes.</para>
    /// </summary>
    /// <param name="conn">Connection to the server.</param>
    public override void OnClientNotReady(NetworkConnection conn) { }

    /// <summary>
    /// Called on client when transport raises an exception.</summary>
    /// </summary>
    /// <param name="exception">Exception thrown from the Transport.</param>
    public override void OnClientError(Exception exception) {
        Debug.Log(exception.Message);
    }

    #endregion

    #region Start & Stop Callbacks

    // Since there are multiple versions of StartServer, StartClient and StartHost, to reliably customize
    // their functionality, users would need override all the versions. Instead these callbacks are invoked
    // from all versions, so users only need to implement this one case.

    /// <summary>
    /// This is invoked when a host is started.
    /// <para>StartHost has multiple signatures, but they all cause this hook to be called.</para>
    /// </summary>
    public override void OnStartHost() {
        NetworkServer.RegisterHandler<MyNewNetworkAuthenticator.ClientConnectionMessage>(CreateClientFromServer, true);
        NetworkServer.RegisterHandler<MyNewNetworkAuthenticator.CreateClientPlayer>(CreatePlayer, true);
    }

    /// <summary>
    /// This is invoked when a server is started - including when a host is started.
    /// <para>StartServer has multiple signatures, but they all cause this hook to be called.</para>
    /// </summary>
    public override void OnStartServer() { }

    /// <summary>
    /// This is invoked when the client is started.
    /// </summary>
    public override void OnStartClient() {
        StartButton.SetActive(false);

    }

    /// <summary>
    /// This is called when a host is stopped.
    /// </summary>
    public override void OnStopHost() {
        
    }

    /// <summary>
    /// This is called when a server is stopped - including when a host is stopped.
    /// </summary>
    public override void OnStopServer() {
        ClearArray(lobbyPlayerServer);
    }

    /// <summary>
    /// This is called when a client is stopped.
    /// </summary>
    public override void OnStopClient() {
        if (SceneManager.GetActiveScene().name == "LobbyScene" && MenuManagerObject.GetComponent<MenuManager>().lobbyObject.activeSelf)
        {
            MenuManagerObject.GetComponent<MenuManager>().ChangeMenu();
        }
    }

    #endregion


    #region Mes fonctions qui g�rent le network
    private void CreateClientFromServer(NetworkConnection conn, MyNewNetworkAuthenticator.ClientConnectionMessage msg)
    {
        if (conn.clientOwnedObjects.Count < 1) // D�bug quand le joueur se connecte � un server qui n'existe pas, et ensuite host
        {
            GameObject obj = Instantiate(lobbyPlayer);
            obj.GetComponent<LobbyPlayerLogic>().clientPseudo = msg.pseudo;
            obj.transform.position = new Vector3(0, 0, 0);
            NetworkServer.AddPlayerForConnection(conn, obj);
            AddToServerArray(obj);
        }
        
    } //Spawn l'objet lobbyPlayer et configure le server

    public void CreatePlayer(NetworkConnection conn, MyNewNetworkAuthenticator.CreateClientPlayer msg)
    {
        StartCoroutine(CreateDelayPlayer(conn, msg));
    }

    public IEnumerator CreateDelayPlayer(NetworkConnection conn, MyNewNetworkAuthenticator.CreateClientPlayer msg)
    {
        //yield return new WaitForSeconds(0.1f);

        //Create player in game
        if(msg.teamName != LobbyPlayerLogic.TeamName.Spectator)
        {
            GameObject obj = Instantiate(Player);

            //Set Pseudo for the client
            obj.GetComponent<PlayerLogic>().pseudoPlayer = msg.pseudo;

            //Set the team for the client
            obj.GetComponent<PlayerLogic>().teamName = msg.teamName;

            if (msg.teamName == LobbyPlayerLogic.TeamName.Blue)
            {
                obj.GetComponent<PlayerLogic>().spawnPosition = nbBlueTeam;
                nbBlueTeam++;
            }

            if (msg.teamName == LobbyPlayerLogic.TeamName.Red)
            {
                obj.GetComponent<PlayerLogic>().spawnPosition = nbRedTeam + 2;
                nbRedTeam++;
            }

            NetworkServer.AddPlayerForConnection(conn, obj);
        }
        else //Create Spectator
        {
            GameObject obj = Instantiate(spectator);
            NetworkServer.AddPlayerForConnection(conn, obj);
        }
        
        yield return null;
    }

    public void SetGameScene(string _sceneName)
	{
        gameScene = _sceneName;
	}

    public void StartGame()
    {
        ServerChangeScene(gameScene);
    }

    #endregion

    #region Gestion du tableau contenant les objets des clients cot� server
    private void AddToServerArray(GameObject obj)
    {
        for(int i =0; i< lobbyPlayerServer.Length; i++)
        {
            if(lobbyPlayerServer[i] == null)
            {
                lobbyPlayerServer[i] = obj;
                obj.GetComponent<LobbyPlayerLogic>().RpcChangePosition(i);
                break;
            }
        }
    }

    private void ClearArray(GameObject[] array)
    {
        for(int i =0; i < array.Length; i++)
        {
            array[i] = null;
        }
    }

    public bool CheckIsReady()
    {
        for (int i = 0; i < lobbyPlayerServer.Length; i++)
        {
            if (lobbyPlayerServer[i] != null)
            {
                if (!lobbyPlayerServer[i].GetComponent<LobbyPlayerLogic>().isReady)
                {
                    StartButton.SetActive(false);
                    return false;
                }
            }
        }
        StartButton.SetActive(true);
        return true;
    }
    #endregion


}
