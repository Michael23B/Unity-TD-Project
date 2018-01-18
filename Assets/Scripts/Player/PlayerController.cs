using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    float nextTimeToBankai = 0f;

    public GameObject graphics;

    GameObject rmbCooldownImage;
    Image rmbCooldownImageComponent;
    GameObject ss3CooldownImage;
    Image ss3CooldownImageComponent;

    private void Start()
    {
        sceneCamera = Camera.main;
        motor = GetComponent<PlayerMotor>();
        speed = startSpeed;

        commands = FindObjectOfType<LocalPlayerCommands>();
        commands.CmdSetClientsRandomValues();

        WaveSpawner.Instance.commands = commands;
        WaveSpawner.Instance.localPlayer = this;

        commands.CmdRequestSceneState();

        rmbCooldownImage = GameObject.FindGameObjectWithTag("CooldownImageRmb");
        rmbCooldownImageComponent = rmbCooldownImage.GetComponent<Image>();
        ss3CooldownImage = GameObject.FindGameObjectWithTag("CooldownImageSS3");
        ss3CooldownImageComponent = ss3CooldownImage.GetComponent<Image>();
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
        WaveSpawner.Instance.localPlayer = this;
        
        ResetCameras();

        transform.position = new Vector3(0f, 2f, 0f);

        rmbCooldownImage = GameObject.FindGameObjectWithTag("CooldownImageRmb");
        rmbCooldownImageComponent = rmbCooldownImage.GetComponent<Image>();
        ss3CooldownImage = GameObject.FindGameObjectWithTag("CooldownImageSS3");
        ss3CooldownImageComponent = ss3CooldownImage.GetComponent<Image>();
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
        UpdateCooldownUI();
    }
    
    void UpdateCooldownUI()
    {
        if (Time.time < nextTimeToFire2)
        {
            rmbCooldownImageComponent.fillAmount = (nextTimeToFire2 - Time.time) / 20f;
        }
        else
        {
            rmbCooldownImageComponent.fillAmount = 0;
        }
        if (Time.time < nextTimeToBankai)
        {
            ss3CooldownImageComponent.fillAmount = (nextTimeToBankai - Time.time) / 300f;
        }
        else
        {
            ss3CooldownImageComponent.fillAmount = 0;
        }
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
            float yRot = Input.GetAxis("Mouse X");

            Vector3 rotation = new Vector3(0, yRot, 0) * turnSpeed;

            //Apply rotation
            motor.Rotate(rotation);
        }
        else
        {
            RotateToMouse();
            motor.Rotate(new Vector3(0f, 0f, 0f));
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
            if (!soundTest && Time.time > nextTimeToBankai)
            {
                commands.CmdUnleashThis(GetComponent<NetworkIdentity>());
            }
        }
    }

    #region Camera controllers

    void ToggleTacticalUI()
    {
        GameObject go = WaveSpawner.Instance.tacticalCamera.gameObject;
        go.SetActive(!go.activeInHierarchy);
    }

    void FirstPersonCamera(bool active)
    {
        if (sceneCamera == null) return;
        if (playerCamera == null) return;

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
        gun.damage += 50;
        nextTimeToBankai = Time.time + 300;

        Invoke("LeashThis", 22f);
    }

    void LeashThis()
    {
        soundTest = false;
        startSpeed = 10;
        verySpeed = 30;
        gun.damage -= 50;
    }
    #endregion
}
