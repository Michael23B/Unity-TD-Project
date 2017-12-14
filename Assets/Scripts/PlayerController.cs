using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float startSpeed = 10f;
    [SerializeField]
    private float verySpeed = 20f;
    private float speed = 10f;
    [SerializeField]
    private float turnSpeed = 3f;

    float toggleCoolDown;   //otherwise camera toggle gets spammed every update

    private PlayerMotor motor;
    Camera sceneCamera, playerCamera;
    [SerializeField]
    Camera fpCamera;

    private void Start()
    {
        sceneCamera = Camera.main;
        playerCamera = GetComponentInChildren<Camera>();
        motor = GetComponent<PlayerMotor>();
        speed = startSpeed;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            if (toggleCoolDown <= 0)
            {
                FirstPersonCamera();
                toggleCoolDown = 1f;
            }
        }
        toggleCoolDown -= Time.deltaTime;

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
}
