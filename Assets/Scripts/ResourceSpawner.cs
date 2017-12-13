using UnityEngine;
using System.Collections.Generic;

public class ResourceSpawner : MonoBehaviour {

    public static ResourceSpawner Instance;

    [SerializeField]
    List<GameObject> resourceList;

    private float ySpawnOffset = 1f;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Error: More than one Resource Spawner in scene!");
            return;
        }
        Instance = this;

        Random.InitState(1);
    }

    public void SpawnResources(int amount)
    {
        int r,rX,rZ;
        for (int i = 0; i < amount; ++i)
        {
            r = Random.Range(0, resourceList.Count);
            rX = Random.Range(-400, 400);
            rZ = Random.Range(-400, 400);
            Vector3 randomPos = new Vector3(rX, ySpawnOffset, rZ);

            Collider[] colliders = Physics.OverlapSphere(randomPos, 75);    //find any nodes within 75 units of the new resource
            foreach (Collider collider in colliders)
            {
                if (collider.tag == "Node") continue;   //don't create a node within range of a node
            }

            Instantiate(resourceList[r],randomPos, Quaternion.identity);
        }
    }
}
