using UnityEngine;
//using UnityEngine.SceneManagement;

public class Ondeath : MonoBehaviour {

    public bool countDown = false; //if you handle your own death no need to countdown
    public float timeToDeath = 0f;
    public GameObject deathEffect;
    [HideInInspector]
    public bool isQuitting = false;

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnDestroy()    //TODO:it spawns stuff when the level reloads and its not a big deal but unity doesn't like that
    {
        if (!isQuitting)
        {
            GameObject effect = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(effect, 5f);
        }
    }

    private void Update()
    {
        if (!countDown) return;

        if(timeToDeath > 0)
        {
            timeToDeath -= Time.deltaTime;
            return;
        }

        Destroy(this);

    }
}
