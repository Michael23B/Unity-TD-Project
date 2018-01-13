using UnityEngine;

public class Portals : MonoBehaviour {

    public Transform portal;
    public Transform exitPortal;

    public Portals otherPortal;

    public GameObject useEffect;
    public float cooldown;
    public float timeToReactivate = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time < timeToReactivate) return;

        GameObject effectIns1 = Instantiate(useEffect);
        GameObject effectIns2 = Instantiate(useEffect);

        effectIns1.transform.position = portal.position;
        effectIns2.transform.position = exitPortal.position;

        Destroy(effectIns1, 3f);
        Destroy(effectIns2, 3f);

        timeToReactivate = Time.time + cooldown;
        otherPortal.timeToReactivate = Time.time + cooldown;
        collision.transform.position = exitPortal.position;
    }
}
