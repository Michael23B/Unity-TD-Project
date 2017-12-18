using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour {

    [Header("Blueprints Array")]
    public TurretBlueprint[] Blueprints;

    BuildManager buildManager;

    [HideInInspector]
    public Button[] buttons;
    [Space(20)]
    public int[] player1Turrets;
    public int[] player2Turrets;

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

    public void EnableButtons(int index)
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
            EnableButtons(i);
        }
    }
}
