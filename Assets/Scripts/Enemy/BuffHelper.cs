using UnityEngine;

//Definitions for buffs and methods for adding and stacking etc.

public enum DebuffType { None, LaserSlow, Freeze, Fear, Poison, AtkSpeed, Heal, Slow, AmplifyDmg, ShieldBreak }    //nostun (if stunned for too long become immune), silence (disable enemyattack)

[System.Serializable]
public class Debuff
{
    public Debuff(DebuffType _type, float _time, float _amount, GameObject _effect) //add bool for buffs or debuffs
    {
        type = _type;
        time = _time;
        amount = _amount;
        effect = _effect;
    }

    public DebuffType type;
    public float time;
    public float amount;
    public GameObject effect;
}

//TODO: debuff size scale could work but it's way off right now because enemy scales arent uniform
//effectIns.transform.localScale = e.transform.localScale / 4;
//TODO:change functions to template instead of defining two of each
public static class BuffHelper {
    //
    //Enemy functions
    //
    public static void AddDebuff(Enemy e, DebuffType _type, float _time, float _amount, GameObject _effect = null)
    {
        if (_type == DebuffType.None) Debug.Log("Empty debuff called"); //oh oh
        if (e.shield > 0 && _type != DebuffType.Heal && _type != DebuffType.ShieldBreak)   //Don't debuff shielded enemies, Negative value debuffs(buffs) and heals go through (and shieldbreak)
        {
            if (_amount >= 0f) return;
        }
        if (isStackingDebuff(_type))                                //If the type can be stacked
        {
            for (int i = e.debuffList.Count - 1; i >= 0; --i)       //Check each current debuff
            {
                if (e.debuffList[i].type == _type)                  //And if it's already in the list
                {
                    if (_effect != null)                            //If it has an effect..
                    {
                        if (e.debuffList[i].effect.CompareTag(_effect.tag)) //check if they are the same, if so just update the values for the debuff
                        {
                            CalcDebuffSimple(ref e.debuffList[i].time, _time, ref e.debuffList[i].amount, _amount);
                            return;
                        }
                                                                    //If the effects are different, make a seperate debuff with the new effect
                    }
                    else
                    {                                               //If there is no effect to be added, just update the debuff values
                        CalcDebuffSimple(ref e.debuffList[i].time, _time, ref e.debuffList[i].amount, _amount);
                        return;
                    }
                }
            }
        }
        //if debuff doesn't already exist or it can't be stacked, add it and if it has an effect instantiate that
        if (_effect != null)
        {
            GameObject effectIns = GameObject.Instantiate(_effect, e.transform.position, Quaternion.Euler(0, 0, 0)); //Don't want the effect rotating with the enemy i believe
            effectIns.transform.localScale = e.debuffEffectScale;
            effectIns.transform.SetParent(e.transform);
            e.debuffList.Add(new Debuff(_type, _time, _amount, effectIns));

        }
        else
        {
            GameObject effectIns = GameObject.Instantiate(e.emptyPlaceHolder, e.transform.position, Quaternion.Euler(0, 0, 0));

            e.debuffList.Add(new Debuff(_type, _time, _amount, effectIns));
        }
    }

    public static void AddDebuff(Enemy e, Debuff d)
    {
        AddDebuff(e, d.type, d.time, d.amount, d.effect);
    }

    static bool isStackingDebuff(DebuffType _type)
    {
        if (_type == DebuffType.LaserSlow || _type == DebuffType.Freeze || _type == DebuffType.Fear) return true;
        else return false;
    }

    static void CalcDebuffSimple(ref float currentTime, float newTime, ref float currentAmount, float newAmount) //Used for stacking types
    {
        if (currentTime < newTime) currentTime = newTime;
        currentAmount += newAmount;
    }
    
    public static void CheckDebuffs(Enemy e) //Calls BuffHelper on every debuff in this enemies list
    {
        if (e.debuffList.Count == 0) return;
        //e.debuffList.Sort(delegate(Debuff d1, Debuff d2) { return d1.time.CompareTo(d2.time); }); //sorts debuff list by time remaining (low->high) so that for example if one stun makes moveable true (by ending) the remaining stun can set it back to false
        for (int i = e.debuffList.Count - 1; i >= 0; --i)
        {
            switch (e.debuffList[i].type)
            {
                case DebuffType.LaserSlow:
                    BuffHelper.LaserSlow(e, i);
                    break;
                case DebuffType.Freeze:
                    BuffHelper.Freeze(e, i);
                    break;
                case DebuffType.Fear:
                    BuffHelper.Fear(e, i);
                    break;
                case DebuffType.Poison:
                    BuffHelper.Poison(e, i);
                    break;
                case DebuffType.Heal:
                    BuffHelper.Heal(e, i);
                    break;
                case DebuffType.Slow:
                    BuffHelper.Slow(e, i);
                    break;
                case DebuffType.AmplifyDmg:
                    BuffHelper.AmplifyDmg(e, i);
                    break;
                case DebuffType.ShieldBreak:
                    BuffHelper.ShieldBreak(e, i);
                    break;
            }
        }
        if (e.speed < e.minSpeed && e.moveable) e.speed = e.minSpeed;
        if (e.damageMulti < 0) e.damageMulti = 0;   //don't heal from damage. TODO: unless some variable
    }

    public static void ResetDebuffs(Enemy e)    //TODO: instead of applying and resetting every update, just apply once and fix clean up when its finished
    {
        e.speed = e.startSpeed;
        e.damageMulti = e.baseDamageMulti;
    }
    //
    //Turret functions
    //
    public static void AddDebuff(Turret t, DebuffType _type, float _time, float _amount, GameObject _effect = null)
    {
        //if debuff doesn't already exist or it can't be stacked, add it and if it has an effect instantiate that
        if (_effect != null)
        {
            GameObject effectIns = GameObject.Instantiate(_effect, t.transform.position, Quaternion.Euler(0, 0, 0));
            effectIns.transform.SetParent(t.transform);
            t.debuffList.Add(new Debuff(_type, _time, _amount, effectIns));

        }
        else
        {
            GameObject effectIns = GameObject.Instantiate(t.emptyPlaceHolder, t.transform.position, Quaternion.Euler(0, 0, 0));
            t.debuffList.Add(new Debuff(_type, _time, _amount, effectIns));
        }
    }

    public static void AddDebuff(Turret t, Debuff d)
    {
        AddDebuff(t, d.type, d.time, d.amount, d.effect);
    }

    public static void CheckDebuffs(Turret t) //Calls BuffHelper on every debuff in this enemies list
    {
        if (t.debuffList.Count == 0) return;
        for (int i = t.debuffList.Count - 1; i >= 0; --i)
        {
            if (t.debuffList[i].type == DebuffType.AtkSpeed)
            {
                BuffHelper.atkSpeed(t, i);
            }
        }
        if (t.fireRate < 0) t.fireRate = 0;
    }

    public static void ResetDebuffs(Turret t)
    {
        t.fireRate = t.baseFireRate;
    }

    #region Buff Definitions
    //Enemies
    public static void LaserSlow(Enemy e, int i) //i = iterator to access debuff in the list, could pass in effect itself :/??
    {
        if (e.debuffList[i].time <= 0)
        {
            GameObject.Destroy(e.debuffList[i].effect); //Placeholder empty so no need to check for null
            e.debuffList.RemoveAt(i);
            //Slows amount is reset every update (Meaning if it isn't being actively applied then it wont do anything)
            //because it's a laser that always applies so it would stack by itself if it worked like the rest
        }
        else
        {
            if (e.moveable)
            {
                e.speed -= e.debuffList[i].amount * e.startSpeed;   //Slow between 0 - 1 * starting speed == speed change between 0% - 100%
            }
            e.debuffList[i].time -= Time.deltaTime;
            e.debuffList[i].amount = 0;
        }
    }

    public static void Freeze(Enemy e, int i)
    {
        if (e.debuffList[i].time <= 0)
        {
            e.moveable = true;
            GameObject.Destroy(e.debuffList[i].effect);
            e.debuffList.RemoveAt(i);
        }
        else
        {
            e.moveable = false;
            e.speed = 0;
            e.debuffList[i].time -= Time.deltaTime;
        }
    }

    public static void Fear(Enemy e, int i)
    {
        if (e.debuffList[i].time <= 0)
        {
            if (e.enemyMovement.fear) e.enemyMovement.SetFear(false);
            GameObject.Destroy(e.debuffList[i].effect);
            e.debuffList.RemoveAt(i);
        }
        else
        {
            if(!e.enemyMovement.fear) e.enemyMovement.SetFear(true);
            e.debuffList[i].time -= Time.deltaTime;
        }
    }

    public static void Poison(Enemy e, int i)
    {
        if (e.debuffList[i].time <= 0)
        {
            GameObject.Destroy(e.debuffList[i].effect);
            e.debuffList.RemoveAt(i);
        }
        else
        {
            e.TakeDamage(e.debuffList[i].amount * Time.deltaTime);
            e.debuffList[i].time -= Time.deltaTime;
        }
    }

    public static void Heal(Enemy e, int i)
    {
        if (e.debuffList[i].time <= 0)
        {
            GameObject.Destroy(e.debuffList[i].effect);
            e.debuffList.RemoveAt(i);
        }
        else
        {
            e.TakeDamage(e.debuffList[i].amount * -1f * Time.deltaTime);
            e.debuffList[i].time -= Time.deltaTime;
        }
    }

    public static void Slow(Enemy e, int i)
    {
        if (e.debuffList[i].time <= 0)
        {
            GameObject.Destroy(e.debuffList[i].effect);
            e.debuffList.RemoveAt(i);
        }
        else
        {
            if (e.moveable)
            {
                e.speed -= e.debuffList[i].amount * e.startSpeed;
            }
            e.debuffList[i].time -= Time.deltaTime;
        }
    }

    public static void AmplifyDmg(Enemy e, int i)
    {
        if (e.debuffList[i].time <= 0)
        {
            GameObject.Destroy(e.debuffList[i].effect);
            e.damageMulti -= e.debuffList[i].amount;
            e.debuffList.RemoveAt(i);
        }
        else
        {
            e.damageMulti += e.debuffList[i].amount;
            e.debuffList[i].time -= Time.deltaTime;
        }
    }

    public static void ShieldBreak(Enemy e, int i)
    {
        if (!e.useShield) e.debuffList[i].time = 0;
        if (e.debuffList[i].time <= 0)
        {
            GameObject.Destroy(e.debuffList[i].effect);
            e.debuffList.RemoveAt(i);
        }
        else
        {
            e.TakeShieldDamage(e.debuffList[i].amount * Time.deltaTime);
            e.debuffList[i].time -= Time.deltaTime;
        }
    }

    //Turrets
    static void atkSpeed(Turret t, int i)
    {
        if (t.debuffList[i].time <= 0)
        {
            t.fireRate -= t.debuffList[i].amount * t.baseFireRate;
            GameObject.Destroy(t.debuffList[i].effect);
            t.debuffList.RemoveAt(i);
        }
        else
        {
            t.fireRate += t.debuffList[i].amount * t.baseFireRate; //Fire rate between 0 - 1 * starting fire rate == fire rate change between 0% - 100%
            t.debuffList[i].time -= Time.deltaTime;
        }
    }
    #endregion
}