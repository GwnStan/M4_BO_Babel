using UnityEngine;
 
public class Billboard : MonoBehaviour
{
    //Dont use public to show field in inspector
    //Keep it private and use [SerializeField]
    //Research Why???
    [SerializeField] private Transform _camera;
 
    private void LateUpdate()
    {
        transform.LookAt(transform.position + _camera.forward);
    }
}
