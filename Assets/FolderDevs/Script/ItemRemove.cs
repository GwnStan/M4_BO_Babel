using UnityEngine;

public class ItemRemove : MonoBehaviour
{
    public string itemTag;

    public void Remove()
    {
        FindFirstObjectByType<Inventoryscript>().RemoveItemByTag(itemTag);
    }
}