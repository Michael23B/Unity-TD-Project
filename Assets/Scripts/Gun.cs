using UnityEngine;

public class Gun : MonoBehaviour {

    public float damage = 10f;
    public float range = 15f;

    public Camera fpsCamera;
    public ParticleSystem shootEffect;
    public ParticleSystem shootEffect2;
    public GameObject altShootBuffEffect;

    public void Shoot()
    {
        shootEffect.Play();
        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
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
