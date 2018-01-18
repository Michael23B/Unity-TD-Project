using UnityEngine;
using UnityEngine.EventSystems;

public class GardenNode : MonoBehaviour {
    public Vector3 positionOffset;
    public Color hoverColor;
    public Color hoverColorBad;
    [HideInInspector]
    public GameObject plant;
    [HideInInspector]
    public Plant plantComponent;
    public float harvestTime = 10f;
    [HideInInspector]
    public float timeTillRipe = 0f;

    //public GardenNodeUI; //(set in inspector)

    private Color startColor;
    private Renderer rend;

    [HideInInspector]
    public int nodeID;

    LocalPlayerCommands player;
    public GardenNodeUI UI;


    private void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
    }

    void SelectGardenNode()
    {
        UI.transform.position = transform.position + positionOffset;
        UI.SetTarget(this);
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        SelectGardenNode();
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

    public void Plant(GameObject plant)
    {
        plant = Instantiate(plant, transform.position + positionOffset, Quaternion.identity);
        plantComponent = plant.GetComponent<Plant>();
        timeTillRipe = Time.time + plantComponent.harvestTime;
        plantComponent.node = this;
    }

    public void CallPlant(int plantID)
    {
        WaveSpawner.Instance.commands.CmdPlant(nodeID, plantID);
    }

    public void CallHarvest()
    {
        WaveSpawner.Instance.commands.CmdHarvest(nodeID);
    }

    public bool CheckRipe()
    {
        return (Time.time >= timeTillRipe);
    }
}
