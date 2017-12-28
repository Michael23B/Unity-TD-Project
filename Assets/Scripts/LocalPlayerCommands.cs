using UnityEngine.Networking;
using UnityEngine;

public class LocalPlayerCommands : NetworkBehaviour
{
    public Shop shop;
    public NetworkAudio networkAudio;

    void Start()
    {
        shop = FindObjectOfType<Shop>();
        if (isLocalPlayer)
        {
            CmdUpdatePlayerCount();
            WaveSpawner.Instance.playerID = connectionToServer.connectionId;
        }
    }


    #region Networking
    [Command]
    public void CmdBuildTurret(int nodeID, int turretID, bool upgrading)
    {
        RpcBuildTurret(nodeID, turretID, upgrading);
    }

    [Command]
    public void CmdSellTurret(int nodeID)
    {
        RpcSellTurret(nodeID);
    }

    [Command]
    public void CmdReady()
    {
        RpcReady();
    }

    [Command]
    public void CmdUnleashThis(NetworkIdentity netID)
    {
        RpcUnleashThis(netID);
    }

    [Command]
    public void CmdPlaySound(int index)
    {
        RpcPlaySound(index);
    }

    [Command]
    public void CmdUpdatePlayerCount()
    {
        int playerCount = NetworkServer.connections.Count;
        RpcUpdatePlayerCount(playerCount);
    }

    [Command]
    public void CmdSetClientsRandomValues()
    {
        LocalRandom rand = FindObjectOfType<LocalRandom>();
        float[] arr = rand.GetRandomValues();
        RpcSetClientsRandomValues(arr);
    }

    [Command]
    public void CmdKillGhost(int enemyID, int netID)
    {
        RpcKillGhost(enemyID, netID);
    }

    //RPCs
    [ClientRpc]
    void RpcBuildTurret(int nodeID, int turretID, bool upgrading)
    {
        Node _node = Nodes.nodes[nodeID].GetComponent<Node>();

        if (_node.turret != null && !upgrading) Destroy(_node.turret);

        if(upgrading) _node.UpgradeTurret();
        else _node.BuildTurret(shop.Blueprints[turretID]);
    }

    [ClientRpc]
    void RpcSellTurret(int nodeID)
    {
        Node _node = Nodes.nodes[nodeID].GetComponent<Node>();

        if (_node.turret == null) return;

        _node.SellTurret();
    }

    [ClientRpc]
    void RpcReady()
    {
        WaveSpawner.Instance.playersReady++;
    }

    [ClientRpc]
    void RpcUnleashThis(NetworkIdentity netID)  //this is probably not working the way i thought. finding the networkid of a playercontroller doesnt work like that
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();

        foreach (PlayerController player in players)
        {
            if(player.GetComponent<NetworkIdentity>() == netID) player.UnleashThis();
        }
    }

    [ClientRpc]
    void RpcPlaySound(int index)
    {
        networkAudio.source.PlayOneShot(networkAudio.soundClips[index]);
    }

    [ClientRpc]
    void RpcUpdatePlayerCount(int playerCount)
    {
        WaveSpawner.Instance.waitForPlayersCount = playerCount;
    }

    [ClientRpc]
    void RpcSetClientsRandomValues(float[] arr)
    {
        LocalRandom.Instance.randomValues = arr;
        LocalRandom.Instance.index = 0;
    }
    
    [ClientRpc]
    void RpcKillGhost(int GID, int netID)
    {
        if (WaveSpawner.Instance.playerID == netID) return;   //ignore the player that called it

        foreach (GameObject ghost in WaveSpawner.Instance.enemyGhostList)
        {
            Enemy ghostComponent = ghost.GetComponent<Enemy>();
            if (ghostComponent.GID == GID)
            {
                ghostComponent.Kill();
                return;
            }
        }
    }
    #endregion
}