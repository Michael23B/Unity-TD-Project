using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
//TODO: Pool enemy health bars, enemies, projects etc.
public class Enemy : MonoBehaviour
{
    static int enemyID = 0;
    [HideInInspector]
    public int ID;

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

    [HideInInspector]
    public float speed; //current speed

    [HideInInspector]
    public bool moveable = true; //is enemy stunned?
    [HideInInspector]
    public List<Debuff> debuffList = new List<Debuff>();

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
    public float debuffScale = 0f;

    //string dateAndTimeVar = System.DateTime.Now.ToString("HH:mm:ss");

    void Start()
    {
        //Assign enemy unique ID
        ID = enemyID;
        enemyID++;

        //stats set up
        baseDamageMulti /= (1 + WaveSpawner.Instance.waveMulti);  //waveMulti starts at 0
        speed = startSpeed;
        minSpeed = startSpeed * 0.2f;
        health = startHealth;
        if (useShield) shield = startShield;
        damageMulti = baseDamageMulti;

        //position adjust
        gameObject.transform.position += new Vector3 (0f, yOffset, 0f);

        //enemyMovement = GetComponent<EnemyMovement>();
    }

    public void TakeDamage(float amount)
    {
        if (health <= 0f) return; //already dead, don't call die() more than once

        if(amount > 0) amount *= damageMulti;   //don't reduce healing

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
        PlayerStats.Instance.money += bounty;

        GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(effect, 5f);

        WaveSpawner.Instance.enemiesAlive--;
        WaveSpawner.Instance.enemyList.Remove(gameObject);
        Destroy(gameObject);
        return;
    }

    public void Kill()
    {
        Die();
    }

    private void OnDestroy()
    {
        //dateAndTimeVar = System.DateTime.Now.ToString("HH:mm:ss");
        //if (Log.isOpen) Log.LogToFile("Enemy " + ID + " destroyed at " + dateAndTimeVar);
        for (int i = debuffList.Count - 1; i >= 0; --i)
        {
            if (debuffList[i].effect != null) Destroy(debuffList[i].effect);
        }
    }
}    //public void SubtractLives() {}
     //TODO: Perform life subtract here instead of in GameMaster for performance