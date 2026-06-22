using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Dialoguebox : MonoBehaviour
{
    [Header("Dialogue Content")]
    public DialogueSegment[] DialogueSegments;

    [Header("Npc References")]
    public Transform target;

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
    public Inventoryscript PlayerInventory; // drag your inventory script here

    private bool playerInRange = false;
    public bool inDialogue = false;
    private bool canSkip = false;
    private int dialogueIndex = 0;
    private string _fullText;

    void Start()
    {
        if (DialoguePanel != null) DialoguePanel.SetActive(false);
        if (ChoicePanel != null) ChoicePanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
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

        if (inDialogue && Input.GetKeyDown(advanceKey))
        {
            if (!canSkip)
            {
                // Skip to end of current line immediately
                StopAllCoroutines();
                if (DialogueDisplay != null) DialogueDisplay.SetText(_fullText);

                if (DialogueSegments[dialogueIndex].Choices != null
                    && DialogueSegments[dialogueIndex].Choices.Length > 0)
                    ShowChoices(DialogueSegments[dialogueIndex].Choices);
                else
                    canSkip = true;
            }
            else
            {
                AdvanceDialogue();
            }
        }

        if (SkipIndicator != null)
            SkipIndicator.enabled = inDialogue && canSkip;
    }

    void StartDialogue()
    {
        inDialogue = true;
        dialogueIndex = 0;

        if (DialoguePanel != null) DialoguePanel.SetActive(true);

        if (MouseLookScript != null)
        {
            MouseLookScript.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (MovementScript != null) MovementScript.canMove = false;

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

        if (DialoguePanel != null) DialoguePanel.SetActive(false);
        if (ChoicePanel != null) ChoicePanel.SetActive(false);

        if (MouseLookScript != null)
        {
            MouseLookScript.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (MovementScript != null) MovementScript.canMove = true;
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
        _fullText = dialogue;

        if (DialogueDisplay != null) DialogueDisplay.SetText(string.Empty);

        for (int i = 0; i <= dialogue.Length; i++)
        {
            if (DialogueDisplay != null)
                DialogueDisplay.SetText(dialogue.Substring(0, i));

            yield return new WaitForSeconds(1f / textspeed);
        }

        if (DialogueSegments[dialogueIndex].Choices != null
            && DialogueSegments[dialogueIndex].Choices.Length > 0)
            ShowChoices(DialogueSegments[dialogueIndex].Choices);
        else
            canSkip = true;
    }

    void ShowChoices(DialogueChoice[] choices)
    {
        if (ChoicePanel != null) ChoicePanel.SetActive(true);

        for (int i = 0; i < ChoiceButtons.Length; i++)
        {
            if (i >= choices.Length)
            {
                ChoiceButtons[i].gameObject.SetActive(false);
                continue;
            }

            DialogueChoice choice = choices[i];
            bool conditionMet = EvaluateCondition(choice.Condition);

            ChoiceButtons[i].gameObject.SetActive(true);
            ChoiceButtons[i].interactable = conditionMet;

x
            if (!conditionMet && choice.Condition != null)
                ChoiceLabels[i].SetText($"{choice.ChoiceText}\n<size=70%><i>{choice.Condition.GetRequirementText()}</i></size>");
            else
                ChoiceLabels[i].SetText(choice.ChoiceText);

            int index = i;
            ChoiceButtons[i].onClick.RemoveAllListeners();
            ChoiceButtons[i].onClick.AddListener(() => OnChoicePicked(choices[index].NextSegmentIndex));
            ChoiceButtons[i].onClick.AddListener(() => choices[index].onChoiceSelected.Invoke());
        }
    }


    bool EvaluateCondition(ChoiceCondition condition)
    {
        if (condition == null || condition.Type == ConditionType.None)
            return true;

        switch (condition.Type)
        {
            case ConditionType.HasItem:
                if (PlayerInventory == null || condition.RequiredItem == null) return false;
                return PlayerInventory.inventoryItems.Exists(i => i.item.itemTag == condition.RequiredItem.itemTag);

            case ConditionType.HasActiveQuest:
                if (QuestManager.Instance == null || condition.RequiredQuest == null) return false;
                return QuestManager.Instance.HasActiveQuest(condition.RequiredQuest);

            case ConditionType.HasCompletedQuest:
                if (QuestManager.Instance == null || condition.RequiredQuest == null) return false;
                return QuestManager.Instance.HasCompletedQuest(condition.RequiredQuest);

            default:
                return true;
        }
    }

    void OnChoicePicked(int nextIndex)
    {
        if (ChoicePanel != null) ChoicePanel.SetActive(false);

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



public enum ConditionType
{
    None,
    HasItem,
    HasActiveQuest,
    HasCompletedQuest
}

[System.Serializable]
public class ChoiceCondition
{
    public ConditionType Type;


    public Item RequiredItem;
    public Quest RequiredQuest;

    public string GetRequirementText()
    {
        switch (Type)
        {
            case ConditionType.HasItem:
                return RequiredItem != null ? $"Requires: {RequiredItem.Name}" : "Requires: (item not set)";
            case ConditionType.HasActiveQuest:
                return RequiredQuest != null ? $"Requires quest: {RequiredQuest.questname}" : "Requires: (quest not set)";
            case ConditionType.HasCompletedQuest:
                return RequiredQuest != null ? $"Requires completed: {RequiredQuest.questname}" : "Requires: (quest not set)";
            default:
                return string.Empty;
        }
    }
}



[System.Serializable]
public class DialogueChoice
{
    public string ChoiceText;
    public int NextSegmentIndex;
    public ChoiceCondition Condition;
    public UnityEvent onChoiceSelected;
}

[System.Serializable]
public class DialogueSegment
{
    [TextArea(3, 10)]
    public string Dialogue;
    public Subject Speaker;
    public DialogueChoice[] Choices;
}