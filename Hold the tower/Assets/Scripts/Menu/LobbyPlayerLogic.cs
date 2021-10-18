using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class LobbyPlayerLogic : NetworkBehaviour
{
    [Header("SyncVar")]
    [SyncVar(hook =nameof(setReadyUI))]
    public bool isReady = false;
    [SyncVar(hook = nameof(updatePseudo))]
    public string clientPseudo;
    [SyncVar(hook = nameof(ChangeTeam))]
    public int team;
    [SyncVar]
    public string nameOfTeam;

    [Header("Var")]
    [SerializeField]
    private GameObject text;
    [SerializeField]
    private GameObject ReadyUi;
    [SerializeField]
    private GameObject ReadyButton;
    [SerializeField]
    private Image Team;
    
    [SerializeField]
    private GameObject[] lobbyPositions;


    private GameObject ServerManager;

    public void Start()
    {
        ServerManager = GameObject.Find("ServerManager"); //Gère l'ui des lobbys personnages
        ReadyUi.SetActive(false);
        ReadyButton.SetActive(false);
        transform.Find("Left").gameObject.SetActive(false);
        transform.Find("Right").gameObject.SetActive(false);

        ChangeTeam(0, team); //Met a tous les joueurs qui se connectent

        setReadyUI(false, false); //sync UI on every client
        if (isLocalPlayer)
        {
            ReadyButton.SetActive(true);
            transform.Find("Left").gameObject.SetActive(true);
            transform.Find("Right").gameObject.SetActive(true);
        }

        if (isServer)
        {
            ServerManager.GetComponent<MyNewNetworkManager>().CheckIsReady(); //Quand un client se connecte mettre a jour le bouton lancer
        }

    }

    #region Mirror Callback
    public override void OnStartClient()
    {
        base.OnStartClient();
        transform.SetParent(GameObject.Find("Canvas/Lobby").transform);
        lobbyPositions = GameObject.FindGameObjectsWithTag("LobbyPosition");
    }

    #endregion

    #region Button
    [Command]
    public void ButtonLeft()
    {
        team--;
        if(team < 0)
        {
            team = 1;
        }
    }
    [Command]
    public void ButtonRight()
    {
        team++;
        if (team > 1)
        {
            team = 0;
        }
    }
    #endregion

    #region Syncro Logic
    public void updatePseudo(string oldValue, string newValue)
    {
        text.GetComponent<Text>().text = newValue;
        gameObject.name = newValue;
    }

    [ClientRpc]
    public void RpcChangePosition(int newPos)
    {
        transform.localPosition = lobbyPositions[newPos].transform.localPosition;
    }// Transmets aux client la bonne position grace aux objets lobbyPositions

    [Command]
    public void CmdSetReady()
    {
        isReady = !isReady;
        ServerManager.GetComponent<MyNewNetworkManager>().CheckIsReady();
    }

    public void setReadyUI(bool oldValue,bool newValue)
    {
        ReadyUi.SetActive(isReady);
    } //Syncronise l'ui 

    public void ChangeTeam(int oldValue,int newValue)
    {
        switch (newValue)
        {
            case 0:
                Team.color = Color.red;
                nameOfTeam = "red";
                break;
            case 1:
                Team.color = Color.blue;
                nameOfTeam = "blue";
                break;
        }

        ServerManager.GetComponent<MyNewNetworkManager>().playerTeamName = nameOfTeam;
    } //Syncronise l'ui
    #endregion
}
