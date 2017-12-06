using UnityEngine;
using UnityEngine.UI;
//TODO: quick fix for debugging, make this not update every frame and call from player stats instead of here
public class LivesUI : MonoBehaviour {

    public Text livesText;

    public int livesAmount = 20;

    void Update () {
        livesAmount = PlayerStats.Instance.lives;
        livesText.text = livesAmount + " LIVES";
	}
}
//TODO: Call text on event instead of every update
