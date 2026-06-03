using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform orientation;
    [SerializeField] float sensitivity = 2f;
    [SerializeField] Vector3 followOffset = new Vector3(0f, 1.6f, 0f);
    [SerializeField] float minPitch = -80f;
    [SerializeField] float maxPitch = 80f;

    float pitch;
    float yaw;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (target != null)
        {
            transform.position = target.position + followOffset;
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity * Time.timeScale;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity * Time.timeScale;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.position = target.position + followOffset;
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        if (orientation != null)
        {
            orientation.rotation = Quaternion.Euler(0f, yaw, 0f);
        }
    }
}
