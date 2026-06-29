using UnityEngine;

public class ItemRemove : MonoBehaviour
{
    public string itemTag;
    public inventoryItem item;
    public void Remove()
    {
        FindFirstObjectByType<Inventoryscript>().RemoveItemByTag(itemTag);
    }
    public void add()
    {
        FindAnyObjectByType<Inventoryscript>().AddItem(item);
    }
}