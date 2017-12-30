using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    Behaviour[] componentsToDisable;

    Camera sceneCamera;

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

    private void OnDisable()
    {
        if(sceneCamera != null) sceneCamera.gameObject.SetActive(true);
    }
}
