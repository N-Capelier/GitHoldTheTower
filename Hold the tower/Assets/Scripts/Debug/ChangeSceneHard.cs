using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class ChangeSceneHard : MonoBehaviour
{

    void Update()
    {
        if(NetworkManager.singleton == null)
        {
            SceneManager.LoadScene("LobbyScene");
            return;
        }
    }
}
