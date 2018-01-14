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
    public Button waveGenerateBtn;
    bool initialSetUp = false;

    private void Start()
    {
        networkHUD = FindObjectOfType<NetworkManagerHUD>();
    }

    private void Update()
    {
        if (WaveSpawner.Instance.gameStarted && !initialSetUp)
        {
            networkHUD.gameObject.SetActive(false);
            wavesInput.gameObject.SetActive(false);
            waveGenerateBtn.gameObject.SetActive(false);
            initialSetUp = true;
        }
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
        if (wavesInput.textComponent.text == "") return;

        int numberOfWaves = 0;
        numberOfWaves = int.Parse(wavesInput.textComponent.text);
        wavesInput.text = "";
        if (numberOfWaves <= 0) return;

        WaveSpawner.Instance.commands.CmdMaxWaveUpdate(numberOfWaves);
    }

    private void OnDisable()
    {
       if (networkHUD != null) networkHUD.gameObject.SetActive(true);
    }
}
