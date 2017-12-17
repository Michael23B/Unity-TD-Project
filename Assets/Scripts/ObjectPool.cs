using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {

    public GameObject pooledObject;
    int pooledAmount = 10;
    public bool canGrow = true;

    List<GameObject> pooledObjects;

    private void Start()
    {
        pooledObjects = new List<GameObject>();

        for (int i = 0; i < pooledAmount; ++i)  //create the pool
        {
            GameObject obj = Instantiate(pooledObject);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; ++i)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        if (canGrow)
        {
            GameObject obj = Instantiate(pooledObject);
            obj.SetActive(false);
            pooledObjects.Add(obj);
            return obj;
        }

        return null;
    }
}
