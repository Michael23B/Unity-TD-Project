using UnityEngine;

public class BuildManager : MonoBehaviour {

    public static BuildManager Instance;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("Error: More than one build manager in scene!");
            return;
        }
        Instance = this;
    }

    public GameObject buildEffect;
    public GameObject sellEffect;

    private TurretBlueprint turretToBuild;
    private int turretToBuildIndex;
    private Node selectedNode;

    public NodeUI nodeUI;

    public bool CanBuild { get { return turretToBuild != null; } }
    public bool HasMoney { get { return PlayerStats.Instance.money >= turretToBuild.cost; } }
    public bool HasMoneyUpgrade { get { return PlayerStats.Instance.money >= selectedNode.turretBlueprint.upgradeCost; } }

    public void SelectTurretToBuild(TurretBlueprint turret, int index)
    {
        turretToBuild = turret;
        turretToBuildIndex = index;
        DeselectNode();
    }

    public void SelectNode(Node node)
    {
        if (node == selectedNode)
        {
            DeselectNode();
            return;
        }

        selectedNode = node;
        turretToBuild = null;

        nodeUI.SetTarget(node);
    }

    public void DeselectNode()
    {
        selectedNode = null;
        nodeUI.Hide();
    }

    public TurretBlueprint GetTurretToBuild()
    {
        return turretToBuild;
    }

    public int GetTurretToBuildIndex()
    {
        return turretToBuildIndex;
    }
}
