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
    [SyncVar(hook = nameof(UpdateUsername))]
    public string clientPseudo;
    [SyncVar(hook = nameof(ChangeTeam))]
    public int team;

    public Color omegaColor;
    public Color psyColor;
    public Color spectatorColor;

    public enum TeamName
    {
        Red,Blue,Spectator
    };

    public TeamName teamName;

    [Header("Var")]
    [SerializeField]
    private Text usernameText;
    [SerializeField]
    private GameObject readyUi;
    [SerializeField]
    private GameObject readyButton;
    [SerializeField]
    private Image teamImage;

    [SerializeField]
    private Sprite omgegaImage;
    [SerializeField]
    private Sprite psiImage;
    [SerializeField]
    private Sprite spectatorImage;

    [SerializeField]
    private GameObject[] lobbyPositions;

    private GameObject serverManager;

    public void Start()
    {
        serverManager = GameObject.Find("ServerManager"); //Gère l'ui des lobbys personnages
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
        if(team - 1 < 0)
        {
            team = 2;
        }
        else
		{
            team--;
		}
    }
    [Command]
    public void ButtonRight()
    {
        if (team + 1 > 2)
        {
            team = 0;
        }
        else
		{
            team++;
		}
    }
    #endregion

    #region Syncro Logic
    public void UpdateUsername(string oldValue, string newValue)
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
                teamImage.sprite = omgegaImage;
                teamImage.color = omegaColor;
                teamName = TeamName.Red;
                break;
            case 1:
                teamImage.sprite = psiImage;
                teamImage.color = psyColor;
                teamName = TeamName.Blue;
                break;
            case 2:
                teamImage.sprite = spectatorImage;
                teamImage.color = spectatorColor;
                teamName = TeamName.Spectator;
                break;
        }
        if (hasAuthority)
        {
            serverManager.GetComponent<MyNewNetworkManager>().playerTeamName = teamName;
        }
        
    } //Syncronise l'ui
    #endregion
}
