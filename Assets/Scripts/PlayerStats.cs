using UnityEngine;

public class PlayerStats : MonoBehaviour {

    public static PlayerStats Instance;

    [HideInInspector]
    public int money;
    public int startMoney = 400;

    [HideInInspector]
    public int lives;
    public int startLives = 20;

    [HideInInspector]
    public int rounds;

    //resources
    public int stone = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Error: More than one player stats in scene!");
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        money = startMoney;
        lives = startLives;
        rounds = 0;
    }
}
