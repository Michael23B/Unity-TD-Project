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
    [HideInInspector]
    public bool isRipe = false;
    [HideInInspector]
    public float timeTillRipe = 0f;

    //public GardenNodeUI; //(set in inspector)

    private Color startColor;
    private Renderer rend;

    PlayerStats playerStats;

    [HideInInspector]
    public int nodeID;

    LocalPlayerCommands player;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
        playerStats = PlayerStats.Instance;
    }

    void SelectGardenNode()
    {
        //GardenNodeUI.transform.position = transform.position + positionOffset;
        //if its active, show() it else hide() it
    }

    private void OnMouseDown()
    {
        if (!WaveSpawner.Instance.buildTime) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (plant != null)
        {
            SelectGardenNode();
            return;
        }


        //gardennodeUI has buttons to plant each plant which call CallPlantPlant(Plantblueprint plant); 
    }

    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (plant == null) return;

        if (plant != null && !isRipe) rend.material.color = hoverColorBad;
        else rend.material.color = hoverColor;
    }

    private void OnMouseExit()
    {
        rend.material.color = startColor;
    }
}
