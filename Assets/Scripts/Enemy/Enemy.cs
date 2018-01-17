using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

//TODO: Pool enemy health bars, enemies, projects etc.
//TODO: add snitch enemy thats very fast and tanky but drops lots of gold. does no core damage, so its just a bonus.
public class Enemy : MonoBehaviour
{
    static int enemyID = 0;
    [HideInInspector]
    public int ID;

    [HideInInspector]
    public int GID; //set by the player that spawned the original enemy

    [Header("Stats")]
    public float startSpeed = 10f;
    [HideInInspector]
    public float minSpeed;  //min speed = 20% of starting speed

    public float startHealth = 100f;    //used for health bar %

    [HideInInspector]
    public float health;    //health = start health

    public float startShield = 0f;    //used for health bar %

    [HideInInspector]
    public float shield;    //health = start health

    public float baseDamageMulti = 1f;
    [HideInInspector]
    public float damageMulti;

    public int bounty = 25;

    public bool armored = false;

    public int coreDamage = 1;

    [HideInInspector]
    public float speed; //current speed

    [HideInInspector]
    public bool moveable = true; //is enemy stunned?
    [HideInInspector]
    public List<Debuff> debuffList = new List<Debuff>();
    public Vector3 debuffEffectScale = new Vector3(1f,1f,1f);

    [Header("Setup Fields")]
    public GameObject deathEffect;
    public GameObject emptyPlaceHolder; //can i just use null?

    public Image healthBar;
    public Image healthBarBG; //TODO: One canvas with all health bars instead of serperate canvasii

    public bool useShield = false;
    public Image shieldBar;
    public Image shieldBarBG; //TODO: One canvas with all health bars instead of serperate canvasii

    public EnemyMovement enemyMovement;
    [Space(10)]
    public float yOffset = 0f;

    [HideInInspector]
    public bool ghost = false;

    public int managerID;   //ID to send to other players when the local player spawns an enemy

    public GameObject graphics;

    public float debuffLimitTimer = 0f;
    public float debuffLimitThreshold = 60f;
    [HideInInspector]
    public bool debuffLimitActive = false;

    void Start()
    {
        //Assign enemy unique ID
        if (!ghost)
        {
            ID = enemyID;
            enemyID++;

            //Make a state of self and send ghost to other player
            EnemyState _state = new EnemyState();
            _state.ID = ID;
            _state.movementTarget = enemyMovement.waypointIndex;
            _state.pos = transform.position;

            WaveSpawner.Instance.commands.CmdCreateGhost(WaveSpawner.Instance.playerID, ID, managerID, _state);
        }

        //stats set up
        baseDamageMulti /= (1 + WaveSpawner.Instance.waveMulti);  //waveMulti starts at 0
        speed = startSpeed;
        minSpeed = startSpeed * 0.2f;
        health = startHealth;
        if (useShield) shield = startShield;
        damageMulti = baseDamageMulti;
        bounty = (int)Mathf.Round(bounty * (WaveSpawner.Instance.waveMulti + 1)); //scale money gained with the difficulty of enemies

        //position adjust
        gameObject.transform.position += new Vector3 (0f, yOffset, 0f);

        if (enemyMovement == null) enemyMovement = GetComponent<EnemyMovement>();
    }

    public void TakeDamage(float amount, bool reducedByArmor = false)
    {
        if (ghost) return;
        if (health <= 0f) return; //already dead, don't call die() more than once

        if (reducedByArmor && armored) amount /= 4; //armored enemies reduce non armor piercing damage by by 75%
        if (amount > 0) amount *= damageMulti;   //don't reduce healing

        if (useShield && shield != 0f && amount > 0f)   //don't heal the shield
        {
            shield -= amount;
            if (shield < 0f)
            {
                health += shield;   //take remaining damage from health
                shield = 0f;
            }
        }
        else health -= amount;  //no shield or it's healing

        if (health > startHealth)
        {
            health = startHealth; //healing cap
        }

        if (ghost)  //don't toggle health bars on ghosts
        {
            if (health <= 0f) GhostDie();
            return;
        }

        if(health != startHealth || shield != startShield)
        {
            HealthBarTogle(true);
            healthBar.fillAmount = health / startHealth;
            if (useShield)
            {
                ShieldBarTogle(true);
                shieldBar.fillAmount = shield / startShield;
            }
        } else
        {
            HealthBarTogle(false);
            ShieldBarTogle(false);
        }
        if (health <= 0f) Die();
    }

    public void TakeShieldDamage(float amount)
    {
        if (ghost) return;
        if (!useShield) return;

        if (amount > 0) amount *= damageMulti;   //don't reduce healing

        if (shield != 0f)
        {
            shield -= amount;
            if (shield < 0f) shield = 0f;
        }
        if (health != startHealth || shield != startShield)
        {
            HealthBarTogle(true);
            healthBar.fillAmount = health / startHealth;
            if (useShield)
            {
                ShieldBarTogle(true);
                shieldBar.fillAmount = shield / startShield;
            }
        }
        else
        {
            HealthBarTogle(false);
            ShieldBarTogle(false);
        }
    }

    void HealthBarTogle(bool b)
    {
        healthBarBG.enabled = b;
        healthBar.enabled = b;
    }

    void ShieldBarTogle(bool b)
    {
        if(useShield) //all enemies have health bar but not all have shields
        {
            shieldBarBG.enabled = b;
            shieldBar.enabled = b;
        }
    }

    void Die()//TODO: deal with desync when an enemy dies on client but not server
    {
        if (ghost)
        {
            GhostDie();
            return;
        }
        PlayerStats.Instance.money += bounty;

        GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(effect, 5f);

        WaveSpawner.Instance.enemiesAlive--;
        WaveSpawner.Instance.enemyList.Remove(gameObject);
        WaveSpawner.Instance.commands.CmdKillGhost(WaveSpawner.Instance.playerID, ID);
        Destroy(gameObject);
        return;
    }

    void GhostDie()
    {
        //GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);    //change to some ghost effect
        //Destroy(effect, 5f);

        WaveSpawner.Instance.enemyGhostList.Remove(gameObject);
        EnemyAttack attackComponent = GetComponent<EnemyAttack>();
        if (attackComponent != null) attackComponent.isQuitting = true;  //stops spawning from ghosts

        Destroy(gameObject);
        return;
    }

    public void Kill()
    {
        Die();
    }

    private void OnDestroy()
    {
        for (int i = debuffList.Count - 1; i >= 0; --i)
        {
            if (debuffList[i].effect != null) Destroy(debuffList[i].effect);    //clean up debuff effects that may be in world
        }
    }
}    //public void SubtractLives() {}
     //TODO: Perform life subtract here instead of in GameMaster for performance