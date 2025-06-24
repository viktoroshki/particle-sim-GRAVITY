// FlyCameraWithCOM.cs  ===> Done by ChatGPT
//
// • Drop on your Main Camera.
// • Assign   ParticleSimulator   reference in Inspector (to read its COM).
// ----------------------------------------------------

using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FlyCameraWithCOM : MonoBehaviour
{
    [Header("Look")]
    public float lookSpeed = 3f;       // mouse sensitivity
    [Header("Move")]
    public float moveSpeed = 5f;       // free-fly speed
    public float sprintMult = 4f;       // Shift multiplier
    [Header("Zoom")]
    public float wheelSpeed = 10f;      // scroll speed
    [Header("Lock / Orbit")]
    public KeyCode lockKey = KeyCode.C; // toggle key
    public float minOrbitDistance = 5f; // how close you can zoom

    [Header("Dependencies")]
    public ParticleSimulator simulator; // drag here

    bool locked = false;
    float orbitDist;
    float yaw, pitch;

    void Awake()
    {
        var e = transform.eulerAngles;
        yaw = e.y;
        pitch = e.x;
    }

    void Update()
    {
        HandleModeToggle();
        HandleMouseLook();
        HandleMoveOrOrbit();
        HandleZoom();
    }

    // ——————————————————————
    void HandleModeToggle()
    {
        if (Input.GetKeyDown(lockKey) && simulator != null)
        {
            locked = !locked;

            if (locked)
            {
                // initialise orbit distance facing current COM
                Vector3 com = simulator.findCentreOfMass();
                orbitDist = Vector3.Distance(transform.position, com);
            }
        }
    }

    // ——————————————————————
    void HandleMouseLook()
    {
        if (!Input.GetMouseButton(1)) return;

        Cursor.lockState = CursorLockMode.Locked;

        yaw += Input.GetAxis("Mouse X") * lookSpeed;
        pitch -= Input.GetAxis("Mouse Y") * lookSpeed;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    // ——————————————————————
    void HandleMoveOrOrbit()
    {
        if (locked && simulator != null)
        {
            // Keep camera on orbit looking at COM
            Vector3 com = simulator.findCentreOfMass();
            Vector3 dir = (transform.position - com).normalized; // outward
            transform.position = com + dir * orbitDist;
            transform.LookAt(com);
        }
        else
        {
            // Free-fly movement
            Vector3 dir = new Vector3(
                Input.GetAxisRaw("Horizontal"),
                (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.R) ? 1 : 0) -
                (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.F) ? 1 : 0),
                Input.GetAxisRaw("Vertical"));
            if (dir.sqrMagnitude > 1e-4f) dir.Normalize();

            float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMult : 1f);
            transform.Translate(dir * speed * Time.deltaTime, Space.Self);
        }
    }

    // ——————————————————————
    void HandleZoom()
    {
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(wheel) < 1e-4f) return;

        if (locked && simulator != null)
        {
            // adjust orbit radius
            orbitDist = Mathf.Max(minOrbitDistance, orbitDist - wheel * wheelSpeed);
        }
        else
        {
            // dolly in free-fly
            transform.Translate(Vector3.forward * wheel * wheelSpeed, Space.Self);
        }
    }
}
