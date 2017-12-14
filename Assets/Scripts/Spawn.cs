using UnityEngine;

//TODO: change this to debuff spawn or something and make a different script for spawning units
public class Spawn : MonoBehaviour {

    public float explosionRadius = 10f;
    public GameObject spawnEffect;
    public GameObject debuffEffect;
    public float amount = 0f;

    public float countdown = 1.5f;
    public float debuffDuration = 1.5f;

    [Header("Enemy target? false = turret target")]
    public bool targetEnemy = true;

    [Header("Debuffs to apply (enemy target)")]
    public bool freeze = false;
    public bool slow = false;
    public bool poison = false; 
    public bool heal = false;
    public bool amplifyDmg = false;
    public bool fear = false;

    [Header("Buffs to apply (turret target)")]
    public bool atkSpeed = false;

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

    void Debuff(Transform target)   //TODO: combine this with the bullet damage function
    {
        if (targetEnemy)
        {
            Enemy e = target.GetComponent<Enemy>();
            GameObject debuffTemp = debuffEffect;
            if (e != null)
            {
                if (slow)
                {
                    BuffHelper.AddDebuff(e, DebuffType.Slow, debuffDuration, amount, debuffTemp);
                    debuffTemp = null;   //if you instantiate an effect, don't stack it
                }
                if (poison)
                {
                    BuffHelper.AddDebuff(e, DebuffType.Poison, debuffDuration, amount, debuffTemp);
                    debuffTemp = null;
                }
                if (freeze)
                {
                    BuffHelper.AddDebuff(e, DebuffType.Freeze, debuffDuration, amount, debuffTemp);
                    debuffTemp = null;
                }
                if (heal)
                {
                    BuffHelper.AddDebuff(e, DebuffType.Heal, debuffDuration, amount, debuffTemp);
                    debuffTemp = null;
                }
                if (amplifyDmg)
                {
                    BuffHelper.AddDebuff(e, DebuffType.AmplifyDmg, debuffDuration, amount, debuffTemp);
                    debuffTemp = null;
                }
                if (fear)
                {
                    BuffHelper.AddDebuff(e, DebuffType.Fear, debuffDuration, amount, debuffTemp);
                    debuffTemp = null;
                }
            }
        }
        else if(!targetEnemy)
        {
            Turret t = target.GetComponent<Turret>();
            if (t != null)
            {
                if (atkSpeed) BuffHelper.AddDebuff(t, DebuffType.AtkSpeed, debuffDuration, amount, debuffEffect);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
