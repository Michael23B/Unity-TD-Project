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
    public Message message;

    public bool CanBuild { get { return turretToBuild != null; } }
    public bool HasMoney { get { return PlayerStats.Instance.money >= turretToBuild.cost; } }
    public bool HasMoneyUpgrade { get { return PlayerStats.Instance.money >= selectedNode.turretBlueprint.upgradeCost; } }

    public bool HasResources() {
        if (turretToBuild.resourceCost == 0) return true;
            switch (turretToBuild.resourceType)
            {
                case ResourceTypes.stone:
                    return PlayerStats.Instance.stone >= turretToBuild.resourceCost;

                case ResourceTypes.diamond:
                    return PlayerStats.Instance.diamond >= turretToBuild.resourceCost;

                case ResourceTypes.green:
                    return PlayerStats.Instance.green >= turretToBuild.resourceCost;
            }
        return false;
    }

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
