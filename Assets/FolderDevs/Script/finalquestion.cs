using UnityEngine;

public class FinalQuestion : MonoBehaviour
{
    [SerializeField] private CanvasGroup myUiGroup;
    [SerializeField] private Transform target;      // the point you're approaching
    [SerializeField] private Transform player;       // usually your camera or player object

    [SerializeField] private float startFadeDistance = 10f; // distance where fade begins (alpha 0)
    [SerializeField] private float fullFadeDistance = 1f;   // distance where fade completes (alpha 1)
    [SerializeField] private movement MovementScript;
    [SerializeField] private mouseLook MouseLookScript;

    public bool reachedFull = false;

    void Update()
    {
        float distance = Vector3.Distance(player.position, target.position);

        float t = Mathf.InverseLerp(startFadeDistance, fullFadeDistance, distance);
        float alpha = Mathf.Clamp01(t);

        myUiGroup.alpha = alpha;

        if (alpha >= 1f && !reachedFull)
        {
            reachedFull = true;

            if (MouseLookScript != null)
            {
                MouseLookScript.enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (MovementScript != null) MovementScript.canMove = false;
        }




    }





}