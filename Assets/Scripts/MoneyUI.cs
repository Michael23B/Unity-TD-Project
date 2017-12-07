using UnityEngine;
using UnityEngine.UI;

public class MoneyUI : MonoBehaviour {

    public Text moneyText;
    public int moneyAmount = 20;

    private void Start()
    {
        InvokeRepeating("GetMoneyAmount", 0f, 0.1f);
    }

    void GetMoneyAmount()
    {
        moneyAmount = PlayerStats.Instance.money;
        moneyText.text = "$" + moneyAmount;
    }
}
