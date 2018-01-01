using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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
    public Image crosshair;

    private bool soundTest = false;

    public GameObject bankaiEffect;

    [SerializeField]
    LocalPlayerCommands commands;

    public NetworkAudio networkAudio;

    public Canvas bottomCanvas;
    public CanvasGroup bottomCanvasGroup;

    public Gun gun;
    float nextTimeToFire = 0f;
    float nextTimeToFire2 = 0f;

    public GameObject graphics;

    private void Start()
    {
        sceneCamera = Camera.main;
        playerCamera = GetComponentInChildren<Camera>();
        motor = GetComponent<PlayerMotor>();
        speed = startSpeed;

        commands = FindObjectOfType<LocalPlayerCommands>();
        commands.CmdSetClientsRandomValues();

        WaveSpawner.Instance.commands = commands;
    }

    private void Update()
    {
        SoundTest();
        Move();
        CameraCheck();
        CameraRotate();
        FireGun();
    }

    void FireGun()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            if (gun != null)
            {
                nextTimeToFire = Time.time + 0.20f;
                gun.Shoot();
                commands.CmdPlayShootEffect(WaveSpawner.Instance.playerID, 1);
            }
        }
        if (Input.GetMouseButtonDown(1) && Time.time >= nextTimeToFire2)
        {
            if (gun != null)
            {
                nextTimeToFire2 = Time.time + 20f;
                gun.AltShoot();
                commands.CmdPlayShootEffect(WaveSpawner.Instance.playerID, 2);
            }
        }
    }

    void Move()
    {
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
    }

    void CameraCheck()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            FirstPersonCamera();
        }
        coolDownCamera -= Time.deltaTime;

        if (!fpCamera.isActiveAndEnabled)   //if fpcamera is enabled don't switch with tab
        {
            if (Input.GetKey(KeyCode.Tab)) ToggleCamera(true);
            else ToggleCamera(false);
        }
    }

    void CameraRotate()
    {
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
            RotateToMouse();
        }
    }

    private void RotateToMouse()
    {
        Ray cameraRay = playerCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointToLook = cameraRay.GetPoint(rayLength);

            graphics.transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
        }
    }

    void SoundTest()
    {
        if (soundTest)
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

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!soundTest)
            {
                commands.CmdUnleashThis(GetComponent<NetworkIdentity>());
            }
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
        if (fpCamera.isActiveAndEnabled)
        {
            Cursor.lockState = CursorLockMode.Locked;
            motor.transform.rotation = graphics.transform.rotation;
            graphics.transform.localRotation = Quaternion.identity;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            motor.transform.rotation = Quaternion.identity;
            graphics.transform.rotation = Quaternion.identity;
        }
    }
    #endregion
    #region Particle test (ss3)
    public void UnleashThis()
    {
        if (soundTest) return;
        GameObject effectIns = Instantiate(bankaiEffect, transform);
        effectIns.transform.position = transform.position;
        Destroy(effectIns, 22f);
        soundTest = true;

        Invoke("LeashThis", 22f);
    }

    void LeashThis()
    {
        soundTest = false;
        startSpeed = 10;
        verySpeed = 30;
    }
    #endregion
}
