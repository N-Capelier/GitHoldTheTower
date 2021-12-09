using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

/*
    Documentation: https://mirror-networking.gitbook.io/docs/components/network-authenticators
    API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkAuthenticator.html
*/

public class MyNewNetworkAuthenticator : NetworkAuthenticator
{
    public string lobbyPseudo;
    public string lobbyPassword;

    [SerializeField]
    private GameObject PseudoTextInput;

    #region Messages

    public struct AuthRequestMessage : NetworkMessage {
        public string pseudo;
        public string password;
    }

    public struct AuthResponseMessage : NetworkMessage {
        public byte status;
        public string message;
    }

    public struct ClientConnectionMessage : NetworkMessage //Message permettant de créer le joueur
    {
        public string pseudo;
    }

    public struct CreateClientPlayer : NetworkMessage
    {
        public LobbyPlayerLogic.nameOfTeam teamName;
    }
    #endregion

    #region Server

    /// <summary>
    /// Called on server from StartServer to initialize the Authenticator
    /// <para>Server message handlers should be registered in this method.</para>
    /// </summary>
    public override void OnStartServer()
    {
        // register a handler for the authentication request we expect from client
        NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
    }

    /// <summary>
    /// Called on server from OnServerAuthenticateInternal when a client needs to authenticate
    /// </summary>
    /// <param name="conn">Connection to client.</param>
    public override void OnServerAuthenticate(NetworkConnection conn) { }

    /// <summary>
    /// Called on server when the client's AuthRequestMessage arrives
    /// </summary>
    /// <param name="conn">Connection to client.</param>
    /// <param name="msg">The message payload</param>
    public void OnAuthRequestMessage(NetworkConnection conn, AuthRequestMessage msg)
    {
        AuthResponseMessage authResponseMessage = new AuthResponseMessage();
        if (msg.password == lobbyPassword)
        {
            authResponseMessage.status = 100;
            authResponseMessage.message = "Succes";
            conn.Send(authResponseMessage);
            ServerAccept(conn);
        }
        else
        {
            authResponseMessage.status = 200;
            authResponseMessage.message = "Error";
            conn.Send(authResponseMessage);
            ServerReject(conn);
        }

        // Accept the successful authentication
        
    }

    #endregion

    #region Client

    /// <summary>
    /// Called on client from StartClient to initialize the Authenticator
    /// <para>Client message handlers should be registered in this method.</para>
    /// </summary>
    public override void OnStartClient()
    {
        // register a handler for the authentication response we expect from server
        NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage, false);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        NetworkClient.UnregisterHandler<AuthResponseMessage>();
    }

    /// <summary>
    /// Called on client from OnClientAuthenticateInternal when a client needs to authenticate
    /// </summary>
    public override void OnClientAuthenticate()
    {
        AuthRequestMessage authRequestMessage = new AuthRequestMessage();
        authRequestMessage.pseudo = lobbyPseudo;
        authRequestMessage.password = lobbyPassword;
        NetworkClient.Send(authRequestMessage);
    }

    /// <summary>
    /// Called on client when the server's AuthResponseMessage arrives
    /// </summary>
    /// <param name="msg">The message payload</param>
    public void OnAuthResponseMessage(AuthResponseMessage msg)
    {
        if (msg.status == 100)
            ClientAccept();
        else
            ClientReject();
    }

    #endregion

    //public void ChangeNetworkAdress(string n)
    //{
    //    networkAddress = TextInputIp.GetComponent<Text>().text;
    //}

    public void inputPseudo(string n) //met à jour le pseudo en fonction de l'input
    {
        lobbyPseudo = PseudoTextInput.GetComponent<Text>().text;
    }
}
