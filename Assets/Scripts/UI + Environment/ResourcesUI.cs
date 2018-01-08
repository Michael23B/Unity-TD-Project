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
        stoneText.text = stoneAmount.ToString();

        diamondAmount = PlayerStats.Instance.diamond;
        diamondText.text = diamondAmount.ToString();

        greenAmount = PlayerStats.Instance.green;
        greenText.text = greenAmount.ToString();
    }
}
