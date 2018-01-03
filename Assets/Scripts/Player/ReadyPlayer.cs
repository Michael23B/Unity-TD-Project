using UnityEngine;

public class ReadyPlayer : MonoBehaviour {

    LocalPlayerCommands player;

    [SerializeField]
    Shop shop;

    [SerializeField]
    TurretSelect turretSelect;

    public void PlayerReady()
    {
        if (player == null) player = FindObjectOfType<LocalPlayerCommands>();
        player.CmdReady();
        gameObject.SetActive(false);
    }

    public void TurretSelectMenu()
    {
        turretSelect.Show();
    }
}
