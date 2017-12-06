using UnityEngine;
using UnityEngine.EventSystems;

public class Resource : MonoBehaviour {

    public float cooldown = 1f;
    [HideInInspector]
    float countdown = 0f;

    [SerializeField]
    GameObject hitEffect;
    [SerializeField]
    GameObject onCooldownHitEffect;

    [Header("Resources to give")]
    public int money = 0;
    public int stone = 0;

    void Update () {
        if (countdown < 0f) return;
        countdown -= Time.deltaTime;
	}

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (countdown > 0f)
        {
            Destroy(Instantiate(onCooldownHitEffect,transform), 0.5f);
            return;
        }
        CheckResources();
        Destroy(Instantiate(hitEffect, transform), 2f);
        countdown = cooldown;
    }

    void CheckResources()
    {
        if (money != 0) PlayerStats.Instance.money += money;
        if (stone != 0) PlayerStats.Instance.stone += stone;
    }
}
