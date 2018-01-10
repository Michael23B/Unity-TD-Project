using UnityEngine;
using UnityEngine.UI;

public class Message : MonoBehaviour {

    [SerializeField]
    GameObject prefab;
    [SerializeField]
    GameObject bossPrefab;
    GameObject prefabToUse;

    public void PlayMessage(string message, Transform pos, Color col, float speed = 1f, float scale = 1f, bool boss = false)
    {
        if (boss) prefabToUse = bossPrefab;
        else prefabToUse = prefab;

        GameObject GO = Instantiate(prefabToUse);
        Text goText = GO.GetComponentInChildren<Text>(false);
        goText.color = col;
        goText.text = message;

        GO.gameObject.transform.position = pos.position;
        GO.gameObject.transform.localScale = new Vector3(scale, scale, scale);

        Animator anim = GO.GetComponentInChildren<Animator>();
        anim.speed = speed;

        Destroy(GO, 2f * (1 / speed));  //the lower the speed, the more time it needs before being destroyed. eg destroy a 0.1 scale object after 10x the usual destroy time etc.
    }
                                        /*
                                        ▒▒▒▒▒▒▒▒▄▄▄▄▄▄▄▄▒▒▒▒▒▒▒▒ 
                                        ▒▒▒▒▒▄█▀▀░░░░░░▀▀█▄▒▒▒▒▒ 
                                        ▒▒▒▄█▀▄██▄░░░░░░░░▀█▄▒▒▒
                                        ▒▒█▀░▀░░▄▀░░░░▄▀▀▀▀░▀█▒▒ 
                                        ▒█▀░░░░███░░░░▄█▄░░░░▀█▒
                                        ▒█░░░░░░▀░░░░░▀█▀░░░░░█▒
                                        ▒█░░░░░░░░░░░░░░░░░░░░█▒ 
                                        ▒█░░██▄░░▀▀▀▀▄▄░░░░░░░█▒ 
                                        ▒▀█░█░█░░░▄▄▄▄▄░░░░░░█▀▒
                                        ▒▒▀█▀░▀▀▀▀░▄▄▄▀░░░░▄█▀▒▒
                                        ▒▒▒█░░░░░░▀█░░░░░▄█▀▒▒▒▒
                                        ▒▒▒█▄░░░░░▀█▄▄▄█▀▀▒▒▒▒▒▒
                                        ▒▒▒▒▀▀▀▀▀▀▀▒▒▒▒▒▒▒▒▒▒▒▒▒
                                        */
}
