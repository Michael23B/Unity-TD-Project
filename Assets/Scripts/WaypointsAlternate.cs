using UnityEngine;

public class WaypointsAlternate : MonoBehaviour
{
    public static Transform[] pointsAlternate;

    private void Awake()
    {
        pointsAlternate = new Transform[transform.childCount];
        for (int i = 0; i < pointsAlternate.Length; ++i)
        {
            pointsAlternate[i] = transform.GetChild(i);
        }
    }
}
