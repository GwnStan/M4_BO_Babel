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
    
    [Header("Choice UI")]
    public GameObject ChoicePanel;
    public Button[] ChoiceButtons;
    public TextMeshProUGUI[] ChoiceLabels;

    [Header("Settings")]
    public float textspeed = 30f;
    public KeyCode interactKey = KeyCode.F;
    public KeyCode advanceKey = KeyCode.Space;

    [Header("Player References")]
    public mouseLook MouseLookScript;
    public movement MovementScript;
    private bool playerInRange = false;
    public bool inDialogue = false;
    private bool canSkip = false;
    private int dialogueIndex = 0;

    void Start()
    {
        if (DialoguePanel != null)
            DialoguePanel.SetActive(false);

        if (ChoicePanel != null)
            ChoicePanel.SetActive(false);
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
            // Uncomment to end dialogue when player walks away:
            // if (inDialogue) EndDialogue();
        }
    }

    private void Update()
    {
        if (playerInRange && !inDialogue && Input.GetKeyDown(interactKey))
        {
            if (DialogueSegments == null || DialogueSegments.Length == 0)
            {
                Debug.LogWarning($"{gameObject.name}: No dialogue segments assigned!");
                return;
            }

            StartDialogue();
        }

        if (inDialogue && canSkip && Input.GetKeyDown(advanceKey))
            AdvanceDialogue();

        if (SkipIndicator != null)
            SkipIndicator.enabled = inDialogue && canSkip;
    }

    void StartDialogue()
    {
        inDialogue = true;
        dialogueIndex = 0;

        if (DialoguePanel != null)
            DialoguePanel.SetActive(true);

        if (MouseLookScript != null)
        {
            MouseLookScript.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (MovementScript != null)
        {
            MovementScript.canMove = false;
        }

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
        inDialogue = false;
        canSkip = false;
        StopAllCoroutines();

        if (DialoguePanel != null)
            DialoguePanel.SetActive(false);

        if (ChoicePanel != null)
            ChoicePanel.SetActive(false);

        if (MouseLookScript != null)
        {
            MouseLookScript.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (MovementScript != null)
        {
            MovementScript.canMove = true;
        }
            
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

        if (DialogueBoxBorder != null)
            DialogueBoxBorder.color = new Color(speaker.bordercolor.r, speaker.bordercolor.g, speaker.bordercolor.b, 1f);

        if (DialogueBoxInner != null)
            DialogueBoxInner.color = new Color(speaker.innercolor.r, speaker.innercolor.g, speaker.innercolor.b, 1f);

        if (SpeakerNameDisplay != null)
            SpeakerNameDisplay.SetText(speaker.subjectname);
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

        if (DialogueSegments[dialogueIndex].Choices != null
            && DialogueSegments[dialogueIndex].Choices.Length > 0)
        {
            ShowChoices(DialogueSegments[dialogueIndex].Choices);
        }
        else
        {
            canSkip = true;
        }
    }

    void ShowChoices(DialogueChoice[] choices)
    {
        if (ChoicePanel != null)
            ChoicePanel.SetActive(true);

        for (int i = 0; i < ChoiceButtons.Length; i++)
        {
            if (i < choices.Length)
            {
                ChoiceButtons[i].gameObject.SetActive(true);
                ChoiceLabels[i].SetText(choices[i].ChoiceText);

                int index = i;
                ChoiceButtons[i].onClick.RemoveAllListeners();
                ChoiceButtons[i].onClick.AddListener(() => OnChoicePicked(choices[index].NextSegmentIndex));
            }
            else
            {
                ChoiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void OnChoicePicked(int nextIndex)
    {
        if (ChoicePanel != null)
            ChoicePanel.SetActive(false);

        if (nextIndex < 0 || nextIndex >= DialogueSegments.Length)
        {
            EndDialogue();
            return;
        }

        dialogueIndex = nextIndex;
        SetStyle(DialogueSegments[dialogueIndex].Speaker);
        StartCoroutine(PlayDialogue(DialogueSegments[dialogueIndex].Dialogue));
    }
}

[System.Serializable]
public class DialogueChoice
{
    public string ChoiceText;
    public int NextSegmentIndex;
}

[System.Serializable]
public class DialogueSegment
{
    [TextArea(3, 10)]
    public string Dialogue;
    public Subject Speaker;
    public DialogueChoice[] Choices;
}