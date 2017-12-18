using UnityEngine;

public class ReadyPlayer : MonoBehaviour {

    LocalPlayerCommands player;

    [SerializeField]
    Shop shop;

    public void PlayerReady()
    {
        if (player == null) player = FindObjectOfType<LocalPlayerCommands>();
        player.CmdReady();
        gameObject.SetActive(false);
    }

    public void SelectPlayerOne()
    {
        shop.EnableTurrets(shop.player1Turrets);
        gameObject.SetActive(false);
    }

    public void SelectPlayerTwo()
    {
        shop.EnableTurrets(shop.player2Turrets);
        gameObject.SetActive(false);
    }
}
