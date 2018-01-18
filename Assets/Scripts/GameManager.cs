using UnityEngine;

public class GameManager : MonoBehaviour {
    
    public static bool gameIsOver = false;

    public GameObject gameOverUI;

    private void Start()
    {
        gameIsOver = false;
    }

    void Update () {
        if (gameIsOver) return;
		if (PlayerStats.Instance.lives <= 0)
        {
            EndGame();
        }
	}
    void EndGame()
    {
        gameIsOver = true;
        gameOverUI.SetActive(true);
    }
}