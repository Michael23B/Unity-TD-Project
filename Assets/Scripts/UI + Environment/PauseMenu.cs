using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

    public GameObject ui;
    public string menuSceneName = "Main Menu";
    public SceneFader sceneFader;
    NetworkManagerHUD networkHUD;
    public InputField wavesInput;

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

    public void Quit()
    {
        Debug.Log("Quitting...");
        Network.Disconnect(200);
        Application.Quit();
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

    public void CallMaxWaveUpdate()
    {
        if (WaveSpawner.Instance.gameStarted) return;

        int numberOfWaves = 0;
        numberOfWaves = int.Parse(wavesInput.textComponent.text);
        if (numberOfWaves <= 0) return;

        WaveSpawner.Instance.commands.CmdMaxWaveUpdate(numberOfWaves);
    }
}
