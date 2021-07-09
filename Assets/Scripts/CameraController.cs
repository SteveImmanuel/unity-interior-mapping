using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float movementSpeed = .1f;
    public float mouseSensitivity = 4f;

    private float yaw;
    private float pitch;

    private void Awake()
    {
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += transform.forward * movementSpeed;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += transform.forward * movementSpeed * -1;
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.position += transform.up * movementSpeed;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position += transform.up * movementSpeed * -1;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += transform.right * movementSpeed * -1;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += transform.right * movementSpeed;
        }

        yaw += mouseSensitivity * Input.GetAxis("Mouse X");
        pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
        transform.eulerAngles = new Vector3(pitch, yaw, 0);
    }
}
