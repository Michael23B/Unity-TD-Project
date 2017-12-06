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
    public float fireCountDown = 0f;

    public bool targetNearest = false;
    //TODO: add prioritze closest to end

    [Header("Use Bullets (Default)")]
    public GameObject bulletPrefab;

    [Header("Use Laser")]
    public bool useLaser = false;

    public float damageOverTime = 20f;
    public float slowAmount = 1.5f; //Change to debuff amount, add debuff type dropdown list (based on debuff class)

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

    public List<Debuff> debuffList = new List<Debuff>();

    public GameObject emptyPlaceHolder; //TODO: this is so stupid honestly

    // Use this for initialization
    void Start()
    {
        fireRate = baseFireRate;
        damage = baseDamage;
        InvokeRepeating("UpdateTarget", 0f, 0.1f);  //Don't really need to update target every frame
    }

    void UpdateTarget()
    {
        if (target != null && !targetNearest)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (distanceToTarget < range)
            {
                return;
            }
        }

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
        else target = null;
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

        if (!useSpawner) LockOnTarget();

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
        BuffHelper.AddDebuff(targetEnemy, DebuffType.LaserSlow, 0.01f, slowAmount);

        if (!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
            impactEffect.Play();
            impactLight.enabled = true;
        }
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.position);

        Vector3 dir = firePoint.position - target.position;

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
