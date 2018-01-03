using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        shop = FindObjectOfType<Shop>();
        if (isLocalPlayer)
        {
            CmdUpdatePlayerCount();
            WaveSpawner.Instance.playerID = connectionToServer.connectionId;
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
    public void CmdKillGhost(int myID, int enemyID)
    {
        foreach (NetworkConnection target in NetworkServer.connections)
        {
            if (target.connectionId != myID)
            {
                TargetRpcKillGhost(target, enemyID);
            }
        }
    }

    [Command]
    public void CmdUpdateGhostPositions(int myID, EnemyState[] state)
    {
        foreach (NetworkConnection target in NetworkServer.connections)
        {
            if (target.connectionId != myID && target.connectionId != -1)
            {
                TargetRpcUpdateGhostPositions(target, state);
            }
        }
    }

    [Command]
    public void CmdReduceLives()
    {
        RpcReduceLives();
    }

    [Command]
    public void CmdPlayShootEffect(int myID, int shotID)    //one for left mouse fire, 2 for right mouse fire
    {
        foreach (NetworkConnection target in NetworkServer.connections)
        {
            if (target.connectionId != myID && target.connectionId != -1)
            {
                TargetRpcPlayShootEffect(target, shotID);
            }
        }
    }

    [Command]
    public void CmdPlayerColorsUpdate()
    {
        RpcPlayerColorsUpdate();
    }

    [Command]
    public void CmdResetLevel()
    {
        RpcResetLevel();
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
    
    [TargetRpc]
    void TargetRpcKillGhost(NetworkConnection target, int GID)
    {
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

    [TargetRpc]
    void TargetRpcUpdateGhostPositions(NetworkConnection target, EnemyState[] state)
    {
        for (int i = 0; i < WaveSpawner.Instance.enemyGhostList.Count; ++i)
        {
            Enemy ghost = WaveSpawner.Instance.enemyGhostList[i].GetComponent<Enemy>(); //TODO: make enemy component list in 

            for (int j = 0; j < state.Length; ++j)  //check if this ghosts id is in the state array (redundant since the enemies should be in the same order in both arrays, but this is very safe)
            {
                if (ghost.GID == state[j].ID)
                {
                    ghost.transform.position = state[j].pos;
                    ghost.enemyMovement.SetAndUpdateWaypoint(state[j].movementTarget);
                    break;
                }
            }
        }
    }

    [ClientRpc]
    void RpcReduceLives()
    {
        PlayerStats.Instance.lives--;
    }

    [TargetRpc]
    void TargetRpcPlayShootEffect(NetworkConnection target, int id) //TODO: change to a grey shoot effect so its obvious the player can't hurt the other players enemies
    {
        Gun gun = GetComponent<Gun>();
        if (id == 1)
        {
            gun.shootEffect.Play();
            gun.shootEffect.transform.rotation = gun.graphics.transform.rotation;
        }
        else if (id == 2) gun.AltShoot();
    }

    [ClientRpc]
    void RpcPlayerColorsUpdate()
    {
        if (WaveSpawner.Instance.playerID == 0)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.grey;

            GameObject p2 = GameObject.FindGameObjectWithTag("Player2");
            if (p2 != null) p2.GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
        }
        else
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;

            GameObject p2 = GameObject.FindGameObjectWithTag("Player2");
            if (p2 != null) p2.GetComponentInChildren<MeshRenderer>().material.color = Color.grey;
        }
    }

    [ClientRpc]
    void RpcResetLevel()
    {
        FindObjectOfType<PauseMenu>().Retry();
    }
    #endregion
}