using UnityEngine;

//Spawns prefabs or shoots targets in range, when shooting picks a random target every shot
//TODO: scale with wavemulti

[RequireComponent(typeof(Enemy))]
public class EnemyAttack : MonoBehaviour {

    public float fireRate = 0.2f;

    private float countDown;

    [Header("Spawn an object with Spawn script?")]
    public bool useSpawner = true;
    public GameObject spawnPrefab;
    [Tooltip("if the spawn should be removed when the parent is, or if you want the spawn to follow the parent")]
    public bool setParent = true;
    public bool isEnemy = false;    //is the spawn an enemy?

    [Header("Gun?")]
    public bool useBullets = false;
    public GameObject bulletPrefab;

    public float damage = 0f;
    public float range = 10f;

    public string enemyTag = "Enemy";
    private Transform target;

    private void Start()
    {
        countDown = 1 / fireRate;
    }

    void Update () {
        if (countDown <= 0)
        {
            if(useSpawner) Spawn();
            if(useBullets)
            {
                UpdateTarget();
                if (target == null) {
                    countDown = 1f; //if theres no target in range, wait for 1 second before trying again
                    return;
                }
                else Shoot();
            }
            countDown = 1 / fireRate;
        }
        else countDown -= Time.deltaTime;
	}

    void Spawn()
    {
        GameObject spawnins = Instantiate(spawnPrefab, transform.position, transform.rotation);
        if (setParent) spawnins.transform.SetParent(transform);
        if (isEnemy)
        {
            WaveSpawner.Instance.AddEnemy(spawnins);

            int parentWaypoint = GetComponent<EnemyMovement>().GetWaypoint;
            spawnins.GetComponent<EnemyMovement>().SetWaypoint(parentWaypoint);
        }
    }
    //
    //TODO: Add update target and shoot and stuff to a seperate class instead of re defining them here and in bullet and turret
    //
    void Shoot()
    {
        GameObject bulletGO = Instantiate(bulletPrefab, transform.position, transform.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.setupBullet(damage, range, transform.position, target);
        }
    }
    //TODO: enemy that spawns enemies
    void UpdateTarget()
    {
        GameObject[] enemies = WaveSpawner.Instance.enemyList.ToArray();    //Dont want to shuffle the main array while something might be using it
        WaveSpawner.Instance.ShuffleArr(enemies);

        foreach (GameObject enemy in enemies)
        {
            if (enemy != gameObject && enemy != null)   //Dont target self
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if(distanceToEnemy <= range)
                {
                    target = enemy.transform;
                    return;
                }
            }
        }
        target = null;
    }
}
