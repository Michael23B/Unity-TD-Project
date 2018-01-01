using UnityEngine;

public class Gun : MonoBehaviour {

    public float damage = 25f;
    public float range = 25f;

    public Camera fpsCamera;
    public GameObject graphics;
    public ParticleSystem shootEffect;
    public ParticleSystem shootEffect2;
    public GameObject altShootBuffEffect;

    public void Shoot()
    {
        shootEffect.Play();
        shootEffect.transform.rotation = graphics.transform.rotation;

        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, graphics.transform.rotation * Vector3.forward, out hit, range))
        {
            Enemy enemy = hit.transform.GetComponent<Enemy>();

            if (enemy != null) enemy.TakeDamage(damage);
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
                if (target != null) BuffHelper.AddDebuff(target, DebuffType.AtkSpeed, 5f, 2f, altShootBuffEffect);
            }
        }

    }
}
