using UnityEngine;

[CreateAssetMenu(fileName = "new Quest", menuName = "Create new Quest")]
public class Quest : ScriptableObject
{
    public Item[] questitems;
    public string questname;
    public string quest_id;
    public string questDescription;

    [HideInInspector] public bool isActive;
    [HideInInspector] public bool isCompleted;
}