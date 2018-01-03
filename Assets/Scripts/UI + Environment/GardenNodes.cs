using UnityEngine;

public class GardenNodes : MonoBehaviour {

    public static Transform[] gardenNodes;

    private void Awake()
    {
        gardenNodes = new Transform[transform.childCount];
        for (int i = 0; i < gardenNodes.Length; ++i)
        {
            gardenNodes[i] = transform.GetChild(i);
            gardenNodes[i].GetComponent<GardenNode>().nodeID = i;
        }
    }
}
