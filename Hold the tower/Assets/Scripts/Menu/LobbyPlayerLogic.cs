using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class LobbyPlayerLogic : NetworkBehaviour
{
    [Header("SyncVar")]
    [SyncVar(hook =nameof(setReadyUI))]
    public bool isReady = false;
    [SyncVar(hook = nameof(updatePseudo))]
    public string clientPseudo;
    [SyncVar(hook = nameof(ChangeTeam))]
    public int team;


    public enum TeamName
    {
        Red,Blue
    };

    public TeamName teamName;

    [Header("Var")]
    [SerializeField]
    private TextMeshProUGUI usernameText;
    [SerializeField]
    private GameObject readyUi;
    [SerializeField]
    private GameObject readyButton;
    [SerializeField]
    private Image teamImage;
    
    [SerializeField]
    private GameObject[] lobbyPositions;

    private GameObject serverManager;

    public void Start()
    {
        serverManager = GameObject.Find("ServerManager"); //G�re l'ui des lobbys personnages
        readyUi.SetActive(false);
        readyButton.SetActive(false);
        transform.Find("Left").gameObject.SetActive(false);
        transform.Find("Right").gameObject.SetActive(false);

        ChangeTeam(0, team); //Met a tous les joueurs qui se connectent

        setReadyUI(false, false); //sync UI on every client
        if (isLocalPlayer)
        {
            transform.localScale = new Vector3(2, 2, 2);
            readyButton.SetActive(true);
            transform.Find("Left").gameObject.SetActive(true);
            transform.Find("Right").gameObject.SetActive(true);
        }

        if (isServer)
        {
            serverManager.GetComponent<MyNewNetworkManager>().CheckIsReady(); //Quand un client se connecte mettre a jour le bouton lancer
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
        usernameText.text = newValue;
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
        serverManager.GetComponent<MyNewNetworkManager>().CheckIsReady();
    }

    public void setReadyUI(bool oldValue,bool newValue)
    {
        readyUi.SetActive(isReady);
    } //Syncronise l'ui 

    public void ChangeTeam(int oldValue,int newValue)
    {
        switch (newValue)
        {
            case 0:
                teamImage.color = Color.red;
                teamName = TeamName.Red;
                break;
            case 1:
                teamImage.color = Color.blue;
                teamName = TeamName.Blue;
                break;
        }
        if (hasAuthority)
        {
            serverManager.GetComponent<MyNewNetworkManager>().playerTeamName = teamName;
        }
        
    } //Syncronise l'ui
    #endregion
}
