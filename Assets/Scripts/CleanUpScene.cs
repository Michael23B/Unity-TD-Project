using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CleanUpScene : MonoBehaviour {

    private void Start()
    {
        SceneManager.activeSceneChanged += DestroyOnMenuScreen;
    }
    /*
    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= DestroyOnMenuScreen;
    }
    */
    void DestroyOnMenuScreen(Scene oldScene, Scene newScene)
    {
        if (newScene.buildIndex != oldScene.buildIndex) //0 == "Main Menu" (could compare scene name instead)
        {
            //MainCamera[] maincams = FindObjectsOfType<MainCamera>();
            //foreach (MainCamera m in maincams) Destroy(m);

            NetworkManager net = FindObjectOfType<NetworkManager>();
            NetworkManagerHUD nethud = FindObjectOfType<NetworkManagerHUD>();
            nethud.gameObject.SetActive(false);
            net.StopHost();
            Network.Disconnect();
            NetworkServer.Reset();
        }
    }
}
