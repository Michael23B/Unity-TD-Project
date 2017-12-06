using UnityEngine;

public class ReadyPlayer : MonoBehaviour {

    LocalPlayerCommands player;

    public void PlayerReady()
    {
        if (player == null) player = FindObjectOfType<LocalPlayerCommands>();
        player.CmdReady();
        gameObject.SetActive(false);
    }
}
