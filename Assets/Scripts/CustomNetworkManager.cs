using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class CustomNetworkManager : NetworkManager
{
    public void StartupHost()
    {
        SetPort();
        StartHost();
        //Debug.Log(NetworkServer.listenPort);
        //Debug.Log(client.serverPort);
        //Debug.Log(NetworkManager.singleton.networkAddress);
    }

    public void JoinGame()
    {
        SetIPAddress();
        SetPort();
        StartClient();
    }

    public void SetIPAddress()
    {
        string ipAddress = GameObject.Find("InputFieldIPAddress").transform.Find("Text").GetComponent<Text>().text;
        networkAddress = ipAddress;
    }

    void SetPort()
    {
        networkPort = 6666;
    }

    private void Start()
    {
        StartCoroutine(SetupMenuSceneButtons());
        //SetupOtherSceneButtons();
    }
    /*
    void OnLevelWasLoaded(int level)
    {
        if (level == 0)
        {
            SetupMenuSceneButtons();
            StartCoroutine(SetupMenuSceneButtons());
        }
        else
        {
            SetupOtherSceneButtons();
        }
    }
    */
    IEnumerator SetupMenuSceneButtons()
    {
        yield return new WaitForSeconds(0.15f);
        GameObject.Find("HostButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("HostButton").GetComponent<Button>().onClick.AddListener(StartupHost);

        GameObject.Find("JoinButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("JoinButton").GetComponent<Button>().onClick.AddListener(JoinGame);
    }

    void SetupOtherSceneButtons()
    {
        GameObject.Find("DisconnectButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("DisconnectButton").GetComponent<Button>().onClick.AddListener(NetworkManager.singleton.StopHost);
    }
}