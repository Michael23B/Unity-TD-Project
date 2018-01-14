using UnityEngine;
using System.Collections;


public class LightControl : MonoBehaviour {

    public Light directionalLight;
    [HideInInspector]
    public float startIntensity;
    public float minIntensity;
    [HideInInspector]
    public float intensity;

    public float cycleSpeed = 8f;
    bool fading = true;
    bool prevState = true;

    private void Start()
    {
        startIntensity = directionalLight.intensity;
        StartCoroutine(LightCycle());
    }

    IEnumerator LightCycle()    //doesnt actually rotate around the world, just goes up and back down, not realistic but also neither is any of the game, its a bunch of spheres endlessly walking into a cube and disappearing so back off
    {
        while (true)
        {
            if (fading) directionalLight.intensity -= 0.00125f;
            else directionalLight.intensity += 0.00125f;
            //if we at min intensity stop fading, if we at max start fading
            if (directionalLight.intensity <= minIntensity) fading = false;
            else if (directionalLight.intensity >= startIntensity) fading = true;
            if (prevState != fading)    //if at max or min height, fading will switch and we wait for a small while before continuing the cycle
            {
                yield return new WaitForSeconds(15f);
                prevState = fading;
            }
            //makes light go down to zero, then back up
            directionalLight.transform.rotation = Quaternion.Euler(directionalLight.intensity * 25, directionalLight.intensity * 50, directionalLight.transform.eulerAngles.z);

            yield return new WaitForSeconds(1f / cycleSpeed);
        }
    }
}
