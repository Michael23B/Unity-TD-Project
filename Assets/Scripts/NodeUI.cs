using UnityEngine;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour {

    public GameObject UI;

    public Text upgradeCost;
    public Button upgradeButton;
    public Text sellAmount;
    public GameObject rangeIndicator;

    private Node target;

    public void SetTarget(Node _target)
    {
        target = _target;

        transform.position = target.GetBuildPosition();

        if (target.turret != null)
        {
            float _range = target.turretBlueprint.GetRange(target.isUpgraded) * 2;  //turret range is radius
            rangeIndicator.transform.localScale = new Vector3(_range, 0.01f, _range);
            rangeIndicator.SetActive(true);
        }

        if (!target.isUpgraded)
        {

            upgradeCost.text = "$" + target.turretBlueprint.upgradeCost;
            upgradeButton.interactable = true;
        } else
        {
            upgradeCost.text = "MAX";
            upgradeButton.interactable = false;
        }

        sellAmount.text = "$" + target.turretBlueprint.GetSellAmount(target.isUpgraded);

        UI.SetActive(true);
    }
    
    public void Hide()
    {
        rangeIndicator.SetActive(false);
        UI.SetActive(false);
    }

    public void Upgrade()   //TODO: hover over upgrade shows upgraded range
    {
        target.CallBuildTurret(true);
        BuildManager.Instance.DeselectNode();
    }

    public void Sell()
    {
        target.CallSellTurret();
        BuildManager.Instance.DeselectNode();
    }
}
