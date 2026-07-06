using UnityEngine;

public class DoorInteract : Interactable
{
    private Animator animator;
    private bool opened = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override void Interact()
    {
        if (opened) return;

        opened = true;
        animator.SetTrigger("Open");
    }
}