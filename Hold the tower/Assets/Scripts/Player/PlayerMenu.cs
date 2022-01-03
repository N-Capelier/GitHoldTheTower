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
    private GameObject menuHud;
    [SerializeField]
    private GameObject menuMainPlayer;
    [SerializeField]
    private GameObject menuSettingsPlayer;
    [SerializeField]
    private GameObject menuInput;

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
                menuMainPlayer.SetActive(true);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                menuHud.SetActive(false);
                menuMainPlayer.SetActive(false);
                menuSettingsPlayer.SetActive(false);
                menuInput.SetActive(false);
            }
        }
    }

    public void CloseMenu()
    {
        menuHud.SetActive(false);
        menuMainPlayer.SetActive(false);
        menuSettingsPlayer.SetActive(false);
        menuInput.SetActive(false);
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

    public void OpenSettings()
    {
        menuMainPlayer.SetActive(false);
        menuSettingsPlayer.SetActive(true);
        menuInput.SetActive(false);
    }

    public void OpenMainMenu()
    {
        menuMainPlayer.SetActive(true);
        menuSettingsPlayer.SetActive(false);
    }

    public void OpenInputSettings()
    {
        menuSettingsPlayer.SetActive(false);
        menuInput.SetActive(true);
    }


}
