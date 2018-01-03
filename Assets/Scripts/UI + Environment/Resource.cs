using UnityEngine;
using UnityEngine.EventSystems;

public enum ResourceTypes { stone, green, diamond }

public class Resource : MonoBehaviour {

    public float cooldown = 1f;
    [HideInInspector]
    float countdown = 0f;

    [SerializeField]
    GameObject hitEffect;
    [SerializeField]
    GameObject onCooldownHitEffect;

    [Header("Resources to give")]
    public int money;
    public int stone, green, diamond;

    public int resourceAmount = 10;

    void Update () {
        if (countdown < 0f) return;
        countdown -= Time.deltaTime;
	}

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (countdown > 0f)
        {
            Destroy(Instantiate(onCooldownHitEffect,transform), 0.75f);
            return;
        }
        CheckResources();

        Destroy(Instantiate(hitEffect, transform.position, Quaternion.identity), 2f);

        countdown = cooldown;

        if (--resourceAmount == 0) Destroy(gameObject);


    }

    void CheckResources()
    {
        if (money != 0) PlayerStats.Instance.money += money;
        if (stone != 0) PlayerStats.Instance.stone += stone;
        if (green != 0) PlayerStats.Instance.green += green;
        if (diamond != 0) PlayerStats.Instance.diamond += diamond;
    }
}
