using UnityEngine;

public class Bullet : MonoBehaviour {

    private Transform target;

    public float speed = 70f;
    public float explosionRadius = 0f;
    public GameObject impactEffect;
    public GameObject travelEffect;
    private ParticleSystem travelEffectPS;
    private float damage = 50f;

    [Header("Debuffs (0 = inactive)")]
    public float slow = 0f;
    public float poison = 0f;
    public float freeze = 0f;
    public float heal = 0f;
    public float amplifyDmg = 0f;
    public float fear = 0f;
    public float duration = 1f;
    public GameObject debuffEffect;

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

    // Update is called once per frame

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

    void Damage(Transform enemy)    //why did i make 2 functions with slightly different implementations that do the same shit
    {
        Enemy e = enemy.GetComponent<Enemy>();
        GameObject debuffTemp = debuffEffect;
        if (e != null)
        {
            if (slow != 0)
            {
                BuffHelper.AddDebuff(e, DebuffType.Slow, duration, slow, debuffTemp);
                debuffTemp = null;   //if you instantiate an effect, don't stack it
            }
            if (poison != 0)
            {
                BuffHelper.AddDebuff(e, DebuffType.Poison, duration, poison, debuffTemp);
                debuffTemp = null;
            }
            if (freeze != 0)
            {
                BuffHelper.AddDebuff(e, DebuffType.Freeze, duration, freeze, debuffTemp);
                debuffTemp = null;
            }
            if (heal != 0)
            {
                BuffHelper.AddDebuff(e, DebuffType.Heal, duration, heal, debuffTemp);
                debuffTemp = null;
            }
            if (amplifyDmg != 0)
            {
                BuffHelper.AddDebuff(e, DebuffType.AmplifyDmg, duration, amplifyDmg, debuffTemp);
                debuffTemp = null;
            }
            if (fear != 0)
            {
                BuffHelper.AddDebuff(e, DebuffType.Fear, duration, fear, debuffTemp);
                debuffTemp = null;
            }
            e.TakeDamage(damage);
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
            Destroy(travelEffect, 1.5f);
        }
    }
}