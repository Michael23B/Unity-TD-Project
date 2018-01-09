﻿using UnityEngine;

public enum ResourceTypes { stone, green, diamond }

public class Resource : MonoBehaviour {

    [SerializeField]
    GameObject hitEffect;
    [SerializeField]
    GameObject breakingEffect;

    [Header("Resources to give")]
    public int money;
    public int stone, green, diamond;

    [Header("Additional effects")]
    public GameObject enemyEncounter = null;
    public bool additionalEffects = false;

    public int hitsToDestroy = 7;

    [HideInInspector]
    public int ID;

    static int indexOfID;

    private void Start()
    {
        ID = indexOfID;
        ++indexOfID;

        ResourceSpawner.Instance.resources.Add(this);
    }

    private void OnMouseDown()
    {
        if (--hitsToDestroy == 0)
        {
            Destroy(Instantiate(hitEffect, transform.position, Quaternion.identity), 2f);
            TakeResources();
            Destroy(gameObject);
            return;
        }
        Destroy(Instantiate(breakingEffect, transform.position, Quaternion.identity), 2f);
    }

    void TakeResources()
    {
        if (money != 0) PlayerStats.Instance.money += money;
        if (stone != 0) PlayerStats.Instance.stone += stone;
        if (green != 0) PlayerStats.Instance.green += green;
        if (diamond != 0) PlayerStats.Instance.diamond += diamond;

        if (additionalEffects) CheckEffects();

        WaveSpawner.Instance.commands.CmdDestroyResource(ID);
    }

    void CheckEffects()
    {
        EnemyWave wave = WaveSpawner.Instance.waves[WaveSpawner.Instance.nextWaveIndex];
        if (enemyEncounter != null) {
            EnemyGroup[] newGroup = new EnemyGroup[wave.wave.Length + 1];   //make a new group
            wave.wave.CopyTo(newGroup, 1);
            newGroup[0] = new EnemyGroup();
            newGroup[0].enemy = enemyEncounter;
            newGroup[0].count = 1;
            newGroup[0].spawnRate = 1;
            newGroup[0].waitTime = 5;

            wave.wave = newGroup;
        }
    }

    public void PlayHitEffect()
    {
        Destroy(Instantiate(hitEffect, transform.position, Quaternion.identity), 2f);
    }

    private void OnDestroy()
    {
        ResourceSpawner.Instance.resources.Remove(this);
    }
}
