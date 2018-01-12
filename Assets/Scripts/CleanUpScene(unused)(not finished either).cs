using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CleanUpScene : MonoBehaviour {

    static CleanUpScene Instance;

    NetworkManager net;
    NetworkManagerHUD nethud;
    MainCamera mainCam;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        SceneManager.activeSceneChanged += DestroyOnMenuScreen;

        net = FindObjectOfType<NetworkManager>();
        nethud = FindObjectOfType<NetworkManagerHUD>();
        mainCam = FindObjectOfType<MainCamera>();
    }
    
    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= DestroyOnMenuScreen;
    }

    void DestroyOnMenuScreen(Scene oldScene, Scene newScene)
    {
        if (newScene.buildIndex == 1)
        {
            net.gameObject.SetActive(true);
            nethud.gameObject.SetActive(true);
            mainCam.gameObject.SetActive(true);
            return;
        }
        if (newScene.buildIndex != oldScene.buildIndex) //0 == "Main Menu" (could compare scene name instead)
        {
            mainCam.gameObject.SetActive(false);
            nethud.gameObject.SetActive(false);
            Network.Disconnect();
            net.StopHost();
            net.StopClient();
            NetworkServer.Reset();
        }
    }
}
