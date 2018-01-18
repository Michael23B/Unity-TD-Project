using UnityEngine;

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
    public EnemyGroup enemyEncounter = null;
    public bool additionalEffects = false;
    public bool playerTurretSelectEnable = false;
    public bool boss = false;
    public bool gunUpgrade = false;
    public int gunUpgradeAmount = 5;

    public int hitsToDestroy = 7;
    public int hitEffectLife = 2;

    public int ID = -1;

    int miningRange = 40;
    bool inRange = false;

    private void Start()
    {
        if (ID == -1)   //if you dont have a pre-set ID
        {
            ID = ResourceSpawner.Instance.indexOfID;
            ResourceSpawner.Instance.indexOfID++;
        }

        ResourceSpawner.Instance.resources.Add(this);
    }

    private void OnMouseDown()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, miningRange);  //i had a bug getting local player positions so im just doing overlap check instead
        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Player" || collider.tag == "Player2")
            {
                inRange = true;
                break;
            }
            inRange = false;
        }
        //float distanceToTarget = Vector3.Distance(transform.position, WaveSpawner.Instance.localPlayer.transform.position);
        if (!inRange) return;

        if (!WaveSpawner.Instance.gameStarted)
        {
            BuildManager.Instance.message.PlayMessage("Wait until game has started to harvest!", transform, Color.white, 1.25f, 0.5f, 2);
            Destroy(Instantiate(breakingEffect, transform.position, Quaternion.identity), 2f);
            return;
        }
        if (--hitsToDestroy == 0)
        {
            Destroy(Instantiate(hitEffect, transform.position, Quaternion.identity), hitEffectLife);
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
        if (playerTurretSelectEnable)
        {   //enable the players turret select option again
            WaveSpawner.Instance.turretSelect.buttonOpenWindow.gameObject.SetActive(true);
            BuildManager.Instance.message.PlayMessage("TURRET SELECT NOW AVAILABLE!", transform, Color.white, 0.5f, 1f, 2);
        }

        if (enemyEncounter.enemy != null) //add enemy group to next wave as the first enemy group
        {   
            if (WaveSpawner.Instance.nextWaveIndex >= WaveSpawner.Instance.waveMax) return;
            EnemyWave wave = WaveSpawner.Instance.waves[WaveSpawner.Instance.nextWaveIndex];
            if (enemyEncounter != null)
            {
                EnemyGroup[] newGroup = new EnemyGroup[wave.wave.Length + 1];   //make a new group
                wave.wave.CopyTo(newGroup, 1);
                newGroup[0] = new EnemyGroup();
                newGroup[0] = enemyEncounter;                                   //assign new encounter at the beginning of wave
                wave.wave = newGroup;                                           //assign new wave
            }

            if (boss) BuildManager.Instance.message.PlayMessage("BOSS INVADING NEXT WAVE", transform, Color.black, 0.5f, 3, 1);
            else BuildManager.Instance.message.PlayMessage("ENEMIES INVADING NEXT WAVE", transform, Color.red, 1f, 1f, 1);
        }

        if (gunUpgrade) WaveSpawner.Instance.CallGunUpgrade(gunUpgradeAmount);
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
