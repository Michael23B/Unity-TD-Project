using UnityEngine;

public class TacticalCamera : MonoBehaviour {

    public Camera tacticalCamP1;
    public Camera tacticalCamP2;

    Camera currentCam;

    void Start () {
        if (WaveSpawner.Instance.playerID == 0) currentCam = tacticalCamP1;
        else currentCam = tacticalCamP2;

        currentCam.gameObject.SetActive(true);
    }
}
