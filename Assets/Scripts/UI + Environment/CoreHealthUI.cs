using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CoreHealthUI : MonoBehaviour {

    public GameObject heartPrefab;

    public List<GameObject> hearts = new List<GameObject>();

    public Text livesText;
    public Text livesTextOverlay;

    void Update()
    {
        livesText.text = "CORE HEALTH: " + PlayerStats.Instance.lives;
        livesTextOverlay.text = PlayerStats.Instance.lives.ToString();

        if (PlayerStats.Instance.lives <= 0)
        {
            gameObject.SetActive(false);
            return;
        }

        while (PlayerStats.Instance.lives != hearts.Count)
        {
            if (PlayerStats.Instance.lives > hearts.Count)
            {
                GameObject h = Instantiate(heartPrefab, transform);
                hearts.Add(h);
            }

            else if (PlayerStats.Instance.lives < hearts.Count)
            {
                GameObject latestHeart = hearts[hearts.Count - 1];
                hearts.Remove(latestHeart);
                Destroy(latestHeart);
            }
        }
    }
}
