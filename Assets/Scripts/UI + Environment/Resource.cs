using UnityEngine;

public enum ResourceTypes { stone, green, diamond }

public class Resource : MonoBehaviour {

    [SerializeField]
    GameObject hitEffect;
    [SerializeField]
    GameObject breakingEffect;

    [Header("Resources to give")]
    public int money;
    public int stone, green, diamond;

    public int hitsToDestroy = 7;

    [HideInInspector]
    public int ID;

    static int indexOfID;

    private void Start()
    {
        ID = indexOfID;
        ++indexOfID;
    }

    private void OnMouseDown()
    {
        if (--hitsToDestroy == 0)
        {
            Destroy(Instantiate(hitEffect, transform.position, Quaternion.identity), 2f);
            TakeResources();
            Destroy(gameObject);
            return;
        }
        Destroy(Instantiate(breakingEffect, transform.position, Quaternion.identity), 2f);
    }

    void TakeResources()
    {
        if (money != 0) PlayerStats.Instance.money += money;
        if (stone != 0) PlayerStats.Instance.stone += stone;
        if (green != 0) PlayerStats.Instance.green += green;
        if (diamond != 0) PlayerStats.Instance.diamond += diamond;

        WaveSpawner.Instance.commands.CmdDestroyResource(ID);
    }
}
