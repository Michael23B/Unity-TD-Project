using UnityEngine;

[System.Serializable]
public class TurretBlueprint {  //TODO: sell amount variable instead of only 50%

    public GameObject prefab;
    public int cost;

    public GameObject upgradedPrefab;
    public int upgradeCost;

    public float turretRange = -1;
    public float turretUpgradedRange = -1;

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
}