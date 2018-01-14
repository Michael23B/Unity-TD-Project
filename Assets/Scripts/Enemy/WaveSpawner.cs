using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
//TODO: update all client nodes with correct turrets when clientjoins()
public class WaveSpawner : MonoBehaviour {
    
    public static WaveSpawner Instance;

    [HideInInspector]
    public int enemiesAlive = 0;

    [Header("Waves Array")]
    public EnemyWave[] waves;
    public Transform spawnPoint;
    
    [HideInInspector]
    public Transform ghostSpawnPoint;

    [HideInInspector]
    public List<GameObject> enemyList = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> enemyGhostList = new List<GameObject>();

    public float waveCountdownTimer = 20f;
    private float countdown = 2f;
    private bool waveActive = false;

    public Text waveCountdownText;

    private int waveIndex = 0;
    [HideInInspector]
    public int nextWaveIndex = 0;
    public float waveMulti = 0;  //multiplier for repeating created waves

    public int playerID;
    public LocalPlayerCommands commands;
    public int playersReady = 0;

    public bool buildTime = true;
    Shop shop;

    public bool cleanUpScene = true;

    public int waitForPlayersCount = 1;
    private bool finishedWaveAndReady = false;

    [Space(10)]
    public Material ghostMaterial;

    public Camera sceneCamera;
    public TacticalCamera tacticalCamera;
    [Space(10)]
    public Text enemyCountText;
    public Text waveMultiText;
    public Text waveIndexText;

    public bool gameStarted = false;    //has game started?

    public int waveMax;
    public EnemyWaveHelper enemyWaveHelper; //for generating random waves

    [HideInInspector]
    public PlayerController localPlayer;    //for getting transform of local player
    public TurretSelect turretSelect;   //for re-enabling turret select window from resources

    public Button startNextWaveBtn;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Error: More than one wave spawner in scene!");
            return;
        }
        Instance = this;

        shop = FindObjectOfType<Shop>();
        if (commands == null) commands = FindObjectOfType<LocalPlayerCommands>();
        if (NetworkServer.connections.Count != 0) waitForPlayersCount = NetworkServer.connections.Count;

        if (cleanUpScene) CleanUpEnemies();
        InvokeRepeating("UpdateGhostPositions", 0f, 5f);
        GenerateAllWaves();
    }

    private void Update()
    {
        enemyCountText.text = enemiesAlive.ToString();
        if (PlayerStats.Instance.lives <= 0) enabled = false;
        if (waveActive) return;
        if (enemiesAlive > 0) return;

        if (finishedWaveAndReady == true)
        {
            if (commands == null) commands = FindObjectOfType<LocalPlayerCommands>();
            commands.CmdReady();
            finishedWaveAndReady = false;
        }
        if (playersReady < waitForPlayersCount) return;

        if (waveIndex >= waveMax)
        {
            BuildManager.Instance.message.PlayMessage("ALL WAVES COMPLETE!", transform, Color.green, 0.1f, 1, 2);
            enabled = false;
            return;
        }

        if (countdown <= 0f)
        {
            buildTimeToggle();
            StartCoroutine(SpawnWave());

            countdown = waveCountdownTimer;
            return;
        }

        if (!buildTime) buildTimeToggle();
        countdown -= Time.deltaTime;
        countdown = Mathf.Clamp(countdown, 0f, Mathf.Infinity);
        waveCountdownText.text = string.Format("{0:00.0}", countdown);
    }

    void buildTimeToggle()
    {
        if (!buildTime)
        {
            BuildManager.Instance.message.PlayMessage("WAVE " + (waveIndex + 1) + " APPROACHING", transform, Color.green, 1, 1, 2);
            startNextWaveBtn.gameObject.SetActive(true);
        }
        else startNextWaveBtn.gameObject.SetActive(false);
        if (!buildTime && playerID == 0) //going from wave -> buildTime. If server, send new random values for the next wave
        {
            commands.CmdSetClientsRandomValues();
        }

        buildTime = !buildTime;
        BuildManager.Instance.SelectTurretToBuild(null, -1);
        BuildManager.Instance.DeselectNode();
        shop.gameObject.SetActive(!shop.isActiveAndEnabled);
    }

    IEnumerator SpawnWave()
    {
        gameStarted = true;
        nextWaveIndex = waveIndex + 1;
        waveIndexText.text = "WAVE " + (waveIndex + 1) + "/" + waveMax + " ARRIVED";
        waveMultiText.text = "ENEMY STRENGTH MULTIPLIER: " + (1 + waveMulti).ToString("F1") + "x";

        playersReady = 0;
        ResourceSpawner.Instance.SpawnResources(25);
        PlayerStats.Instance.rounds++;

        EnemyWave currentWave = waves[waveIndex % waves.Length];
        waveActive = true;

        if (currentWave.randomOrder) ShuffleArr(currentWave.wave);

        for (int i = 0; i < currentWave.wave.Length; ++i)
        {
            for (int n = 0; n < currentWave.wave[i].count; ++n)
            {
                SpawnEnemy(currentWave.wave[i].enemy);
                yield return new WaitForSeconds(1f / currentWave.wave[i].spawnRate);
            }
            yield return new WaitForSeconds(currentWave.wave[i].waitTime);
        }
        waveIndex++;
        waveActive = false;
        finishedWaveAndReady = true;

        if (nextWaveIndex > waveMax)    //if all waves have arrived, display winning text and final enemy strength
        {
            waveIndexText.text = "WAVE " + (waveIndex) + "/" + waveMax + "COMPLETE!";
            waveMultiText.text = "ENEMY STRENGTH MULTIPLIER: " + (1 + waveMulti).ToString("F1") + "x";
        }
        else
        {   //update enemy strength multiplier and display wave info
            waveMulti = CalcWaveMulti();

            waveIndexText.text = "WAVE " + (waveIndex + 1) + "/" + waveMax + "INCOMING";
            waveMultiText.text = "ENEMY STRENGTH MULTIPLIER: " + (1 + waveMulti).ToString("F1") + "x";
        }
    }

    float CalcWaveMulti()    //difficulty curve of the game
    {
        return (waveIndex * (Mathf.Sqrt(waveIndex))) * 0.1f;
    }

    void SpawnEnemy(GameObject enemy)
    {
        float rX = Random.Range(-1, 1); //rand.GetNextRandom(1f, false);
        float rZ = Random.Range(-4, 4); //rand.GetNextRandom(4f, false);

        GameObject e = Instantiate(enemy, spawnPoint.position + new Vector3(rX, 0f, rZ), spawnPoint.rotation);
        enemyList.Add(e);
        enemiesAlive++;
    }

    public void SpawnGhost(GameObject enemy, int _GID, EnemyState state)
    {
        GameObject e = Instantiate(enemy, ghostSpawnPoint.position, ghostSpawnPoint.rotation);
        enemyGhostList.Add(e);

        Enemy enemyComponent = e.GetComponent<Enemy>();
        Renderer[] enemyRenderer = e.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in enemyRenderer)
        {
            renderer.material = ghostMaterial;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        enemyComponent.ghost = true;
        enemyComponent.GID = _GID;

        //set state
        enemyComponent.transform.position = state.pos;
        enemyComponent.GetComponent<EnemyMovement>().SetAndUpdateWaypoint(state.movementTarget);
    }

    public void AddEnemy(GameObject enemy, bool ghost = false)  //for enemy-spawning enemies
    {
        if (ghost)
        {
            enemyGhostList.Add(enemy);

            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            Renderer[] enemyRenderer = enemyComponent.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in enemyRenderer)
            {
                renderer.material = ghostMaterial;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            enemyComponent.ghost = true;
        }
        else
        {
            enemyList.Add(enemy);
            enemiesAlive++;
        }
    }

    //TODO: add general class for helper function like this and update target (bullets,turrets,enemies) and some other stuff
    public void ShuffleArr<T>(T[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            T tmp = arr[i];
            int r = Random.Range(i, arr.Length);
            arr[i] = arr[r];
            arr[r] = tmp;
        }
    }

    void CleanUpEnemies()    //cleans any enemies from the scene on load
    {
        Enemy[] enemiesToCleanUp = GameObject.FindObjectsOfType<Enemy>();

        foreach (Enemy e in enemiesToCleanUp)
        {
            EnemyAttack eAttack = e.GetComponent<EnemyAttack>();
            if (eAttack != null) eAttack.isQuitting = true; //So it won't spawn more enemies
            Ondeath eOndeath = e.GetComponent<Ondeath>();
            if (eOndeath != null) eOndeath.isQuitting = true;

            DestroyImmediate(e.gameObject);
        }
    }

    public void AlternateSpawner()
    {
        Transform spawnTemp;
        Transform[] waypointsTemp = new Transform[Waypoints.points.Length];

        spawnTemp = spawnPoint;
        Waypoints.points.CopyTo(waypointsTemp, 0);

        spawnPoint = ghostSpawnPoint;
        WaypointsAlternate.pointsAlternate.CopyTo(Waypoints.points, 0);

        ghostSpawnPoint = spawnTemp;
        waypointsTemp.CopyTo(WaypointsAlternate.pointsAlternate, 0);
    }

    public void UpdateGhostPositions()
    {
        if (enemyList.Count == 0) return;

        EnemyState[] state = new EnemyState[enemyList.Count];

        for (int i = 0; i < state.Length; ++i)
        {
            state[i] = new EnemyState();
            Enemy e = enemyList[i].GetComponent<Enemy>();   //TODO: add enemy component list to wavespawner or make a manager class for enemies rather than getting component every time

            state[i].ID = e.ID;
            state[i].movementTarget = e.enemyMovement.waypointIndex;
            state[i].pos = enemyList[i].transform.position;
        }

        commands.CmdUpdateGhostPositions(playerID, state);
    }

    void GenerateAllWaves()
    {
        waves = new EnemyWave[waveMax];
        for (int i = 0; i < waveMax; ++i)
        {
            waves[i] = enemyWaveHelper.GenerateWave(i, (4 + i));
        }
    }

    public void CallGenerateAllWaves(int numberOfWaves) //called from joining clients to sync max waves with host or creates a new array of waves based on the new number
    {
        if (gameStarted) return;
        if (numberOfWaves > 99) numberOfWaves = 99;
        if (waveMax != numberOfWaves)
        {
            waveMax = numberOfWaves;
            GenerateAllWaves();
        }
    }

    public void StartNextWave()
    {
        if (!buildTime) return;
        if (countdown <= 0) return;

        PlayerStats.Instance.money += (int)Mathf.Round(countdown);
        countdown = 0;

        waveCountdownText.text = string.Format("{0:00.0}", countdown);
    }

    public void CallStartNextWave()
    {
        if (!buildTime) return;
        if (countdown <= 0) return;
        commands.CmdStartNextWave();
    }
    //if you bankai then hit gun upgrade you will give super damage to other player forever fix this TODO:
    public void CallGunUpgrade(int amount)
    {
        commands.CmdShootDamageUpdate(amount);
    }
}
