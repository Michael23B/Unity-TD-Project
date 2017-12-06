using UnityEngine;
using UnityEngine.SceneManagement;
//this was made with single player in mind, need fix a lot for mp
public class PauseMenu : MonoBehaviour {

    public GameObject ui;
    public string menuSceneName = "Main Menu";
    public SceneFader sceneFader;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        ui.SetActive(!ui.activeSelf);   //if disabled, enables, if enabled, disables
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
}
