using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class LocalPlayerCommands : NetworkBehaviour
{
    public Shop shop;
    public NetworkAudio networkAudio;

    void Start()
    {
        shop = FindObjectOfType<Shop>();
        if (isLocalPlayer)
        {
            CmdMaxWaveUpdate(WaveSpawner.Instance.waveMax);
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
    public void CmdCreateGhost(int myID, int _GID, int enemyPrefabID, EnemyState state)
    {
        foreach (NetworkConnection target in NetworkServer.connections)
        {
            if (target.connectionId != myID)
            {
                TargetRpcCreateGhost(target, _GID, enemyPrefabID, state);
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
    public void CmdReduceLives(int amount)
    {
        RpcReduceLives(amount);
    }

    [Command]
    public void CmdPlayShootEffect(int myID, int shotID)    //one for left mouse fire, 2 for right mouse fire
    {
        foreach (NetworkConnection target in NetworkServer.connections)
        {
            if (target.connectionId != myID && target.connectionId != -1)
            {
                //TargetRpcPlayShootEffect(target, shotID);
                TargetRpcShoot(target, shotID);
            }
        }
    }

    [Command]
    public void CmdDamageEnemy(int myID, int enemyID, float damage)
    {
        foreach (NetworkConnection target in NetworkServer.connections)
        {
            if (target.connectionId != myID && target.connectionId != -1)
            {
                TargetRpcDamageEnemy(target, enemyID, damage);
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

    [Command]
    public void CmdPlant(int nodeID, int plantID)
    {
        RpcPlant(nodeID, plantID);
    }

    [Command]
    public void CmdHarvest(int nodeID)
    {
        RpcHarvest(nodeID);
    }

    [Command]
    public void CmdDestroyResource(int resourceID)
    {
        RpcDestroyResource(resourceID);
    }

    [Command]
    public void CmdMaxWaveUpdate(int numberOfWaves)
    {
        RpcMaxWaveUpdate(numberOfWaves);
    }

    [Command]
    public void CmdStartNextWave()
    {
        RpcStartNextWave();
    }

    [Command]
    public void CmdShootDamageUpdate(int amount)
    {
        RpcShootDamageUpdate(amount);
    }

    [Command]
    public void CmdRequestSceneState()  //sends the get scene state rpc which fetches the state from player '0' (host)
    {
        RpcGetSceneState();
    }

    [Command]
    public void CmdSendSceneState(NodeState[] nodeStates, float lightIntensity, bool fading, bool prevstate)
    {
        RpcSendSceneState(nodeStates, lightIntensity, fading, prevstate);
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
    {                                           //it looks like its working so im going to leave it
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
    void TargetRpcCreateGhost(NetworkConnection target, int _GID, int enemyPrefabID, EnemyState state)
    {
        WaveSpawner.Instance.SpawnGhost(EnemyManager.Instance.enemyPrefabList[enemyPrefabID], _GID, state);
    }

    [TargetRpc]
    void TargetRpcUpdateGhostPositions(NetworkConnection target, EnemyState[] state)
    {
        for (int i = 0; i < WaveSpawner.Instance.enemyGhostList.Count; ++i)
        {
            Enemy ghost = WaveSpawner.Instance.enemyGhostList[i].GetComponent<Enemy>(); //TODO: make enemy component list 

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
    void RpcReduceLives(int amount)
    {
        PlayerStats.Instance.lives -= amount;
    }

    [TargetRpc]
    void TargetRpcPlayShootEffect(NetworkConnection target, int id) 
    {
        GameObject p2 = GameObject.FindGameObjectWithTag("Player2");
        PlayerController player = p2.GetComponent<PlayerController>();
        if (player == null) return;

        if (id == 1)
        {
            player.gun.shootEffect.Play();
            player.gun.shootEffect.transform.rotation = player.gun.graphics.transform.rotation;
        }
        else if (id == 2) player.gun.AltShoot();
    }

    [TargetRpc]
    void TargetRpcShoot(NetworkConnection target, int id) //actually damages enemy with shot
    {
        GameObject p2 = GameObject.FindGameObjectWithTag("Player2");
        PlayerController player = p2.GetComponent<PlayerController>();
        if (player == null) return;

        if (id == 1)
        {
            player.gun.Shoot();
            //player.gun.shootEffect.transform.rotation = player.gun.graphics.transform.rotation;
        }
        else if (id == 2) player.gun.AltShoot();
    }

    [TargetRpc]
    void TargetRpcDamageEnemy(NetworkConnection target, int id, float damage)
    {
        foreach (GameObject enemy in WaveSpawner.Instance.enemyList)
        {
            Enemy enemyComponent = enemy.GetComponent<Enemy>();

            if (enemyComponent.ID == id)
            {
                enemyComponent.TakeDamage(damage);
            }
        }
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

    [ClientRpc]
    void RpcPlant(int nodeID, int plantID)
    {
        GardenNode _node = GardenNodes.gardenNodes[nodeID].GetComponent<GardenNode>();

        if (_node.plantComponent != null) Destroy(_node.plant);
        _node.Plant(GardenNodeUI.plantListReference[plantID]);
    }

    [ClientRpc]
    void RpcHarvest(int nodeID)
    {
        GardenNode _node = GardenNodes.gardenNodes[nodeID].GetComponent<GardenNode>();

        if (_node.plantComponent == null) return;
        _node.plantComponent.Harvest(true);
    }

    [ClientRpc]
    void RpcDestroyResource(int resourceID)
    {
        foreach (Resource resource in ResourceSpawner.Instance.resources)
        {
            if (resource.ID == resourceID)
            {
                resource.PlayHitEffect();
                Destroy(resource.gameObject);
                return;
            }
        }
    }

    [ClientRpc]
    void RpcMaxWaveUpdate(int numberOfWaves)
    {
        WaveSpawner.Instance.CallGenerateAllWaves(numberOfWaves);
    }

    [ClientRpc]
    void RpcStartNextWave()
    {
        WaveSpawner.Instance.StartNextWave();
    }

    [ClientRpc]
    public void RpcShootDamageUpdate(int amount)
    {
        Gun[] allGuns = FindObjectsOfType<Gun>();

        foreach (Gun gun in allGuns)
        {
            gun.damage += amount;
            BuildManager.Instance.message.PlayMessage("Gun Upgraded! (" + gun.damage + " damage total)", gun.transform, Color.green, 0.5f, 1.5f);
        }
    }

    [ClientRpc]
    void RpcGetSceneState() //get the scene on the host, then send it to the client
    {
        if (WaveSpawner.Instance.playerID == 0) //only the host should get and send scene state
        {
            float lightIntensity = 1;
            NodeState[] nodeStates = new NodeState[Nodes.nodes.Length];
            Shop shop = FindObjectOfType<Shop>();
            int j = 0;  //iterator for nodeStates[]

            //get scene data (nodes data and light intensity)
            foreach (Transform node in Nodes.nodes) //for each node on the map, check if it has a turret
            {
                Node nodeComponent = node.GetComponent<Node>();
                if (nodeComponent.turretBlueprint == null) continue;    //if it has no turret, ignore and check next node

                for (int i = 0; i < shop.Blueprints.Length; ++i)
                {
                    if (nodeComponent.turretBlueprint == shop.Blueprints[i])    //if it has a turret, compare it to the turrets in the shop to get its ID
                    {
                        nodeStates[j].nodeID = nodeComponent.nodeID;
                        nodeStates[j].turretID = i; //blueprints index matches turret to build index
                        nodeStates[j].isUpgraded = nodeComponent.isUpgraded;
                        j++;
                        break;
                    }
                }
            }

            Array.Resize(ref nodeStates, j);

            LightControl lightCon = FindObjectOfType<LightControl>();
            lightIntensity = lightCon.directionalLight.intensity;
            bool fading = lightCon.fading;
            bool prevstate = lightCon.prevState;

            CmdSendSceneState(nodeStates, lightIntensity, fading, prevstate);
        }
    }

    [ClientRpc]
    void RpcSendSceneState(NodeState[] nodeStates, float lightIntensity, bool fading, bool prevstate)
    {
        if (WaveSpawner.Instance.playerID == 0 || WaveSpawner.Instance.playerID == -1) return;  //host and server shouldnt recieve state updates
        Shop shop = FindObjectOfType<Shop>();
        
        foreach (Transform node in Nodes.nodes) //for each node on the map, check if it has a turret
        {
            Node nodeComponent = node.GetComponent<Node>();
            foreach (NodeState state in nodeStates) //if there is a state for that nodeID, update the node
            {
                if (nodeComponent.nodeID == state.nodeID)
                {
                    nodeComponent.BuildTurret(shop.Blueprints[state.turretID]);
                    if (state.isUpgraded) nodeComponent.UpgradeTurret();
                    break;
                }
            }
            //match lightcontroller to hosts
            LightControl lightCon = FindObjectOfType<LightControl>();
            lightCon.directionalLight.intensity = lightIntensity;
            lightCon.fading = fading;
            lightCon.prevState = prevstate;
        }
    }
    #endregion
}