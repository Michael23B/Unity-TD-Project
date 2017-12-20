using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour {

    [Header("Blueprints Array")]
    public TurretBlueprint[] Blueprints;    //TurretSelect currently relies on blueprints index matching buttons index (button 0 accesses blueprint 0)

    BuildManager buildManager;

    [HideInInspector]
    public Button[] buttons;

    private void Start()
    {
        buildManager = BuildManager.Instance;
        buttons = gameObject.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons) button.gameObject.SetActive(false);
    }

    public void SelectTurret(int index)
    {
        buildManager.SelectTurretToBuild(Blueprints[index], index);

        Debug.Log(Blueprints[index].prefab.name + " selected.");
    }

    public void EnableButton(int index)
    {
        buttons[index].gameObject.SetActive(true);
    }

    public void DisableAllButtons()
    {
        foreach (Button button in buttons) button.gameObject.SetActive(false);
    }

    public void EnableTurrets(int[] arr)
    {
        DisableAllButtons();
        foreach (int i in arr)
        {
            EnableButton(i);
        }
    }
}
