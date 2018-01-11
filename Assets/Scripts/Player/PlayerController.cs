using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//TODO: there is a bug sometimes when switching between scene and fpcamera that makes the graphics rotation out of sync with the motor, happens only sometimes tho
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float startSpeed = 10f;
    [SerializeField]
    private float verySpeed = 20f;
    private float speed = 10f;
    [SerializeField]
    private float turnSpeed = 3f;

    float coolDownSS3;

    private PlayerMotor motor;
    public Camera sceneCamera, playerCamera, fpCamera;

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

    //bool fpCamMode = false;

    private void Start()
    {
        sceneCamera = Camera.main;
        motor = GetComponent<PlayerMotor>();
        speed = startSpeed;

        commands = FindObjectOfType<LocalPlayerCommands>();
        commands.CmdSetClientsRandomValues();

        WaveSpawner.Instance.commands = commands;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        motor = GetComponent<PlayerMotor>();
        speed = startSpeed;

        commands = FindObjectOfType<LocalPlayerCommands>();
        commands.CmdSetClientsRandomValues();

        WaveSpawner.Instance.commands = commands;

        ResetCameras();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (!sceneCamera.isActiveAndEnabled && !playerCamera.isActiveAndEnabled && !fpCamera.isActiveAndEnabled)
        {
            ResetCameras();
        }
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
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //ToggleCamera(!sceneCamera.isActiveAndEnabled);
            ToggleTacticalUI();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            FirstPersonCamera(!fpCamera.isActiveAndEnabled);
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
            motor.Rotate(rotation);
        }
        else
        {
            RotateToMouse();
            motor.Rotate(new Vector3(0f, 0f, 0f));  //TODO: make a variable to check whether we want to lock rotation, lock / unlock when toggling fpCamera
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

    #region Camera controllers
    /*
    void ToggleCamera(bool active)  //turns off fpcamera then activates either scene cam or player cam
    {
        if (sceneCamera == null) return;
        if (playerCamera == null) return;
        if (fpCamera == null) return;

        if (active) fpCamMode = fpCamera.isActiveAndEnabled;  //if activating scene cam, remember what cam was active before, so we can switch back to it

        if (fpCamMode)
        {
            playerCamera.gameObject.SetActive(false);
            fpCamera.gameObject.SetActive(!active);
            sceneCamera.gameObject.SetActive(active);
        }
        else
        {
            playerCamera.gameObject.SetActive(!active);
            sceneCamera.gameObject.SetActive(active);

            //lock mouse when using scene cam (fp cam handles this itself) ((this code is so messy i need to clean it))
            if (active)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = (false);
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = (true);
            }
        }
    }
    */

    void ToggleTacticalUI()
    {
        GameObject go = WaveSpawner.Instance.tacticalCamera.gameObject;
        go.SetActive(!go.activeInHierarchy);
    }

    void FirstPersonCamera(bool active)    //disables scene cam and toggles fp or player camera (beta)
    {
        if (sceneCamera == null) return;
        if (playerCamera == null) return;

        //sceneCamera.gameObject.SetActive(false);
        fpCamera.gameObject.SetActive(active);
        playerCamera.gameObject.SetActive(!active);

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
            //graphics.transform.rotation = Quaternion.identity;
        }
    }

    public void ResetCameras()
    {
        sceneCamera.gameObject.SetActive(false);
        fpCamera.gameObject.SetActive(false);
        WaveSpawner.Instance.tacticalCamera.gameObject.SetActive(false);

        playerCamera.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = (true);
        motor.transform.rotation = Quaternion.identity;
        graphics.transform.rotation = Quaternion.identity;
        //fpCamMode = false;
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
        gun.damage *= 3;

        Invoke("LeashThis", 22f);
    }

    void LeashThis()
    {
        soundTest = false;
        startSpeed = 10;
        verySpeed = 30;
        gun.damage /= 3;
    }
    #endregion
}
