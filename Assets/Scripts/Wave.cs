using UnityEngine;

[System.Serializable]
public class EnemyGroup
{
    public GameObject enemy;
    public int count;

    public float spawnRate;    //time between each enemy
    public float waitTime;      //time until next group spawns
}

[System.Serializable]
public class EnemyWave
{
    public EnemyGroup[] wave;

    public bool randomOrder;
}