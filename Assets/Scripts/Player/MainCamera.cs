using UnityEngine;
using UnityEngine.SceneManagement;

public class MainCamera : MonoBehaviour {

    public static MainCamera Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) Destroy(gameObject);
    }
}
