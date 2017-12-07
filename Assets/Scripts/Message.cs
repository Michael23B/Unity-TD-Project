using UnityEngine;
using UnityEngine.UI;

public class Message : MonoBehaviour {

    [SerializeField]
    Text text;
    [SerializeField]
    GameObject prefab;

    public void PlayMessage(string message, Transform pos)
    {
        text.text = message;
        GameObject GO = Instantiate(prefab, pos, true);
        Destroy(GO, 2f);
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
