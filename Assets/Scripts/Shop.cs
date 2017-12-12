using UnityEngine;

public class Shop : MonoBehaviour {

    [Header("Blueprints Array")]
    public TurretBlueprint[] Blueprints;

    BuildManager buildManager;

    private void Start()
    {
        buildManager = BuildManager.Instance;
    }

    public void SelectTurret(int index)
    {
        buildManager.SelectTurretToBuild(Blueprints[index], index);

        switch (index)
        {
            case 0:
                Debug.Log("Standard Turret Selected.");
                break;
            case 1:
                Debug.Log("Missle Launcher Selected.");
                break;
            case 2:
                Debug.Log("Laser Turret Selected.");
                break;
            case 3:
                Debug.Log("Freeze Turret Selected.");
                break;
            case 4:
                Debug.Log("Poison Turret Selected.");
                break;
            case 5:
                Debug.Log("Speed Turret Selected.");
                break;
            case 6:
                Debug.Log("Fear Turret Selected.");
                break;
        }
    }
}
