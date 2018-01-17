using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {

    public Text roundsText;
    public string menuSceneName = "Main Menu";
    public SceneFader sceneFader;

    private void OnEnable()
    {
        roundsText.text = PlayerStats.Instance.rounds.ToString();
    }

    public void Retry ()
    {
        sceneFader.FadeTo(SceneManager.GetActiveScene().name);
    }
    //Unity Settings -> Window -> Lighting -> Scene -> Auto
    //Auto needs to be off when resetting the scene, so if lighting needs to change, turn on auto and then off when finished

    public void Menu ()
    {
        sceneFader.FadeTo(menuSceneName);
    }

    public void Quit()
    {
        Network.Disconnect(200);
        Application.Quit();
    }
}
