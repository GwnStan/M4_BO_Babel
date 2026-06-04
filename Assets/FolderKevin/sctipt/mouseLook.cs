using UnityEngine;

public class mouseLook : MonoBehaviour
{

    public float mousesensitivity = 0.5f;
    public Transform playerbody;
    private float Xrotation = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mousex = Input.GetAxis("Mouse X") * mousesensitivity * Time.deltaTime;
        float mousey = Input.GetAxis("Mouse Y") * mousesensitivity * Time.deltaTime;

        Xrotation -= mousey;
        Xrotation = Mathf.Clamp(Xrotation, -90f, 90f);


        transform.localRotation = Quaternion.Euler(Xrotation, 0f, 0f);

        playerbody.Rotate(Vector3.up * mousex);


    }
}