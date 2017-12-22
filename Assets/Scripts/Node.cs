using UnityEngine;
using UnityEngine.EventSystems;

//TODO: fix when selling upgraded turret reset the node
public class Node : MonoBehaviour
{
    public Vector3 positionOffset;
    public Color hoverColor;
    public Color hoverColorBad;

    [HideInInspector]
    public GameObject turret;
    [HideInInspector]
    public TurretBlueprint turretBlueprint;
    [HideInInspector]
    public bool isUpgraded = false;

    private Color startColor;
    private Renderer rend;

    BuildManager buildManager;
    PlayerStats playerStats;

    [HideInInspector]
    public int nodeID;

    LocalPlayerCommands player;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
        buildManager = BuildManager.Instance;
        playerStats = PlayerStats.Instance;

        //buildManager = FindObjectOfType<BuildManager>();
        //playerStats = FindObjectOfType<PlayerStats>();
    }

    public Vector3 GetBuildPosition()
    {
        return transform.position + positionOffset;
    }

    private void OnMouseDown()
    {
        if (!WaveSpawner.Instance.buildTime) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (turret != null)
        {
            buildManager.SelectNode(this);
            return;
        }

        if (!buildManager.CanBuild) return;

        CallBuildTurret();
    }

    public void CallBuildTurret(bool upgrading=false)
    {
        if(upgrading)
        {
            if (!buildManager.HasMoneyUpgrade)
            {
                buildManager.message.PlayMessage("Not Enough Money!", transform);
                return;
            }
            playerStats.money -= turretBlueprint.upgradeCost;
        }
        else
        {
            if (!buildManager.HasMoney)
            {
                buildManager.message.PlayMessage("Not Enough Money!", transform);
                return;
            }
            if (!buildManager.HasResources())
            {
                buildManager.message.PlayMessage("Not Enough Resources!", transform);
                return;
            }

            switch (buildManager.GetTurretToBuild().resourceType)
            {
                case ResourceTypes.stone:
                    PlayerStats.Instance.stone -= buildManager.GetTurretToBuild().resourceCost;
                    break;
                case ResourceTypes.diamond:
                    PlayerStats.Instance.diamond -= buildManager.GetTurretToBuild().resourceCost;
                    break;
                case ResourceTypes.green:
                    PlayerStats.Instance.green -= buildManager.GetTurretToBuild().resourceCost;
                    break;
            }
            playerStats.money -= buildManager.GetTurretToBuild().cost;
        }

        if (player == null) player = FindObjectOfType<LocalPlayerCommands>();

        int _tID = buildManager.GetTurretToBuildIndex();
        player.CmdBuildTurret(nodeID, _tID, upgrading);
    }

    public void CallSellTurret()
    {
        if (player == null) player = FindObjectOfType<LocalPlayerCommands>();

        playerStats.money += turretBlueprint.GetSellAmount(isUpgraded);
        player.CmdSellTurret(nodeID);
    }

    #region turret build/sell functions (local)
    public void BuildTurret(TurretBlueprint blueprint)
    {
        GameObject _turret = Instantiate(blueprint.prefab, GetBuildPosition(), Quaternion.identity);
        turret = _turret;

        turretBlueprint = blueprint;

        GameObject effect = Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);

        Debug.Log("Turret Built!");
    }

    public void UpgradeTurret()
    {
        Destroy(turret);    //destroy old turret

        GameObject _turret = Instantiate(turretBlueprint.upgradedPrefab, GetBuildPosition(), Quaternion.identity);
        turret = _turret;

        GameObject effect = Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);
        //TODO: Change build effect from buildManager to the blueprint (makes more sense)

        isUpgraded = true;
        //TODO: Bool -> int for upgrade levels, array of upgraded turrets

        Debug.Log("Turret Upgraded!");
    }

    public void SellTurret()
    {
        GameObject effect = Instantiate(buildManager.sellEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);

        Destroy(turret);
        turretBlueprint = null;

        isUpgraded = false;
    }
    #endregion

    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (!buildManager.CanBuild) return;

        if (turret != null) rend.material.color = hoverColorBad;
        else if (!buildManager.HasMoney) rend.material.color = hoverColorBad;
        else rend.material.color = hoverColor;
    }

    private void OnMouseExit()
    {
        rend.material.color = startColor;
    }
}