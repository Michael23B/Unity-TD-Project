﻿using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    public Vector3 positionOffset;
    public Color hoverColor;
    public Color hoverColorBad;
    [Tooltip("0 for p1, 1 for p2")]
    public int owner;
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
            if (isUpgraded)
            {
                BuildManager.Instance.message.PlayMessage("Already Upgraded!", transform, Color.red);
                return;
            }
            if (!buildManager.HasMoneyUpgrade)
            {
                BuildManager.Instance.message.PlayMessage("Not Enough Money!", transform, Color.red);
                return;
            }
            playerStats.money -= turretBlueprint.upgradeCost;
        }
        else
        {
            if (!buildManager.HasMoney)
            {
                BuildManager.Instance.message.PlayMessage("Not Enough Money!", transform, Color.red);
                return;
            }
            if (!buildManager.HasResources())
            {
                BuildManager.Instance.message.PlayMessage("Not Enough Resources!", transform, Color.red);
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

        if (turretBlueprint == null)
        {
            BuildManager.Instance.message.PlayMessage("No Turret To Sell!", transform, Color.red);
            return;
        }

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

        turret.GetComponent<Turret>().owner = owner;
    }

    public void UpgradeTurret()
    {
        Destroy(turret);    //destroy old turret

        GameObject _turret = Instantiate(turretBlueprint.upgradedPrefab, GetBuildPosition(), Quaternion.identity);
        turret = _turret;

        GameObject effect = Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);
        //TODO: Change build effect from buildManager to the blueprint (for different effects for each turret)

        isUpgraded = true;
        //TODO: Bool -> int for upgrade levels, array of upgraded turrets

        turret.GetComponent<Turret>().owner = owner;
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