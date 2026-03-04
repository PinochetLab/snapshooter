using System.Collections.Generic;

namespace Interaction
{
    public interface IInteractable
    {
        bool CanInteract => true;
        string CantInteractText => "";
        string InteractionText { get; }
        void Interact();
    }
}