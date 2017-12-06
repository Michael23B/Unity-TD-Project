using UnityEngine;
using UnityEngine.UI;

public class MoneyUI : MonoBehaviour {

    public Text moneyText;
    public int moneyAmount = 20;

    void Update()
    {
        moneyAmount = PlayerStats.Instance.money;
        moneyText.text = "$" + moneyAmount;
    }
}
