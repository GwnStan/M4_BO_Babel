using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventoryscript : MonoBehaviour
{
    [Header("Items In Inventory")]
    public List<inventoryItem> inventoryItems = new List<inventoryItem>();

    [Header("Canvas")]
    public Canvas canvas;
    public RectTransform itemPrefab;
    public GameObject inventorypanel;
    public RectTransform content;

    [Header("Settings")]
    public KeyCode InventoryKey = KeyCode.Tab;

    [Header("Scripts")]
    public movement MovementScript;
    public mouseLook MouseLookScript;


    private void Start()
    {
        inventorypanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(InventoryKey))
        {
            if (inventorypanel.activeSelf)
                closeinventory();
            else
                openinventory();
        }
    }

    private void openinventory()
    {
        inventorypanel.SetActive(true);

        foreach (inventoryItem item in inventoryItems)
        {
            RectTransform slot = Instantiate(itemPrefab, content);
            slot.GetComponent<Image>().color = item.item.inventoryboxcolor;
            slot.Find("NameText").GetComponent<TMP_Text>().text = item.item.Name;
            slot.Find("DescriptionText").GetComponent<TMP_Text>().text = item.item.Description;
            slot.Find("Icon").GetComponent<Image>().sprite = item.item.Icon;
        }

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
    }

    private void closeinventory()
    {
        inventorypanel.SetActive(false);

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

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

    public void AddItem(inventoryItem newItem)
    {
        inventoryItems.Add(newItem);
    }

    public void RemoveItemByTag(string tag)
    {
        inventoryItem itemToRemove = inventoryItems.Find(i => i.item.itemTag == tag);
        if (itemToRemove != null)
            inventoryItems.Remove(itemToRemove);
    }

}

[System.Serializable]
public class inventoryItem
{
    public Item item;
    public string inventorydescription;
    public int itemcount;
    public enum itemtype { keyItem, LoreItem }
}