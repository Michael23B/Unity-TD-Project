using UnityEngine;
using UnityEngine.SceneManagement;

public class Gun : MonoBehaviour {

    public float startDamage = 10f;
    public float damage = 10f;
    public float range = 25f;

    public Camera fpsCamera;
    public GameObject graphics;
    public ParticleSystem shootEffect;
    public ParticleSystem shootEffect2;
    public GameObject altShootBuffEffect;

    private void Start()
    {
        damage = startDamage;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        damage = startDamage;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Shoot()
    {
        shootEffect.Play();
        shootEffect.transform.rotation = graphics.transform.rotation;

        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, graphics.transform.rotation * Vector3.forward, out hit, range))
        {
            Enemy enemy = hit.transform.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);   //if (!enemy.ghost) //send the ghost damage to the other client. prefer to just fire on the client side and trust the sync is enough
                //else WaveSpawner.Instance.commands.CmdDamageEnemy(WaveSpawner.Instance.playerID, enemy.GID, damage);
            }
        }
    }

    public void AltShoot()
    {
        shootEffect2.Play();

        Collider[] colliders = Physics.OverlapSphere(transform.position, range / 3);

        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Turret")
            {
                Turret target = collider.transform.GetComponent<Turret>();
                if (target != null) BuffHelper.AddDebuff(target, DebuffType.AtkSpeed, 5f, 1f, altShootBuffEffect);
            }
        }
    }
}
