using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float startSpeed = 10f;
    [SerializeField]
    private float verySpeed = 20f;
    private float speed = 10f;
    [SerializeField]
    private float turnSpeed = 3f;

    float coolDownCamera, coolDownSS3;   //otherwise camera toggle gets spammed every update

    private PlayerMotor motor;
    Camera sceneCamera, playerCamera;
    [SerializeField]
    Camera fpCamera;

    private bool evenFurtherBeyondAHHHHHHHHHHHHHHH = false;

    public GameObject bankaiEffect;

    [SerializeField]
    LocalPlayerCommands commands;

    public NetworkAudio networkAudio;

    public Canvas bottomCanvas;
    public CanvasGroup bottomCanvasGroup;

    private void Start()
    {
        sceneCamera = Camera.main;
        playerCamera = GetComponentInChildren<Camera>();
        motor = GetComponent<PlayerMotor>();
        speed = startSpeed;
    }

    private void Update()
    {
        if (evenFurtherBeyondAHHHHHHHHHHHHHHH)
        {
            startSpeed = 50;
            verySpeed = 150;
            coolDownSS3 -= Time.deltaTime;
            if (Input.GetKey(KeyCode.LeftShift) && coolDownSS3 <= 0f)
            {
                commands.CmdPlaySound(0);
                coolDownSS3 = Random.Range(0.01f, 0.5f);
            }
        }

        if (Input.GetKey(KeyCode.B))
        {
            if (!evenFurtherBeyondAHHHHHHHHHHHHHHH)
            {
                commands.CmdUnleashThis(GetComponent<NetworkIdentity>());
            }
        }

        if (Input.GetKey(KeyCode.E))
        {
            if (coolDownCamera <= 0)
            {
                FirstPersonCamera();
                coolDownCamera = 1f;
            }
        }
        coolDownCamera -= Time.deltaTime;

        if (!fpCamera.isActiveAndEnabled)   //if fpcamera is enabled don't switch with tab
        {
            if (Input.GetKey(KeyCode.Tab)) ToggleCamera(true);
            else ToggleCamera(false);
        }

        if (Input.GetKey(KeyCode.LeftShift)) speed = verySpeed;
        else speed = startSpeed;

        //Calculate movement velocity as vector3
        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * xMov;
        Vector3 moveVertical = transform.forward * zMov;

        //Final movement vector
        Vector3 velocity = (moveHorizontal + moveVertical).normalized * speed;

        //Apply movement
        motor.Move(velocity);

        if (fpCamera.isActiveAndEnabled)
        {
            //Calculate roation as vector3
            float yRot = Input.GetAxisRaw("Mouse X");

            Vector3 rotation = new Vector3(0, yRot, 0) * turnSpeed;

            //Apply rotation
            motor.transform.Rotate(rotation);
        }
        else
        {
            motor.transform.rotation = Quaternion.identity; //reset camera rotation
        }
    }
    #region Camera controls
    void ToggleCamera(bool active)  //turns off fpcamera then activates either scene cam or player cam
    {
        if (sceneCamera == null) return;
        if (playerCamera == null) return;
        if (fpCamera == null) return;

        fpCamera.gameObject.SetActive(false);

        sceneCamera.gameObject.SetActive(active);
        playerCamera.gameObject.SetActive(!active);
    }

    void FirstPersonCamera()    //disables scene cam and toggles fp or player camera (beta)
    {
        if (sceneCamera == null) return;
        if (playerCamera == null) return;

        sceneCamera.gameObject.SetActive(false);
        fpCamera.gameObject.SetActive(!fpCamera.isActiveAndEnabled);
        playerCamera.gameObject.SetActive(!fpCamera.isActiveAndEnabled);

        Cursor.visible = (!fpCamera.isActiveAndEnabled);
        if(fpCamera.isActiveAndEnabled) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;
    }
    #endregion
    #region Particle test (ss3)
    public void UnleashThis()
    {
        GameObject effectIns = Instantiate(bankaiEffect, transform);
        effectIns.transform.position = transform.position;
        GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
        Destroy(effectIns, 22f);
        evenFurtherBeyondAHHHHHHHHHHHHHHH = true;

        Invoke("LeashThis", 22f);
    }

    void LeashThis()
    {
        GetComponentInChildren<MeshRenderer>().material.color = Color.white;
        evenFurtherBeyondAHHHHHHHHHHHHHHH = false;
        startSpeed = 10;
        verySpeed = 30;
    }
    #endregion
}
