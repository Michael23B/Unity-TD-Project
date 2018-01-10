using UnityEngine;
using UnityEngine.UI;
//TODO: add check for a wavespawner variable hasGameStarted that is true only when the first wave has begun, don't allow planting until then
public class GardenNodeUI : MonoBehaviour {

    public GameObject UI;

    public GameObject plantButtons;
    public Text harvestTime;
    public Button harvestButton;

    public GameObject currentPlantInfoBox;
    public Text currentPlantNameText;
    public Text currentPlantInfoText;
    Image currentPlantInfoBoxImage;

    public Image harvestCooldown;

    private GardenNode target;

    public GameObject[] plantList;
    [HideInInspector]
    public Plant[] plantComponentList;
    public static GameObject[] plantListReference;

    private void Start()
    {
        plantListReference = plantList;
        plantComponentList = new Plant[plantList.Length];
        for (int i = 0; i < plantList.Length; ++i)
        {
            plantComponentList[i] = plantList[i].GetComponent<Plant>();
        }
        currentPlantInfoBoxImage = currentPlantInfoBox.GetComponent<Image>();
    }

    private void Update()
    {
        if (target == null) return;

        if (target.plantComponent == null)
        {
            harvestTime.text = "No plant!";
            harvestButton.interactable = false;
            plantButtons.SetActive(true);
            currentPlantInfoBox.SetActive(false);
            return;
        }

        if (!target.CheckRipe())
        {
            float timeLeft = target.timeTillRipe - Time.time;

            harvestTime.text = string.Format("{0:00}", timeLeft);
            harvestCooldown.fillAmount = timeLeft / target.plantComponent.harvestTime;
            harvestButton.interactable = false;
        }
        else
        {
            harvestCooldown.fillAmount = 0;
            harvestTime.text = "Ready!";
            harvestButton.interactable = true;
        }
    }

    public void SetTarget(GardenNode _target)
    {
        if (_target == target)
        {
            Hide();
            return;
        }

        target = _target;

        transform.position = target.transform.position;

        if (target.plantComponent != null)
        {
            plantButtons.SetActive(false);

            currentPlantInfoBox.SetActive(true);
            currentPlantNameText.text = target.plantComponent.displayName;
            currentPlantInfoText.text = target.plantComponent.info;
            currentPlantInfoBoxImage.color = target.plantComponent.displayColor;
        }
        else
        {
            plantButtons.SetActive(true);
            currentPlantInfoBox.SetActive(false);
            harvestCooldown.fillAmount = 0;
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
        target = null;
    }

    public void Plant(int index)   //TODO: hover over upgrade shows upgraded range
    {
        if (target.plantComponent != null)
        {
            BuildManager.Instance.message.PlayMessage("Already Planted Here!", transform, Color.red);
            return;
        }
        if (PlayerStats.Instance.money >= plantComponentList[index].cost)
        {
            PlayerStats.Instance.money -= plantComponentList[index].cost;
            target.CallPlant(index);
            Hide();
        }
        else
        {
            BuildManager.Instance.message.PlayMessage("Not Enough Money!", transform, Color.red);
        }
    }

    public void Harvest()
    {
        target.plantComponent.Harvest();
        target.CallHarvest();
        Hide();
    }
}
