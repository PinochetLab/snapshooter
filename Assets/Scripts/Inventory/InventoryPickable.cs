using Interaction;
using UnityEngine;
using Zenject;

namespace Inventory
{
    public class InventoryPickable : AbstractPickable
    {
        [SerializeField] private InventoryItem item;
        
        [Inject] private InventoryManager _inventoryManager;

        protected override string PickableName => item.ItemName.ToLower();
        
        protected override void Pick()
        {
            _inventoryManager.AddItem(item);
        }
    }
}