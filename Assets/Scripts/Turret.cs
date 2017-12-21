using UnityEngine;
using System.Collections.Generic;

public class Turret : MonoBehaviour
{
    private Enemy targetEnemy;
    private Transform target;

    [Header("General")]
    public float range = 15f;
    public float baseFireRate = 1f;
    public float baseDamage = 50f;
    [HideInInspector]
    public float damage;
    [HideInInspector]
    public float fireRate;
    private float fireCountDown = 0f;

    public bool targetNearest = false;
    //TODO: add prioritze closest to end

    [Header("Use Bullets (Default)")]
    public GameObject bulletPrefab;

    [Header("Use Laser")]
    public bool useLaser = false;
    public float damageOverTime = 20f;
    [Header("Laser debuffs")]
    public float debuffAmount = 0.15f;
    public DebuffType type = DebuffType.LaserSlow;

    public int extraTargetNumber = 0;
    public float extraTargetFindRange = 5f;
    private Enemy[] targetEnemies;
    private Transform[] targets;//set number of targets for multi

    [Space(10)]

    public float buildUpTime = 0f;  //should it take time before applying debuff
    private float currentBuildUp = 0f;
    public float debuffDuration = 0f;
    public GameObject debuffEffect;

    [Space(10)]

    public LineRenderer lineRenderer;
    public ParticleSystem impactEffect;
    public Light impactLight;

    [Header("Use Spawner")]
    public bool useSpawner = false;
    public bool noTarget = false;   //Should this always spawn or wait for enemies?

    public GameObject spawnPrefab;

    [Header("Unity Setup Fields")]
    public string enemyTag = "Enemy";
    public bool hasAnimation = false;

    public Transform PartToRotate;
    public Transform firePoint;

    public float turnSpeed = 10f;

    public bool childTurret = false;
    public Turret parentTurret;

    public List<Debuff> debuffList = new List<Debuff>();

    public GameObject emptyPlaceHolder; //TODO: this is so stupid honestly

    // Use this for initialization
    void Start()
    {
        fireRate = baseFireRate;
        damage = baseDamage;
        InvokeRepeating("UpdateTarget", 0f, 0.1f);  //Don't really need to update target every frame

        targetEnemies = new Enemy[extraTargetNumber+1];
        targets = new Transform[extraTargetNumber+1];
    }

    void UpdateTarget()
    {
        if (childTurret)
        {
            target = parentTurret.target;
            return;
        }
        if (target != null && !targetNearest)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (distanceToTarget < range)
            {
                return;
            }
        }
        //currentBuildUp = 0f;    //resets build up if target is CHANGED or LOST
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        if (WaveSpawner.Instance.enemyList.Count == 0) return;
        foreach (GameObject enemy in WaveSpawner.Instance.enemyList)
        {
            if (enemy != null)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy;
                }
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
            targetEnemy = target.GetComponent<Enemy>();
        }
        else
        {
            target = null;
            currentBuildUp = 0f;    //resets build up only if target is LOST
        }
    }

    void Update()
    {
        BuffHelper.ResetDebuffs(this);
        BuffHelper.CheckDebuffs(this);
        if (hasAnimation) Animate();

        fireCountDown -= Time.deltaTime;

        if (target == null)
        {
            if (useSpawner && noTarget && fireCountDown <= 0)
            {
                Spawn();
                fireCountDown = 1f / fireRate;
                return;
            }
            if (useLaser)
            {
                if (lineRenderer.enabled)
                {
                    lineRenderer.enabled = false;
                    impactEffect.Stop();
                    impactLight.enabled = false;
                }
            }
            return;
        }

        if (!useSpawner && !childTurret) LockOnTarget();

        if (useLaser) Laser();
        else
        {
            if (fireCountDown <= 0)
            {
                if (useSpawner) Spawn();
                else Shoot();
                fireCountDown = 1f / fireRate;
            }
        }
    }

    Transform newTarget(Transform origin, float _range, Transform[] ignoreTargets)  //returns a new target (closest) within range from an origin, ignoring any requested targets
    {
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        bool ignoredTarget = false;

        if (WaveSpawner.Instance.enemyList.Count == 0) return null;
        foreach (GameObject enemy in WaveSpawner.Instance.enemyList)
        {
            if (enemy != null)
            {
                foreach (Transform ignore in ignoreTargets)
                {
                    if (enemy.transform == ignore)
                    {
                        ignoredTarget = true;
                        break;
                    }
                }
                if (ignoredTarget)  //skip checking this enemy if the target should be ignored
                {
                    ignoredTarget = false;
                    continue;
                }

                float distanceToEnemy = Vector3.Distance(origin.position, enemy.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy;
                }
            }
        }

        if (nearestEnemy != null && shortestDistance <= _range)
        {
            return nearestEnemy.transform;
        }
        else return null;
    }

    void Animate()
    {
        PartToRotate.transform.Rotate(1f, -1f, 1f); //TODO: actually animate it
    }

    void LockOnTarget()
    {
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(PartToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        PartToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    void Laser()
    {
        targetEnemy.TakeDamage(damageOverTime * Time.deltaTime * fireRate);
        //for multitarget
        bool debuffApplied = false; //if a debuff is applied, apply it to any other targets needed
        bool resetTargets = false;  //if the mulitargets are still valid dont update the array (targets[])

        if (buildUpTime != 0)
        {
            if (currentBuildUp >= buildUpTime)
            {
                BuffHelper.AddDebuff(targetEnemy, type, debuffDuration, debuffAmount, debuffEffect);
                currentBuildUp = 0.001f;
                debuffApplied = true;
            }
            else currentBuildUp += Time.deltaTime * fireRate;
        }
        else if (debuffAmount != 0) BuffHelper.AddDebuff(targetEnemy, type, 0.01f, debuffAmount, null);   //if buildUpTime is 0 just apply it for the minimum time


        if (!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
            impactEffect.Play();
            impactLight.enabled = true;
        }
        //TODO: animate the laser with perlin noise or some animation
        //lineRenderer.startWidth = Random.Range(0, 2);
        //lineRenderer.endWidth = Random.Range(0, 2);
        //doesnt update the random?
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.position);
        #region multi-target
        if (extraTargetNumber != 0) //TODO:add check if any of the targets[] are out of range before resetting the array again
                                    //TODO: visual effect on all enemies as well
        {
            if (target == null) return;
            if (targets[0] == null)
            {
                resetTargets = true;
            }
            else
            {
                for (int i = 1; i < targets.Length; ++i)
                {
                    if (targets[i] == null)
                    {
                        resetTargets = true;
                        break;
                    }
                    if (Vector3.Distance(targets[i].position, targets[i - 1].position) > extraTargetFindRange)
                    {
                        resetTargets = true;
                        break;
                    }
                }
            }

            if (resetTargets)
            {
                for (int i = 0; i < targets.Length; ++i) targets[i] = null; //reset targets so that the current targets aren't ignored
                for (int i = 0; i < targetEnemies.Length; ++i) targetEnemies[i] = null;

                int currentTargets = 0;
                targets[0] = target.transform;

                //excluding 0 (initial target) find extra targets in range
                for (int i = 1; i < targets.Length; ++i)
                {
                    targets[i] = newTarget(targets[i - 1], extraTargetFindRange, targets);    //find a new target within range, excluding things already targeted
                    if (targets[i] == null) break;
                    currentTargets++;
                }
                //linerender update
                lineRenderer.positionCount = currentTargets + 2;    //position 0 and 1 are already used on the starting position and the initial enemy
            }

            for (int i = 1; i < lineRenderer.positionCount - 1; ++i)    //position count is no. of extra targets + initial two points (at turret and first enemy
            {                                                           //position count  - 1 means points from first enemy to the last enemy
                if (targets[i] == null) break;
                lineRenderer.SetPosition(i + 1, targets[i].position);
            }
            //lineRenderer.positionCount = //number of targets

            //make enemies take damage (not initial target (already took damage))
            for (int i = 1; i < targets.Length; ++i)
            {
                if (targets[i] == null) break;
                targetEnemies[i] = targets[i].GetComponent<Enemy>();
                targetEnemies[i].TakeDamage(damageOverTime * Time.deltaTime * fireRate);

                if (debuffApplied && debuffDuration != 0)  //don't keep track of build up on all enemies just apply it to all when one is affected
                {
                    BuffHelper.AddDebuff(targetEnemies[i], type, debuffDuration, debuffAmount, debuffEffect);
                }
                else if (debuffDuration == 0)
                {
                    if (debuffAmount != 0) BuffHelper.AddDebuff(targetEnemy, type, 0.01f, debuffAmount, null);
                }
            }
        }
        else lineRenderer.positionCount = 2;
        #endregion
        Vector3 dir = firePoint.position - target.position;
        //TODO: add effect to all extra tagets
        impactEffect.transform.position = target.position + dir.normalized;
        //TODO: only works because enemy radius is 1 (normalized = 1)

        impactEffect.transform.rotation = Quaternion.LookRotation(dir);
    }

    void Shoot()
    {
        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.setupBullet(damage, range, transform.position, target);
        }
    }

    void Spawn()
    {
        Instantiate(spawnPrefab, firePoint.position, firePoint.rotation);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
