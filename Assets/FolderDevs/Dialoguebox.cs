using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Dialoguebox : MonoBehaviour
{
    [Header("Dialogue Content")]
    public DialogueSegment[] DialogueSegments;

    [Header("UI References")]
    public GameObject DialoguePanel;
    public Image SpeakerFaceDisplay;
    public Image DialogueBoxBorder;
    public Image DialogueBoxInner;
    public Image SkipIndicator;
    public TextMeshProUGUI SpeakerNameDisplay;
    public TextMeshProUGUI DialogueDisplay;
    public movement PlayerMovementScript;
    public mouseLook MouseLookScript;

    [Header("Settings")]
    public float textspeed = 30f;
    public KeyCode interactKey = KeyCode.F;
    public KeyCode advanceKey = KeyCode.Space;

    private bool playerInRange = false;
    private bool inDialogue = false;
    private bool canSkip = false;
    private int dialogueIndex = 0;
    
    void Start()
    {
        if (DialoguePanel != null)
            DialoguePanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            // Optional: end dialogue if player walks away
            // EndDialogue();
        }
    }

    private void Update()
    {
        // Start dialogue
        if (playerInRange && !inDialogue && Input.GetKeyDown(interactKey))
        {
            if (DialogueSegments == null || DialogueSegments.Length == 0)
            {
                Debug.LogWarning($"{gameObject.name}: No dialogue segments assigned!");
                return;
            }
            StartDialogue();
        }

        // Advance dialogue
        if (inDialogue && canSkip && Input.GetKeyDown(advanceKey))
        {
            AdvanceDialogue();
        }

        // Update skip indicator
        if (SkipIndicator != null)
            SkipIndicator.enabled = inDialogue && canSkip;
    }

    void StartDialogue()
    {
        if (MouseLookScript != null)
        {
            MouseLookScript.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        inDialogue = true;
        dialogueIndex = 0;

        if (PlayerMovementScript != null)
            PlayerMovementScript.enabled = false;

        

        if (DialoguePanel != null)
            DialoguePanel.SetActive(true);

        SetStyle(DialogueSegments[dialogueIndex].Speaker);
        StartCoroutine(PlayDialogue(DialogueSegments[dialogueIndex].Dialogue));
    }   

    void AdvanceDialogue()
    {
        dialogueIndex++;

        if (dialogueIndex >= DialogueSegments.Length)
        {
            EndDialogue();
            return;
        }

        SetStyle(DialogueSegments[dialogueIndex].Speaker);
        StartCoroutine(PlayDialogue(DialogueSegments[dialogueIndex].Dialogue));
    }

    void EndDialogue()
    {
        if (MouseLookScript != null)
        {
            MouseLookScript.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        inDialogue = false;
        canSkip = false;
        StopAllCoroutines();

        if (PlayerMovementScript != null)
            PlayerMovementScript.enabled = true;

        

        if (DialoguePanel != null)
            DialoguePanel.SetActive(false);
    }

    void SetStyle(Subject speaker)
    {
        if (speaker == null)
        {
            Debug.LogWarning($"{gameObject.name}: Dialogue segment has no speaker assigned!");
            return;
        }

        if (SpeakerFaceDisplay != null)
        {
            SpeakerFaceDisplay.sprite = speaker.subjectface;
            SpeakerFaceDisplay.color = speaker.subjectface != null ? Color.white : new Color(0, 0, 0, 0);
        }

        if (DialogueBoxBorder != null) DialogueBoxBorder.color = speaker.bordercolor;
        if (DialogueBoxInner != null) DialogueBoxInner.color = speaker.innercolor;
        if (SpeakerNameDisplay != null) SpeakerNameDisplay.SetText(speaker.subjectname);
    }

    IEnumerator PlayDialogue(string dialogue)
    {
        canSkip = false;
        if (DialogueDisplay != null)
            DialogueDisplay.SetText(string.Empty);

        foreach (char c in dialogue)
        {
            if (DialogueDisplay != null)
                DialogueDisplay.text += c;

            yield return new WaitForSeconds(1f / textspeed);
        }

        canSkip = true;
    }
}

[System.Serializable]
public class DialogueSegment
{
    [TextArea(3, 10)]
    public string Dialogue;
    public Subject Speaker;
}