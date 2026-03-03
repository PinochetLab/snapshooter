using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        private readonly List<InventoryItem> _items = new ();

        public void AddItem(InventoryItem item)
        {
            _items.Add(item);
        }

        public bool HasItem(InventoryItem item)
        {
            return _items.Contains(item);
        }

        public void RemoveItem(InventoryItem item)
        {
            _items.Remove(item);
        }
    }
}