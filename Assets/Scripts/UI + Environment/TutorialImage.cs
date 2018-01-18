using UnityEngine;
using UnityEngine.UI;

public class TutorialImage : MonoBehaviour {

    static bool playerSeen = false;

    public GameObject tutorialPicture;
    Image image;
    Animator anim;

    void Start () {
        if (playerSeen) gameObject.SetActive(false);
        playerSeen = true;

        image = tutorialPicture.GetComponent<Image>();
        anim = tutorialPicture.GetComponent<Animator>();
        anim.enabled = false;
	}

    public void FadeOut()
    {
        anim.enabled = true; ;
        image.GetComponent<CanvasGroup>().blocksRaycasts = false;
        Destroy(gameObject, 1f);
    }
}
