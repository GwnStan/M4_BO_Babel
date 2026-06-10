using UnityEngine;

public class Billboarding : MonoBehaviour
{
    [SerializeField] private BillboardType billboardType;

    public enum BillboardType
    {
        LookAtCamera,
        CameraForward
    };

    // Use Late update so everything should have finished moving.
    void LateUpdate()
    {
        // There are two ways people billboard things.
        switch (billboardType)
        {
            case BillboardType.LookAtCamera:
                transform.LookAt(Camera.main.transform.position, Vector3.up);
                break;

            case BillboardType.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;

            default:
                break;
        }
    }
}