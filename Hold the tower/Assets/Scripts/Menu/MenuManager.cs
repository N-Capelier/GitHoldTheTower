using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject menuObject, lobbyObject;

    private GameObject serverManager;
    private bool isHost = false;

    void Start()
    {
        menuObject.SetActive(true);
        lobbyObject.SetActive(false);
        serverManager = GameObject.Find("ServerManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Button
    public void onPressedHost()
    {
        changeMenu();
        isHost = true;
        serverManager.GetComponent<MyNewNetworkManager>().StartHost();
    }

    public void onPressedJoin()
    {
        changeMenu();
        serverManager.GetComponent<MyNewNetworkManager>().StartClient();
    }

    public void onPressedLeave()
    {
        changeMenu();
        if (isHost)
        {
            serverManager.GetComponent<MyNewNetworkManager>().StopHost();
            isHost = false;
        }
        else
        {
            serverManager.GetComponent<MyNewNetworkManager>().StopClient();
        }
    }

    #endregion

    private void changeMenu()
    {
        menuObject.SetActive(!menuObject.activeSelf);
        lobbyObject.SetActive(!lobbyObject.activeSelf);
    }
}
