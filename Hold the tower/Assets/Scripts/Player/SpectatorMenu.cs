using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;


public class SpectatorMenu : NetworkBehaviour
{
    [SerializeField]
    private KeyCode menuKey = KeyCode.Escape;

    #region références
    [Header("Références")]
    [SerializeField] private GameObject canvasMenuObject;

    private MatchManager match; //To get score

    [SerializeField] private GameObject fullScore;
    [SerializeField] private Text redScore;
    [SerializeField] private Text blueScore;

    //All the object that contains Button component attach
    //bind to each players on lateStart
    [SerializeField] private GameObject[] buttonPlayerFollow;


    //All menu game object to hide/show
    [SerializeField] private GameObject MenuSettings;
    [SerializeField] private GameObject MenuInput;
    [SerializeField] private GameObject MenuGraphics;
    [SerializeField] private GameObject MenuSounds;
    [SerializeField] private GameObject MenuMain;
    #endregion

    #region variables internes
    private GameObject[] allPlayers; //Get all players connected in this game
    private bool lateStart = true; // just a do once, when all players are connected

    [HideInInspector] public bool menuActive = false;

    [HideInInspector]
    public bool nearFocusX = false;
    [HideInInspector]
    public bool nearFocusY = false;
    [HideInInspector]
    public bool nearFocusZ = false;


    //Gameobject the spectator will focus
    [HideInInspector] public GameObject playerToFocus = null;
    //Bool that check if spectator focus a player
    [HideInInspector] public bool spectatorIsFocus = false;

    
    #endregion

    private void Start()
    {
        canvasMenuObject.SetActive(false);
        if (hasAuthority)
        {
            match = GameObject.Find("GameManager").GetComponent<MatchManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (hasAuthority)
        {
            if (Input.GetKeyDown(menuKey))
            {
                OpenCloseMenu();
            }
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            if (fullScore.activeSelf)
            {
                fullScore.SetActive(false);
            }
            else
            {
                fullScore.SetActive(true);
            }
        }

        if (hasAuthority && AllClientAreReady() && lateStart)
        {
            lateStart = false;

            for (int j = 0; j < buttonPlayerFollow.Length; j++)
            {
                buttonPlayerFollow[j].SetActive(false);
            }

            //a start when all player are connected
            if (isClientOnly)
            {
                CmdGetAllPlayers();
            }
            else
            {
                allPlayers = GetAllPlayers();
            }

            for (int i = 0; i < allPlayers.Length; i++)
            {
                Debug.Log("Boutton : " + buttonPlayerFollow[i] + "Joueur : " + allPlayers[i]);
                if (buttonPlayerFollow[i] != null && allPlayers[i] != null)
                {
                    buttonPlayerFollow[i].SetActive(true);
                    Button b = buttonPlayerFollow[i].GetComponent<Button>();
                    GameObject playerObj = allPlayers[i];
                    b.GetComponentInChildren<Text>().text = playerObj.GetComponent<PlayerLogic>().pseudoPlayer;
                    b.onClick.AddListener(() => {
                        OnPressedFocus(playerObj);
                    });

                }
            }

        }
        if(hasAuthority)
        {
            redScore.text = match.redScore.ToString();
            blueScore.text = match.blueScore.ToString();
        }

    }

    public void OnPressedFocus(GameObject objToFocus)
    {
        playerToFocus = objToFocus;
        spectatorIsFocus = true;
        nearFocusX = false;
        nearFocusY = false;
        nearFocusZ = false;
    }

    private void OpenCloseMenu()
    {
        canvasMenuObject.SetActive(!canvasMenuObject.activeSelf);
        menuActive = canvasMenuObject.activeSelf;
        if (canvasMenuObject.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void SpectatorExitGame()
    {
        SoundManager.Instance.StopMusic();
        DestroyImmediate(GameObject.Find("SoundManager"));

        if (isServer)
        {
            MyNewNetworkManager.singleton.StopHost();
        }
        else
        {
            MyNewNetworkManager.singleton.StopClient();
        }
    }

    [Command]
    public void CmdGetAllPlayers()
    {
        RpcGetAllPlayers();
    }

    [ClientRpc]
    public void RpcGetAllPlayers()
    {
        allPlayers = GetAllPlayers();
    }

    [ServerCallback]
    public GameObject[] GetAllPlayers()
    {
        // -1 pour le spectateur
        GameObject[] _allplayers = new GameObject[NetworkServer.connections.Count-1];
        int i = 0;
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            foreach (NetworkIdentity idOwnedByClient in conn.clientOwnedObjects)
            {
                if (idOwnedByClient.gameObject.GetComponent<PlayerLogic>() != null)
                {
                    Debug.Log("Player" + i);
                    _allplayers[i] = idOwnedByClient.gameObject;
                    i++;
                }
            }
        }
        
        return _allplayers;
    }

    [ServerCallback]
    private bool AllClientAreReady()
    {
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            if (!conn.isReady)
            {
                return false;
            }
        }
        return true;
    }

    public void OpenSettings()
    {
        MenuSettings.SetActive(true);
        MenuGraphics.SetActive(false);
        MenuInput.SetActive(false);
        MenuSounds.SetActive(false);
        MenuMain.SetActive(false);
    }

    public void OpenSounds()
    {
        MenuSettings.SetActive(false);
        MenuGraphics.SetActive(false);
        MenuInput.SetActive(false);
        MenuSounds.SetActive(true);
        MenuMain.SetActive(false);
    }

    public void OpenInput()
    {
        MenuSettings.SetActive(false);
        MenuGraphics.SetActive(false);
        MenuInput.SetActive(true);
        MenuSounds.SetActive(false);
        MenuMain.SetActive(false);
    }

    public void OpenGraphics()
    {
        MenuSettings.SetActive(false);
        MenuGraphics.SetActive(true);
        MenuInput.SetActive(false);
        MenuSounds.SetActive(false);
        MenuMain.SetActive(false);
    }

    public void OpenMain()
    {
        MenuSettings.SetActive(false);
        MenuGraphics.SetActive(false);
        MenuInput.SetActive(false);
        MenuSounds.SetActive(false);
        MenuMain.SetActive(true);
    }
}
