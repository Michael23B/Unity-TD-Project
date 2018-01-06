using UnityEngine;
using UnityEngine.EventSystems;

public enum PlantTypes { tomato, carrot, fish, grass }

public class Plant : MonoBehaviour {

    [SerializeField]
    GameObject hitEffect;
    [SerializeField]
    GameObject onCooldownHitEffect;

    [Header("Resources to give")]
    public int money;
    public int stone, green, diamond;

    public int timesToReGrow = 3;
    public int harvestTime = 15;

    public GardenNode node;

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
    }
    
    public void Harvest()
    {
        if (!node.CheckRipe())
        {
            Destroy(Instantiate(onCooldownHitEffect, transform.position, Quaternion.identity), 2f);
        }
        else
        {
            Destroy(Instantiate(hitEffect, transform.position, Quaternion.identity), 2f);
            CheckResources();
            Destroy(gameObject);
        }
    }
}
