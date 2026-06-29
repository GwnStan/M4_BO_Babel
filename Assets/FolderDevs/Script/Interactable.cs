using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string interactionText = "Press E";

    public abstract void Interact();
}