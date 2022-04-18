using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkToLobby : MonoBehaviour
{

    void Update()
    {
        if (MyNewNetworkManager.singleton == null)
        {
            SceneManager.LoadScene(0);
        }
    }
}
