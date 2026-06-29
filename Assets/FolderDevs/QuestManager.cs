using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartQuest(Quest quest)
    {
        if (quest == null || quest.isActive || quest.isCompleted) return;
        quest.isActive = true;
        Debug.Log($"Quest started: {quest.questname}");
    }

    public void CompleteQuest(Quest quest)
    {
        if (quest == null || quest.isCompleted) return;
        quest.isActive = false;
        quest.isCompleted = true;
        Debug.Log($"Quest completed: {quest.questname}");
    }

    public bool HasActiveQuest(Quest quest) => quest != null && quest.isActive;
    public bool HasCompletedQuest(Quest quest) => quest != null && quest.isCompleted;
}