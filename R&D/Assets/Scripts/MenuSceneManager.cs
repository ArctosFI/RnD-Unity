using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class MenuSceneManager : NetworkBehaviour
{
    [SerializeField] GameObject xrRig;

    private void Start()
    {
        NetworkManager.OnServerStarted += LoadServer;
    }

    void LoadServer()
    {
        NetworkManager.SceneManager.LoadScene("Server", LoadSceneMode.Single);
    }
}
