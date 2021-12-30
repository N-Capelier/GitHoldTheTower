using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMenu : MonoBehaviour
{
    private KeyCode menuKey = KeyCode.Tab;

    [SerializeField]
    private PlayerLogic selfLogic;
    [SerializeField]
    public GameObject menuHud;
    [HideInInspector]
    public bool menuIsOpen = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(menuKey))
        {
            menuIsOpen = !menuIsOpen;
            if (menuIsOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                menuHud.SetActive(true);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                menuHud.SetActive(false);
            }
        }
        MenuHud();
    }

    private void MenuHud()
    {
        if (menuIsOpen)
        {

        }
        else
        {

        }
    }

    public void BackToMenu()
    {
        if (selfLogic.isClientOnly)
        {
            MyNewNetworkManager.singleton.StopClient();
        }
        else
        {
            MyNewNetworkManager.singleton.StopHost();
        }
        Destroy(GameObject.Find("ServerManager"));
        SceneManager.LoadScene("LobbyScene");
    }
}
