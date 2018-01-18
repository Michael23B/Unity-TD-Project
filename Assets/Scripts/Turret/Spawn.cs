using UnityEngine;

public class Spawn : MonoBehaviour {

    public float explosionRadius = 10f;
    public GameObject spawnEffect;
    public GameObject debuffEffect;
    public float damage = 0f;

    public float countdown = 1.5f;

    [Header("Enemy target? False = Turret target")]
    public bool targetEnemy = true;

    [Header("Debuffs (For enemies)")]
    public Debuff[] debuffs;

    [Header("Buffs (For turrets)")]
    public Debuff[] buffs;

    private void Start()
    {
        GameObject effectins = Instantiate(spawnEffect, transform.position, transform.rotation);
        effectins.transform.SetParent(transform.parent);
        Destroy(effectins, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        while (countdown > 0)
        {
            countdown -= Time.deltaTime;
            return;
        }
        Activate();
    }

    void Activate()
    {
        Explode();
        Destroy(gameObject);
        countdown = Mathf.Infinity; //Activate should only activate once 
    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders)
        {
            if (targetEnemy)
            {
                if (collider.tag == "Enemy")
                {
                    Debuff(collider.transform);
                }
            }
            else if(!targetEnemy)
            {
                if (collider.tag == "Turret")
                {
                    Debuff(collider.transform);
                }
            }
        }
    }

    void Debuff(Transform target)
    {
        if (targetEnemy)
        {
            Enemy e = target.GetComponent<Enemy>();
            if (e != null)
            {
                e.TakeDamage(damage);
                foreach (Debuff debuff in debuffs)
                {
                    BuffHelper.AddDebuff(e, debuff);
                }
            }
        }
        else if(!targetEnemy)
        {
            Turret t = target.GetComponent<Turret>();
            if (t != null)
            {
                foreach (Debuff buff in buffs)
                {
                    BuffHelper.AddDebuff(t, buff);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
