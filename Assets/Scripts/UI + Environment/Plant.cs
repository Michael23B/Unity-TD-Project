using UnityEngine;
//TODO: stop from counting down in the initial build time but not after then
//TODO: expensive plant that gives a life after a long growing time
public enum PlantTypes { tomato, carrot, fish, cactus, atmosphere }

public class Plant : MonoBehaviour {

    [SerializeField]
    GameObject hitEffect;
    [SerializeField]
    GameObject onCooldownHitEffect;
    [SerializeField]
    GameObject growEffect;
    [SerializeField]
    GameObject baseModel;
    [SerializeField]
    GameObject ripeModel;

    public int cost = 5;

    [Header("Resources to give")]
    public int money;
    public int stone, green, diamond, lives;

    public float harvestTime = 35f;
    public string displayName;
    public string info;
    public Color displayColor;

    [HideInInspector]
    public GardenNode node;

    private bool destroyed = false; //if both players attempt to take this resource at the same time

    private void Start()
    {
        baseModel.SetActive(true);
        ripeModel.SetActive(false);
        Invoke("Grow", harvestTime);
    }

    private void OnMouseDown()
    {
        if (node == null) return;
        Harvest();
    }

    void CheckResources()
    {
        if (money != 0) PlayerStats.Instance.money += money;
        if (stone != 0) PlayerStats.Instance.stone += stone;
        if (green != 0) PlayerStats.Instance.green += green;
        if (diamond != 0) PlayerStats.Instance.diamond += diamond;
        if (lives != 0) WaveSpawner.Instance.commands.CmdReduceLives(-lives);
    }

    public void Harvest(bool calledFromClient = false)
    {
        if (destroyed) return;  //can be called twice if both players try to take the same thing
        if (calledFromClient)   //if the other player took this resource, destroy it and dont award resources to this player
        {
            Destroy(Instantiate(hitEffect, transform.position, Quaternion.identity), 2f);
            destroyed = true;
            Destroy(gameObject);
            return;
        }

        if (!node.CheckRipe())
        {
            Destroy(Instantiate(onCooldownHitEffect, transform.position, Quaternion.identity), 2f);
        }
        else
        {
            Destroy(Instantiate(hitEffect, transform.position, Quaternion.identity), 2f);
            CheckResources();

            WaveSpawner.Instance.commands.CmdHarvest(node.nodeID);

            Destroy(gameObject);
        }
    }

    void Grow()
    {
        if (node == null) return;
        Destroy(Instantiate(growEffect, transform.position, Quaternion.identity), 2f);

        baseModel.SetActive(false);
        ripeModel.SetActive(true);
    }
}
