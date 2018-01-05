using UnityEngine;
using UnityEngine.EventSystems;

public class GardenNode : MonoBehaviour {
    public Vector3 positionOffset;
    public Color hoverColor;
    public Color hoverColorBad;
    [HideInInspector]
    public GameObject plant;
    [HideInInspector]
    public TurretBlueprint plantBlueprint; //change to plant blueprint
    public float harvestTime = 10f;
    [HideInInspector]
    public float timeTillRipe = 0f;

    //public GardenNodeUI; //(set in inspector)

    private Color startColor;
    private Renderer rend;

    PlayerStats playerStats;

    [HideInInspector]
    public int nodeID;

    LocalPlayerCommands player;
    public GardenNodeUI UI;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
        playerStats = PlayerStats.Instance;
    }

    void SelectGardenNode()
    {
        UI.transform.position = transform.position + positionOffset;
        UI.SetTarget(this);
        //if its active, show() it else hide() it
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        SelectGardenNode();

        //gardennodeUI has buttons to plant each plant which call CallPlantPlant(Plantblueprint plant); 
    }

    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (plant != null && !CheckRipe()) rend.material.color = hoverColorBad;
        else rend.material.color = hoverColor;
    }

    private void OnMouseExit()
    {
        rend.material.color = startColor;
    }

    public void Plant(int index)
    {
        plant = Instantiate(ResourceSpawner.Instance.GetResource(index), transform.position - positionOffset, Quaternion.identity);
        timeTillRipe = Time.time + harvestTime; //todo plants blueprint harvest time
    }

    public void Harvest()
    {
        if (plant == null) return;

        timeTillRipe = Time.time + harvestTime;
        Destroy(plant); //todo plant.harvest
    }

    public bool CheckRipe()
    {
        return (Time.time >= timeTillRipe);
    }
}
