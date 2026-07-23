using UnityEngine;




[CreateAssetMenu(fileName = "New Item", menuName = "Create new Item")]
public class Item : ScriptableObject
{
    public string Name;
    public string Description;
    public Color inventoryboxcolor;
    public Sprite Icon;
    public string itemTag;
}
