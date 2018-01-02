using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    Behaviour[] componentsToDisable;

    Camera sceneCamera;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    private void Start()
    {
        if (!isLocalPlayer)
        {
            for (int i = 0; i < componentsToDisable.Length; ++i)
            {
                componentsToDisable[i].enabled = false;
            }
            gameObject.tag = "Player2";
        }
        else
        {
            sceneCamera = Camera.main;
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }

            if (connectionToServer.connectionId != 0 && connectionToServer.connectionId != -1)
            {
                WaveSpawner.Instance.AlternateSpawner();
            }
        }
        WaveSpawner.Instance.commands.CmdPlayerColorsUpdate();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!isLocalPlayer)
        {
            for (int i = 0; i < componentsToDisable.Length; ++i)
            {
                componentsToDisable[i].enabled = false;
            }
            gameObject.tag = "Player2";
        }
        else
        {
            sceneCamera = Camera.main;
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }

            if (connectionToServer.connectionId != 0 && connectionToServer.connectionId != -1)
            {
                WaveSpawner.Instance.AlternateSpawner();
            }
        }
        WaveSpawner.Instance.commands.CmdPlayerColorsUpdate();
    }

    private void OnDisable()
    {
        if(sceneCamera != null) sceneCamera.gameObject.SetActive(true);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
