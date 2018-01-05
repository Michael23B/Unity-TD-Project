using UnityEngine;

public class GardenNodes : MonoBehaviour {

    public static Transform[] gardenNodes;
    public GardenNodeUI UI;

    private void Awake()
    {
        gardenNodes = new Transform[transform.childCount];
        for (int i = 0; i < gardenNodes.Length; ++i)
        {
            gardenNodes[i] = transform.GetChild(i);

            GardenNode nodeComponent = gardenNodes[i].GetComponent<GardenNode>();

            nodeComponent.nodeID = i;
            nodeComponent.UI = UI;
        }
    }
}
