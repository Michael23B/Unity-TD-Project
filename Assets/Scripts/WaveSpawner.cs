using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
//TODO: update all client nodes with correct turrets when clientjoins()
public class WaveSpawner : MonoBehaviour {
    
    public static WaveSpawner Instance;

    [HideInInspector]
    public int enemiesAlive = 0;

    [Header("Waves Array")]
    public EnemyWave[] waves;
    public Transform spawnPoint;

    [HideInInspector]
    public List<GameObject> enemyList = new List<GameObject>();

    public float waveCountdownTimer = 20f;
    private float countdown = 2f;
    private bool waveActive = false;

    public Text waveCountdownText;

    private int waveIndex = 0;
    public float waveMulti = 0;  //multiplier for repeating created waves

    LocalPlayerCommands player;
    public int playersReady = 0;

    public bool buildTime = true;
    Shop shop;

    public bool cleanUpScene = true;

    public int waitForPlayersCount = 1;
    private bool finishedWaveAndReady = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Error: More than one wave spawner in scene!");
            return;
        }
        Instance = this;

        shop = FindObjectOfType<Shop>();

        Random.InitState(1);

        if (cleanUpScene) CleanUpEnemies();
    }


    private void Update()
    {
        if (waveActive) return;
        if (enemiesAlive > 0) return;
        if (finishedWaveAndReady == true)
        {
            if (player == null) player = FindObjectOfType<LocalPlayerCommands>();
            player.CmdReady();
            finishedWaveAndReady = false;
        }
        if (playersReady < waitForPlayersCount) return;
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

        if (currentWave.randomOrder || waveMulti > 0) ShuffleArr(currentWave.wave);

        //yield return new WaitForSeconds(5f);    //wait for 5 seconds so all clients are caught up before starting

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

    void SpawnEnemy(GameObject enemy)
    {
        float rX = Random.Range(-1, 1);
        float rZ = Random.Range(-4, 4);

        GameObject e = Instantiate(enemy, spawnPoint.position + new Vector3(rX, 0f, rZ), spawnPoint.rotation);
        enemyList.Add(e);
        enemiesAlive++;
    }

    public void AddEnemy(GameObject enemy)  //for enemy-spawning enemies
    {
        enemyList.Add(enemy);
        enemiesAlive++;
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
}
