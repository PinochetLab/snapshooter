using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "Inventory Item", menuName = "Inventory/Item", order = 0)]
    public class InventoryItem : ScriptableObject
    {
        [SerializeField] private string itemName;
        [SerializeField] private string itemDescription;
        
        public string ItemName => itemName;
    }
}