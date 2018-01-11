using UnityEngine;

public enum FetchStat { Damage, FireRate, Range, Cost, ResourceCost, ResourceType, DebuffAmount, DebuffType, ExtraTargets, DebuffDuration, BuildUpTime, AOE}

[System.Serializable]
public class TurretBlueprint {  //TODO: sell amount variable instead of only 50%

    public GameObject prefab;
    public int cost;
    private Turret prefabTurret;

    public GameObject upgradedPrefab;
    public int upgradeCost;
    private Turret upgradedPrefabTurret;

    public int resourceCost;
    public ResourceTypes resourceType;

    [HideInInspector]
    public float turretRange = -1;
    [HideInInspector]
    public float turretUpgradedRange = -1;

    public string displayName;
    public string displayDesc;
    [Space(10)]
    public string overrideFieldDamage = "";
    public string overrideFieldDebuffType = "";

    private Spawn prefabSpawn = null;
    private Bullet prefabBullet = null;

    public float GetRange(bool upgraded)
    {
        if (upgraded)
        {
            if (turretUpgradedRange == -1) turretUpgradedRange = upgradedPrefab.GetComponent<Turret>().range;
            return turretUpgradedRange;
        }
        else if (!upgraded)
        {
            if (turretRange == -1) turretRange = prefab.GetComponent<Turret>().range;
            return turretRange;
        }
        return -1;
    }

    public int GetSellAmount(bool upgraded)
    {
        if (upgraded) return (cost + upgradeCost) / 2;
        return cost / 2;
    }

    public string GetStat(FetchStat stat)
    {
        if (prefabTurret == null) prefabTurret = prefab.GetComponent<Turret>();
        string statString = "";
        switch (stat)   //general stats (no type check required)
        {
            case FetchStat.Cost:
                statString += cost;
                break;
            case FetchStat.ResourceCost:
                statString += resourceCost;
                break;
            case FetchStat.ResourceType:
                statString += resourceType;
                break;
            case FetchStat.FireRate:
                statString += prefabTurret.baseFireRate;
                break;
        }
        if (prefabTurret.useLaser)  //stats for laser types
        {
            switch (stat)
            {
                case FetchStat.Damage:
                    statString += prefabTurret.damageOverTime;
                    break;
                case FetchStat.Range:
                    statString += GetRange(false);
                    break;
                case FetchStat.DebuffType:
                    if (prefabTurret.type == DebuffType.LaserSlow) statString += "Slow";    //TODO: differentiate these with some variable or a new debufftype
                    else if (prefabTurret.type == DebuffType.Freeze) statString += "Stun";
                    else statString += prefabTurret.type;
                    break;
                case FetchStat.DebuffAmount:
                    statString += prefabTurret.debuffAmount;
                    break;
                case FetchStat.DebuffDuration:
                    statString += prefabTurret.debuffDuration;
                    break;
                case FetchStat.ExtraTargets:
                    statString += prefabTurret.extraTargetNumber;
                    break;
            }
        }

        else if (prefabTurret.useSpawner)   //stats for spawner types
        {
            if (prefabSpawn == null) prefabSpawn = prefabTurret.spawnPrefab.GetComponent<Spawn>();

            switch (stat)
            {
                case FetchStat.Range:
                    statString += prefabSpawn.explosionRadius;
                    break;
                case FetchStat.DebuffType:
                    if (prefabSpawn.debuffs.Length != 0) statString += prefabSpawn.debuffs[0].type;
                    else if (prefabSpawn.buffs.Length != 0) statString += prefabSpawn.buffs[0].type;
                    break;
                case FetchStat.DebuffAmount:
                    if (prefabSpawn.debuffs.Length != 0) statString += prefabSpawn.debuffs[0].amount;
                    else if (prefabSpawn.buffs.Length != 0) statString += prefabSpawn.buffs[0].amount;
                    break;
                case FetchStat.DebuffDuration:
                    if (prefabSpawn.debuffs.Length != 0) statString += prefabSpawn.debuffs[0].time;
                    else if (prefabSpawn.buffs.Length != 0) statString += prefabSpawn.buffs[0].time;
                    break;
                case FetchStat.AOE:
                    if (!prefabSpawn.targetEnemy) statString += "Can hit multiple turrets";
                    else statString += prefabSpawn.explosionRadius;
                    break;
            }
        }
        else   //stats for bullet types
        {
            if (prefabBullet == null) prefabBullet = prefabTurret.bulletPrefab.GetComponent<Bullet>();

            switch (stat)
            {
                case FetchStat.Damage:
                    statString += prefabTurret.baseDamage;
                    break;
                case FetchStat.Range:
                    statString += GetRange(false);
                    break;
                case FetchStat.DebuffType:
                    if (prefabBullet.debuffs.Length > 0) statString += prefabBullet.debuffs[0].type;
                    break;
                case FetchStat.DebuffAmount:
                    if (prefabBullet.debuffs.Length > 0) statString += prefabBullet.debuffs[0].amount;
                    break;
                case FetchStat.DebuffDuration:
                    if (prefabBullet.debuffs.Length > 0) statString += prefabBullet.debuffs[0].time;
                    break;
                case FetchStat.AOE:
                    statString += prefabBullet.explosionRadius;
                    break;
                    //case FetchStat.ExtraTargets:  //TODO: add extra targets for bullets
                    //statString += prefabTurret.extraTargetNumber;
                    //break;
            }
        }
        return statString;
    }
}