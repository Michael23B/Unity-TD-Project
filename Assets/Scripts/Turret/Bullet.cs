﻿using UnityEngine;

public class Bullet : MonoBehaviour {

    private Transform target;

    public float speed = 70f;
    public float explosionRadius = 0f;
    public GameObject impactEffect;
    public GameObject travelEffect;
    private ParticleSystem travelEffectPS;
    private float damage = 50f;
    public bool reducedByArmor = false;

    [Header("Debuffs")]
    public Debuff[] debuffs;

    //Turret setup fields
    private float range = 15f;
    private Vector3 firePoint;

    public void setupBullet(float _damage, float _range, Vector3 _firePoint, Transform _target)
    {
        damage = _damage;
        range = _range;
        firePoint = _firePoint;
        target = _target;
    }

    private void Start()
    {
        if (travelEffect)
        {
            travelEffect = Instantiate(travelEffect);
            travelEffectPS = travelEffect.GetComponent<ParticleSystem>();
        }
    }

    void Update () {
        if (target == null)
        {
            UpdateBulletTarget();
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if(dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        transform.LookAt(target);
        if (travelEffect)
        {
            travelEffect.transform.position = this.transform.position;
        }
	}
    void HitTarget()
    {
        GameObject effectIns = Instantiate(impactEffect, transform.position, transform.rotation);

        Destroy(effectIns, 5f);

        if(explosionRadius > 0 )
        {
            Explode();
        }
        else
        {
            Damage(target);
        }
        
        Destroy(gameObject);
    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider collider in colliders)
        {
            if(collider.tag == "Enemy")
            {
                Damage(collider.transform);
            }
        }
    }

    void Damage(Transform enemy)
    {
        Enemy e = enemy.GetComponent<Enemy>();
        if (e != null)
        {
            foreach (Debuff debuff in debuffs)
            {
                BuffHelper.AddDebuff(e, debuff);
            }
            e.TakeDamage(damage, reducedByArmor);
        }
    }

    void UpdateBulletTarget()
    {
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in WaveSpawner.Instance.enemyList)
        {
            if (enemy != null)
            {
                float distanceToEnemy = Vector3.Distance(firePoint, enemy.transform.position);

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
        }
        else Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    private void OnDestroy()
    {
        if (travelEffect)
        {
            travelEffectPS.Stop();
            Destroy(travelEffect, 1f);
        }
    }
}