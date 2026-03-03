using UnityEngine;

namespace Interaction
{
    public abstract class AbstractPickable : MonoBehaviour, IInteractable
    {
        public string InteractionText => $"take a {PickableName}";
        
        public void Interact()
        {
            Pick();
            Destroy(gameObject);
        }
        
        protected abstract string PickableName { get; }

        protected abstract void Pick();
    }
}