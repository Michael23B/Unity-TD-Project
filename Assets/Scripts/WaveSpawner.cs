using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
//TODO: update all client nodes with correct turrets when clientjoins()

public class WaveSpawner : MonoBehaviour {
    
    public static WaveSpawner Instance;

    [HideInInspector]
    public int enemiesAlive = 0;

    [Header("Waves Array")]
    public EnemyWave[] waves;
    public Transform spawnPoint;
    
    [HideInInspector]
    public EnemyWave[] ghostWaves;
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
    public float waveMulti = 0;  //multiplier for repeating created waves

    public int playerID;
    public LocalPlayerCommands commands;
    public int playersReady = 0;

    public bool buildTime = true;
    Shop shop;

    public bool cleanUpScene = true;

    public int waitForPlayersCount = 1;
    private bool finishedWaveAndReady = false;

    LocalRandom rand = LocalRandom.Instance;
    [Space(10)]
    public Material ghostMaterial;
    public bool ghostOnly = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Error: More than one wave spawner in scene!");
            return;
        }
        Instance = this;

        shop = FindObjectOfType<Shop>();

        if (cleanUpScene) CleanUpEnemies();
        rand = LocalRandom.Instance;
        ghostWaves = waves; //take this out and make a seperate wave array for each player
        InvokeRepeating("UpdateGhostPositions", 0f, 5f);
    }

    private void Update()
    {
        if (waveActive) return;
        if (enemiesAlive > 0) return;
        if (finishedWaveAndReady == true)
        {
            if (commands == null) commands = FindObjectOfType<LocalPlayerCommands>();
            commands.CmdReady();
            finishedWaveAndReady = false;
        }
        if (playersReady < waitForPlayersCount) return;
        if (countdown <= 0f)
        {
            buildTimeToggle();
            if (!ghostOnly) StartCoroutine(SpawnWave());
            StartCoroutine(SpawnGhostWave());

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
        buildTime = !buildTime;
        BuildManager.Instance.SelectTurretToBuild(null, -1);
        BuildManager.Instance.DeselectNode();
        shop.gameObject.SetActive(!shop.isActiveAndEnabled);
    }

    IEnumerator SpawnWave()
    {
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

        waveMulti = waveIndex * 0.1f;

        if (waveIndex == waves.Length * 5)
        {
            Debug.Log("Level Complete!");
            enabled = false;
        }
    }

    IEnumerator SpawnGhostWave()
    {
        EnemyWave currentWave = ghostWaves[waveIndex % waves.Length];   //ghostwaves will be populated by the other player 

        if (currentWave.randomOrder) ShuffleArr(currentWave.wave);

        for (int i = 0; i < currentWave.wave.Length; ++i)
        {
            for (int n = 0; n < currentWave.wave[i].count; ++n)
            {
                SpawnGhost(currentWave.wave[i].enemy);
                yield return new WaitForSeconds(1f / currentWave.wave[i].spawnRate);
            }
            yield return new WaitForSeconds(currentWave.wave[i].waitTime);
        }
        waveIndex++;
        waveActive = false;
        finishedWaveAndReady = true;

        waveMulti = waveIndex * 0.1f;

        if (waveIndex == waves.Length * 5)
        {
            Debug.Log("Level Complete!");
            enabled = false;
        }
    }

    void SpawnEnemy(GameObject enemy)
    {
        float rX = rand.GetNextRandom(1f, false);//Random.Range(-1, 1);
        float rZ = rand.GetNextRandom(4f, false);//Random.Range(-4, 4);

        GameObject e = Instantiate(enemy, spawnPoint.position + new Vector3(rX, 0f, rZ), spawnPoint.rotation);
        enemyList.Add(e);
        enemiesAlive++;
    }

    void SpawnGhost(GameObject enemy)
    {
        float rX = rand.GetNextRandom(1f, false);//Random.Range(-1, 1);
        float rZ = rand.GetNextRandom(4f, false);//Random.Range(-4, 4);

        GameObject e = Instantiate(enemy, ghostSpawnPoint.position + new Vector3(rX, 0f, rZ), ghostSpawnPoint.rotation);
        enemyGhostList.Add(e);

        Enemy enemyComponent = e.GetComponent<Enemy>();
        Renderer enemyRenderer = e.GetComponent<Renderer>();

        enemyRenderer.material = ghostMaterial;
        enemyRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        enemyComponent.ghost = true;
    }

    public void AddEnemy(GameObject enemy, bool ghost = false)  //for enemy-spawning enemies
    {
        if (ghost)
        {
            enemyGhostList.Add(enemy);

            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            Renderer enemyRenderer = enemy.GetComponent<Renderer>();

            enemyRenderer.material = ghostMaterial;
            enemyRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
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
            int r = rand.GetNextRandom(arr.Length-1);//Random.Range(i, arr.Length);
            //TODO: this shuffle will not be as random as if i increasing reduced the range accordingly (like the first algorithm)
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
        EnemyWave[] wavesTemp = new EnemyWave[waves.Length];
        Transform[] waypointsTemp = new Transform[Waypoints.points.Length];
        //copy to temp
        waves.CopyTo(wavesTemp, 0);
        spawnTemp = spawnPoint;
        Waypoints.points.CopyTo(waypointsTemp, 0);
        //make alternate paths, spawnpoints and waves the main
        spawnPoint = ghostSpawnPoint;
        ghostWaves.CopyTo(waves, 0);
        WaypointsAlternate.pointsAlternate.CopyTo(Waypoints.points, 0);
        //copy main back to alternate
        ghostSpawnPoint = spawnTemp;
        wavesTemp.CopyTo(ghostWaves, 0);
        waypointsTemp.CopyTo(WaypointsAlternate.pointsAlternate, 0);
    }

    public void UpdateGhostPositions()
    {
        if (enemyList.Count == 0) return;

        EnemyState[] state = new EnemyState[enemyList.Count];

        for (int i = 0; i < state.Length; ++i)
        {
            state[i] = new EnemyState();
            Enemy e = enemyList[i].GetComponent<Enemy>();   //TODO: stop getting component and add a reference on the enemy itself

            state[i].ID = e.ID; // state[i] is null fix
            state[i].movementTarget = e.enemyMovement.waypointIndex;   //TODO: change this to just send the target index and update on the client
            state[i].pos = enemyList[i].transform.position;
        }

        commands.CmdUpdateGhostPositions(playerID, state);
    }
}
