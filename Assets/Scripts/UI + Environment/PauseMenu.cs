using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
//this was made with single player in mind, need fix a lot for mp
public class PauseMenu : MonoBehaviour {

    public GameObject ui;
    public string menuSceneName = "Main Menu";
    public SceneFader sceneFader;
    NetworkManagerHUD networkHUD;

    private void Start()
    {
        networkHUD = FindObjectOfType<NetworkManagerHUD>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        ui.SetActive(!ui.activeSelf);
        /*  Don't pause time in multiplayer (TODO: let it pause if 1 player)
        if(ui.activeSelf)
        {
            Time.timeScale = 0f;
        } else
        {
            Time.timeScale = 1f;
        }
        */
    }

    public void Retry()
    {
        Toggle();
        sceneFader.FadeTo(SceneManager.GetActiveScene().name);
    }

    public void Menu()
    {
        Toggle();
        sceneFader.FadeTo(menuSceneName);
    }

    public void CallRetry()
    {
        WaveSpawner.Instance.commands.CmdResetLevel();
    }

    public void ToggleNetworkHUD()
    {
        networkHUD.gameObject.SetActive(!networkHUD.isActiveAndEnabled);
    }

    public void ToggleNetworkHUD(bool b)
    {
        networkHUD.gameObject.SetActive(b);
    }

    private void OnPlayerConnected(NetworkPlayer player)
    {
        ToggleNetworkHUD(false);
    }

    private void OnPlayerDisconnected(NetworkPlayer player)
    {
        ToggleNetworkHUD(true);
    }
}
