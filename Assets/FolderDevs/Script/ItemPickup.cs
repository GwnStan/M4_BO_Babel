using UnityEngine;

    
public class ItemPickup : MonoBehaviour
    {
        public Item item;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                inventoryItem newItem = new inventoryItem();
                newItem.item = item;

                FindFirstObjectByType<Inventoryscript>().AddItem(newItem);
                Destroy(gameObject);
            }
        }
    }

