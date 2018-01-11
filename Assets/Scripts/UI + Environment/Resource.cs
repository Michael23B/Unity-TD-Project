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

    public int hitsToDestroy = 7;
    public int hitEffectLife = 2;

    [HideInInspector]
    public int ID;

    [Tooltip("For sending messages about additional effect events")]
    public Transform textOrigin;

    private void Start()
    {
        ID = ResourceSpawner.Instance.indexOfID;
        ResourceSpawner.Instance.indexOfID++;

        ResourceSpawner.Instance.resources.Add(this);
    }

    private void OnMouseDown()
    {
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
        if (money != 0)
        {
            PlayerStats.Instance.money += money;
            BuildManager.Instance.message.PlayMessage("+$" + money, transform, Color.yellow);
        }
        if (stone != 0)
        {
            PlayerStats.Instance.stone += stone;
            BuildManager.Instance.message.PlayMessage("+" + stone, transform, Color.grey);
        }
        if (green != 0)
        {
            PlayerStats.Instance.green += green;
            BuildManager.Instance.message.PlayMessage("+" + green, transform, Color.green);
        }
        if (diamond != 0)
        {
            PlayerStats.Instance.diamond += diamond;
            BuildManager.Instance.message.PlayMessage("+" + diamond, transform, Color.blue);
        }

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
            newGroup[0] = enemyEncounter;                                   //assign new encounter at the beginning of wave
            wave.wave = newGroup;                                           //assign new wave
        }

        BuildManager.Instance.message.PlayMessage("Boss Invading Next Wave", textOrigin, Color.black, 0.5f, 3, 1);
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
