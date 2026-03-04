using Interaction;
using Inventory;
using UnityEngine;
using Zenject;

namespace Locks
{
    public class Padlock : MonoBehaviour, IInteractable, ILock
    {
        [SerializeField] private GameObject key;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject lockableGameObject;
        [SerializeField] private InventoryItem keyItem;
        
        [Inject] private InventoryManager _inventoryManager;

        private bool _startUnlock;
        private ILockable _lockable;

        public bool CanInteract => !_startUnlock && _inventoryManager.HasItem(keyItem);
        
        public string InteractionText => "unlock";

        private void Awake()
        {
            key.SetActive(false);
            _lockable = lockableGameObject.GetComponent<ILockable>();
        }

        public string CantInteractText
        {
            get
            {
                if (_startUnlock)
                    return "";
                return "No key";
            }
        }

        public void Interact()
        {
            _startUnlock = true;
            key.SetActive(true);
            animator.Play("Unlock", 0, 0);
        }

        public void Unlock()
        {
            _lockable.Unlock();
            Destroy(gameObject);
        }

        public void ShowLocked()
        {
            animator.Play("Shake", 0, 0);
        }
    }
}