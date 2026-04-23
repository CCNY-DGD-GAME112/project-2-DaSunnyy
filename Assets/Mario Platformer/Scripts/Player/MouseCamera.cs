using UnityEngine;

public class MouseCamera : MonoBehaviour
{
    public Transform player;

    public Vector3 offset = new Vector3(0, 5, -25);
    public float followSpeed = 10f;
    public float sensitivity = 3f;

    float yaw;
    float pitch;

    public float minPitch = -20f;
    public float maxPitch = 60f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        yaw = player.eulerAngles.y;
        pitch = 10f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            yaw = player.eulerAngles.y;
            pitch = 10f;
        }

        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        Vector3 desiredPosition = player.position + rotation * offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        transform.LookAt(player.position + Vector3.up * 1.5f);
    }
}