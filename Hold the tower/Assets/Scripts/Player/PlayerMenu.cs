using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMenu : MonoBehaviour
{
    private KeyCode menuKey = KeyCode.Tab;

    [SerializeField]
    private PlayerLogic selfLogic;
    [SerializeField]
    private ScriptableParamsPlayer selfParams;

    [SerializeField]
    private GameObject menuHud;
    [SerializeField]
    private GameObject menuMainPlayer;
    [SerializeField]
    private GameObject menuSettingsPlayer;
    [SerializeField]
    private GameObject menuInput;
    [SerializeField]
    private GameObject blackLoadScreen;
    [SerializeField]
    private GameObject menuBase;
    [SerializeField]
    private GameObject menuSound;

    [HideInInspector]
    public bool menuIsOpen = false;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(menuKey) && selfLogic.hasAuthority)
        {
            menuIsOpen = !menuIsOpen;
            blackLoadScreen.SetActive(false);
            if (menuIsOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                menuHud.SetActive(true);
                menuMainPlayer.SetActive(true);
                menuBase.SetActive(true);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                menuHud.SetActive(false);
                menuMainPlayer.SetActive(false);
                menuSettingsPlayer.SetActive(false);
                menuInput.SetActive(false);
                menuBase.SetActive(false);
                menuSound.SetActive(false);
            }
        }
    }

    public void CloseMenu()
    {
        menuIsOpen = false;
        menuHud.SetActive(false);
        menuMainPlayer.SetActive(false);
        menuSettingsPlayer.SetActive(false);
        menuInput.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
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
        
        StartCoroutine(BackToMenuManager());
    }

    private IEnumerator BackToMenuManager()
    {
        MyNewNetworkManager.Shutdown();
        Destroy(GameObject.Find("ServerManager"));
 
        while (GameObject.Find("ServerManager") != null)
        {
            Debug.Log("hein");
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(2f);
        SceneManager.LoadSceneAsync("LobbyScene");

        yield return new WaitForEndOfFrame();
    }

    public void OpenSettings()
    {
        menuMainPlayer.SetActive(false);
        menuSettingsPlayer.SetActive(true);
        menuInput.SetActive(false);
        menuSound.SetActive(false);
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

    public void OpenSoundSettings()
    {
        menuSound.SetActive(true);
        menuSettingsPlayer.SetActive(false);
    }

    public void OpenGraphicsSettings()
    {

    }

}
