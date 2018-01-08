using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    public GameObject[] enemyPrefabList;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Error: More than one wave spawner in scene!");
            return;
        }
        Instance = this;
    }
}
