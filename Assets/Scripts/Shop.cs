using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour {

    [Header("Blueprints Array")]
    public TurretBlueprint[] Blueprints;

    BuildManager buildManager;

    private Button[] buttons;

    public int[] player1Turrets;
    public int[] player2Turrets;

    private void Start()
    {
        buildManager = BuildManager.Instance;

        for (int i = 0; i < transform.childCount; ++i)
        {
            buttons = gameObject.GetComponentsInChildren<Button>(true);
        }
    }

    public void SelectTurret(int index)
    {
        buildManager.SelectTurretToBuild(Blueprints[index], index);

        Debug.Log(Blueprints[index].prefab.name + " selected.");
    }

    public void ToggleButton(int index)
    {
        buttons[index].gameObject.SetActive(!isActiveAndEnabled);
    }

    public void DisableTurrets(int[] arr)
    {
        foreach (int i in arr)
        {
            ToggleButton(i);
        }
    }
}
