using UnityEngine;
using UnityEngine.UI;

public class GardenNodeUI : MonoBehaviour {

    public GameObject UI;

    public Text PlantCost;
    public Button plantButton;
    public Text harvestTime;
    public Button harvestButton;

    private GardenNode target;

    public void SetTarget(GardenNode _target)
    {
        if (_target == target && isActiveAndEnabled)
        {
            target = null;
            Hide();
            return;
        }
        target = _target;

        transform.position = target.transform.position;

        if (target.plant != null)
        {
            plantButton.interactable = false;
        }
        else
        {
            plantButton.interactable = true;
        }

        if (!target.CheckRipe())
        {
            harvestTime.text = target.timeTillRipe - Time.time + " seconds till ripe!";
            harvestButton.interactable = false;
        }
        else
        {
            harvestTime.text = "Ready!";
            harvestButton.interactable = true;
        }
        UI.SetActive(true);
    }
    
    public void Hide()
    {
        UI.SetActive(false);
    }

    public void Plant(int index)   //TODO: hover over upgrade shows upgraded range
    {
        target.Plant(index);
        Hide();
    }

    public void Harvest()
    {
        target.Harvest();
        Hide();
    }
}
