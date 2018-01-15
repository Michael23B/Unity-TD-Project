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
    [HideInInspector]
    public bool fading = true;
    [HideInInspector]
    public bool prevState = true;

    public GameObject stars;
    public GameObject starsObject;
    bool starsOn;

    private void Start()
    {
        startIntensity = directionalLight.intensity;
        StartCoroutine(LightCycle());
    }

    IEnumerator LightCycle()    //rotates to a point in the sky i liked the look of the shadows at. goes down and back up.
    {
        float reflectIntensity = 1f;
        while (true)
        {
            if (fading) directionalLight.intensity -= 0.00125f;
            else directionalLight.intensity += 0.00125f;
            //if we at min intensity stop fading, if we at max start fading
            if (directionalLight.intensity <= minIntensity) fading = false;
            else if (directionalLight.intensity >= startIntensity) fading = true;
            if (prevState != fading)    //if at max height (noon), wait for some time before going back down
            {
                if (fading == true) yield return new WaitForSeconds(100f);
                prevState = fading;
            }
            //makes light go down to zero, then back up
            directionalLight.transform.rotation = Quaternion.Euler((directionalLight.intensity * 50) - 37.5f, directionalLight.intensity * 50, directionalLight.transform.eulerAngles.z);

            if (directionalLight.intensity + 0.1 > 1) reflectIntensity = 1f;
            else reflectIntensity = directionalLight.intensity + 0.1f;

            RenderSettings.reflectionIntensity = reflectIntensity;

            if (directionalLight.intensity < 0.5 && !starsOn)   //turn stars on at 0.5 intensity
            {
                starsObject = Instantiate(stars);
                starsOn = true;
            }
            if (directionalLight.intensity > 0.5 && starsOn)    //turn off
            {
                starsOn = false;
                Destroy(starsObject);
            }

            yield return new WaitForSeconds(1f / cycleSpeed);
        }
    }
}
