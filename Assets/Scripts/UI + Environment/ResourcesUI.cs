using UnityEngine;
using UnityEngine.UI;


public class ResourcesUI : MonoBehaviour {

    public Text stoneText, diamondText, greenText;
    [HideInInspector]
    public int stoneAmount, diamondAmount, greenAmount;

    private void Start()
    {
        InvokeRepeating("GetResourceAmount", 0f, 0.1f);
    }

    void GetResourceAmount()
    {
        stoneAmount = PlayerStats.Instance.stone;
        stoneText.text = "Stone: " + stoneAmount;

        diamondAmount = PlayerStats.Instance.diamond;
        diamondText.text = "Diamond: " + diamondAmount;

        greenAmount = PlayerStats.Instance.green;
        greenText.text = "Green: " + greenAmount;
    }
}
