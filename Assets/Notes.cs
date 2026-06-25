using UnityEngine;

public class Notes : MonoBehaviour
{
    public GameObject Notepicture;
    public movement MovementScript; // assign in Inspector

    private bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            bool isOpening = !Notepicture.activeSelf;
            Notepicture.SetActive(isOpening);

            if (MovementScript != null) MovementScript.canMove = !isOpening;
        }
    }
}